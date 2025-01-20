using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class EndLevel : NetworkBehaviour
{
    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private TMP_Text Victory_TMP;
    [SerializeField] private GameObject countdownMenu;

    ThirdPersonController player;
    Rigidbody rbPlayer;

    private List<ulong> playerRanking = new List<ulong>();
    private bool countdownStarted = false;
    private int countdownStart = 30;
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

    private IEnumerator startCelebrate(){
        yield return new WaitForSeconds(2f);
        //rbPlayer.velocity = Vector3.zero;
        //player.DisableMovement();
    
        int countdown = countdownStart;
        while (countdown > 0){
            countdown--;
            NotifyCountdownTimerClientRpc(countdown);
            yield return new WaitForSeconds(1f);
        }

        countdownMenu.SetActive(false);
    }

    private void EndGame(){
        //Block all players movements
        //Display ranking
        //Add possibility to display name
        int noRank = playerRanking.Count;
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds){
            if(!playerRanking.Contains(clientId)){
                //block player's movements
                playerRanking.Add(clientId);
            }
        }
        NotifyFinalRankingsClientRpc(playerRanking.ToArray(), noRank);
    }

    [ClientRpc]
    private void NotifyFinalRankingsClientRpc(ulong[] finalRankings, int noRank)
    {
        Debug.Log("Final Rankings:");
        for (int i = 0; i < noRank; i++)
        {
            //Add UI for ranking
            Debug.Log($"Rank {i + 1}: Player {finalRankings[i]}");
        }
        for (int i = noRank; i < finalRankings.Length; i++){
            Debug.Log($"Rank {i + 1}: Player {finalRankings[i]}");
        }
    }

}
