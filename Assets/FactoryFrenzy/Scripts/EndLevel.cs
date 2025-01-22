using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using System.Linq;

public class EndLevel : NetworkBehaviour
{ // Nom des joueurs

    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private TMP_Text Victory_TMP;
    [SerializeField] private GameObject countdownMenu;
    [SerializeField] private GameObject RankingMenu;

    [SerializeField] private GameObject rankingItem;

    public Transform parentRanking;

    private ThirdPersonController playerController;

    private Lobby currentLobby;

    private List<ulong> playerRanking = new List<ulong>();
    private List<string> playerRankingName = new List<string>();
    private List<string> playerOut = new List<string>();
    private bool countdownStarted = false;
    private int countdownStart = 5;
    private string username;
    async void Start(){
        countdownMenu.SetActive(false);
        currentLobby = await getLobby();
        username = getPlayerName();
    }

    private void OnTriggerEnter(Collider other){
        if(!IsServer) return;

        if(other.gameObject.CompareTag("Player")){
            var playerId = other.GetComponent<NetworkObject>();
            if(!playerRanking.Contains(playerId.OwnerClientId)){
                playerRanking.Add(playerId.OwnerClientId);
                Debug.Log($"Final name {username}");
                playerRankingName.Add(username);
                int rank = playerRanking.Count;

                NotifyPlayerRankClientRpc(playerId.OwnerClientId, rank);
            }
            if(!countdownStarted){
                countdownStarted = true;
                StartCoroutine(startCelebrate());
            }
            Debug.LogWarning($"Client ID: {playerId.OwnerClientId}");
            StartDisabledMovementServerRpc(playerId.OwnerClientId);
            
            
        }
    }

    [ClientRpc]
    private void NotifyCountdownTimerClientRpc(int timeLeft)
    {
        countdownMenu.SetActive(true);
        Countdown_TMP.text = $"Countdown: {timeLeft} seconds remaining!";
    }

    [ClientRpc]
    private void NotifyPlayerRankClientRpc(ulong clientId, int rank)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            countdownMenu.SetActive(true);
            Victory_TMP.text = $"Congrats!\nYou finished in rank {rank}!";
        }
    }

    [ServerRpc]
    public void StartDisabledMovementServerRpc(ulong clientId){
        if(IsServer){
            StartCoroutine(StopPlayer(clientId));
        }
    }

    private IEnumerator StopPlayer(ulong clientId)
    {   //Call this in server
        yield return new WaitForSeconds(0.5f);
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
        {
            Debug.LogWarning($"Client ID: {clientId}");
            NetworkObject networkObject = networkClient.PlayerObject;
            playerController = networkObject.GetComponent<ThirdPersonController>();
            playerController.DisableMovementClientRpc();
        }
        else{
            Debug.LogWarning("Player not identified, can't disable movement");
        }
    }

    private IEnumerator startCelebrate(){
        int countdown = countdownStart;
        while (countdown > 0){
            countdown--;
            NotifyCountdownTimerClientRpc(countdown);
            yield return new WaitForSeconds(1f);
        }
        
        EndGame();
    }

   
    private void EndGame(){
        if(!IsServer) return;
        RankPlayersServerRpc();
    }

    [ServerRpc]
    private void RankPlayersServerRpc(){
        //Block all players movements
        //Add possibility to display name
        Debug.Log("Start ranking");
        
        int noRank = playerRanking.Count + 1;
        foreach (var name in currentLobby.Players){
            if(!playerRankingName.Contains(name.Data["Username"].Value)){
                playerOut.Add(name.Data["Username"].Value);
            }
        }
        
        NotifyFinalRankingsClientRpc(new NetworkStringArray {Array = playerRankingName.ToArray()}, noRank, new NetworkStringArray {Array = playerOut.ToArray()});
        StartCoroutine(returnToLobby());
    }

    [ClientRpc]
    private void NotifyFinalRankingsClientRpc(NetworkStringArray finalRankings, int noRank, NetworkStringArray eliminatedPlayers)
    {
        countdownMenu.SetActive(false);
        RankingMenu.SetActive(true);
        Debug.Log("Final Rankings:");
        
        for(int i = 0; i < finalRankings.Array.Length; i++){
            //Add UI for ranking
            GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
            newItem.SetActive(true);
            TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
            //Debug.LogWarning($"Rank Index number: {i}");
            rankingTexts.text = $"Rank {i + 1} - {finalRankings.Array[i]}";
        
        }
        
        for(int j = 0; j < eliminatedPlayers.Array.Length; j++){
            //Add UI for disqualified/eliminated players
            GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
            newItem.SetActive(true);
            TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
            //Debug.LogWarning($"Eliminated Index number: {i}");
            rankingTexts.text = $"Rank {noRank} - {eliminatedPlayers.Array[j]}";
                
        }
            
    }

    private IEnumerator returnToLobby(){
        yield return new WaitForSeconds(10f);
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds){
            NetworkObject player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            if(player !=null){
                player.GetComponent<ThirdPersonController>().EnableMovementClientRpc();
            }
        }
        NetworkManager.SceneManager.LoadScene("LobbyEmpty", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private async Task<Lobby> getLobby(){
        string lobbyId = PlayerPrefs.GetString("Lobby ID");
        Lobby currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        return currentLobby;
    }

    private string getPlayerName(){
        string playerID = AuthenticationService.Instance.PlayerId;
        string playerName;
        Debug.LogWarning($"PlayerId: {playerID}");
        foreach(var player in currentLobby.Players){
            if(player.Id == playerID){
                playerName = player.Data["Username"].Value;
                Debug.LogWarning($"name: {playerName}");
                return playerName;
            }
        }
        return null;
        
    }

}
