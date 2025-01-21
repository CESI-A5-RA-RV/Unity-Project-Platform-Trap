using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class EndLevel : NetworkBehaviour
{ // Nom des joueurs, stopper les joueurs, retour au lobby

    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private TMP_Text Victory_TMP;
    [SerializeField] private GameObject countdownMenu;
    [SerializeField] private GameObject RankingMenu;

    [SerializeField] private GameObject rankingItem;

    public Transform parentRanking;

    ThirdPersonController player;
    Rigidbody rbPlayer;

    private List<ulong> playerRanking = new List<ulong>();
    private List<ulong> playerOut = new List<ulong>();
    private bool countdownStarted = false;
    private int countdownStart = 5;
    void Start(){
        countdownMenu.SetActive(false);
    }

    private void OnTriggerEnter(Collider other){
        if(!IsServer) return;

        if(other.gameObject.CompareTag("Player")){
            player = other.gameObject.GetComponent<ThirdPersonController>();
            rbPlayer = other.gameObject.GetComponent<Rigidbody>();
            var playerId = other.GetComponent<NetworkObject>();
            if(player != null && !playerRanking.Contains(playerId.OwnerClientId)){
                playerRanking.Add(playerId.OwnerClientId);
                int rank = playerRanking.Count;

                NotifyPlayerRankClientRpc(playerId.OwnerClientId, rank);
            }
            if(!countdownStarted){
                countdownStarted = true;
                StartCoroutine(startCelebrate());
            }
            
            
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

    private void StopPlayer(ulong clientId)
    {   //Call this in server
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
        {
            NetworkObject networkObject = networkClient.PlayerObject;
            player = networkObject.GetComponent<ThirdPersonController>();
            rbPlayer = networkObject.GetComponent<Rigidbody>();
            rbPlayer.velocity = Vector3.zero;
            player.DisableMovement();
        }
    }

    private IEnumerator startCelebrate(){
        yield return new WaitForSeconds(1f);
        
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
        //Display ranking
        //Add possibility to display name
        Debug.Log("Start ranking");
        
        int noRank = playerRanking.Count + 1;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds){
            StopPlayer(clientId);
            if(!playerRanking.Contains(clientId)){
                //block player's movements
                playerOut.Add(clientId);
            }
        }
        
        NotifyFinalRankingsClientRpc(playerRanking.ToArray(), noRank, playerOut.ToArray());
    }

    [ClientRpc]
    private void NotifyFinalRankingsClientRpc(ulong[] finalRankings, int noRank, ulong[] eliminatedPlayers)
    {
        countdownMenu.SetActive(false);
        RankingMenu.SetActive(true);
        Debug.Log("Final Rankings:");
        int i = 1;
        for(i =0; i < noRank-1; i++){
            //Add UI for ranking
            GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
            newItem.SetActive(true);
            TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
            Debug.LogWarning($"Rank Index number: {i}");
            rankingTexts.text = $"Rank {i + 1} - {finalRankings[i]}";    
        }
        
        for(i = 0; i < eliminatedPlayers.Length; i++){
                //Add UI for ranking
                GameObject newItem = Instantiate(rankingItem, rankingItem.transform.position ,rankingItem.transform.rotation, parentRanking);
                newItem.SetActive(true);
                TMP_Text rankingTexts = newItem.GetComponent<TMP_Text>();
                Debug.LogWarning($"Eliminated Index number: {i}");
                rankingTexts.text = $"Rank {noRank} - {eliminatedPlayers[i]}";
                
        }
            
    }

}
