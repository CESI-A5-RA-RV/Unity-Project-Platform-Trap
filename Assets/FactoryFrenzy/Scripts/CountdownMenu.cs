using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownMenu : MonoBehaviour
{
    [SerializeField] private GameObject countdownMenu;
    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private GameObject goMessage;
    [SerializeField] private GameObject barrier;

    private int countdownStart = 10;
    private string display;
    Deathzone deathzone;
    KillPlayer killPlayer;

    void Start(){
        countdownMenu.SetActive(false);
        goMessage.SetActive(false);
        countdownStart = 10;
        deathzone = GameObject.Find("Deathzone").GetComponent<Deathzone>();
        killPlayer = GameObject.Find("GameManager").GetComponent<KillPlayer>();
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            deathzone.respawnPositions = gameObject.transform.position;
            killPlayer.lastCheckpoint = gameObject.transform.position;
            ThirdPersonController playerMove = other.GetComponent<ThirdPersonController>();
            StartCoroutine(startCountdown(playerMove));
        }
    }

    private IEnumerator startCountdown(ThirdPersonController player){
        int countdown = countdownStart;
        while (countdown > 0){
            player.DisableMovementClientRpc();
            
            if(countdown < 6){
                display = countdown.ToString();
                Countdown_TMP.text = display;
            }

            countdown -= 1;
            countdownMenu.SetActive(true);
            yield return new WaitForSeconds(1f);
            if(countdown == 0){
                Countdown_TMP.text = "";
                goMessage.SetActive(true);
                yield return new WaitForSeconds(1f);
                countdownMenu.SetActive(false);
                barrier.SetActive(false);
                player.EnableMovementClientRpc();
            }
        }
          
    }
}
