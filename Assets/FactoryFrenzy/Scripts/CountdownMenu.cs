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
    private bool countdownStarted = false;

    private int countdownStart = 10;
    private string display;

    void Start(){
        countdownMenu.SetActive(false);
        goMessage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            ThirdPersonController playerMove = other.GetComponent<ThirdPersonController>();
            if(!countdownStarted){
                countdownStarted = true;
                StartCoroutine(startCountdown(playerMove));
            }
            
        }
    }

    private IEnumerator startCountdown(ThirdPersonController player){
        int countdown = countdownStart;
        while (countdown > 0){
            player.DisableMovementClientRpc(false);
            
            if(countdown < 6){
                display = countdown.ToString();
                Countdown_TMP.text = display;
            }

            countdown--;
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
