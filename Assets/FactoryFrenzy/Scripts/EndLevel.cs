using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System.Threading.Tasks;
using Unity.Services.Authentication;

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

    private Dictionary<ulong, string> playerRanking = new Dictionary<ulong, string>();
    private List<string> playerOut = new List<string>();
    private bool countdownStarted = false;
    private int countdownStart = 5;
    async void Start(){
        countdownMenu.SetActive(false);
        await getLobby();
    }

    public void Initialized(Lobby lobby){
        currentLobby = lobby;
    }

    private void OnTriggerEnter(Collider other){
        if(!IsServer) return;

        if(other.gameObject.CompareTag("Player")){
            var playerId = other.GetComponent<NetworkObject>();
            if(!playerRanking.ContainsKey(playerId.OwnerClientId)){
                playerRanking.Add(playerId.OwnerClientId, AuthenticationService.Instance.PlayerName);
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
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds){
            if(!playerRanking.ContainsKey(clientId)){
                //block player's movements
                playerOut.Add(clientId.ToString());
            }
        }
        
        NotifyFinalRankingsClientRpc(noRank);
        StartCoroutine(returnToLobby());
    }

    [ClientRpc]
    private void NotifyFinalRankingsClientRpc(int noRank)
    {
        countdownMenu.SetActive(false);
        RankingMenu.SetActive(true);
        Debug.Log("Final Rankings:");
        int i = 1;
        foreach(var key in playerRanking.Keys){
            //Add UI for ranking
            
            GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
            newItem.SetActive(true);
            TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
            //Debug.LogWarning($"Rank Index number: {i}");
            rankingTexts.text = $"Rank {i} - {playerRanking[key]}";
            i++;
        }
        
        foreach(var j in playerOut){
            //Add UI for disqualified/eliminated players
            GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
            newItem.SetActive(true);
            TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
            //Debug.LogWarning($"Eliminated Index number: {i}");
            rankingTexts.text = $"Rank {noRank} - {j}";
                
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
        string playerID = AuthenticationService.Instance.PlayerId;
        string playerName = AuthenticationService.Instance.PlayerName;
        Debug.LogWarning($"PlayerId: {playerID} and name: {playerName}");
        Lobby currentLobby = await LobbyService.Instance.GetLobbyAsync(lobbyId);
        return currentLobby;
    }

}
