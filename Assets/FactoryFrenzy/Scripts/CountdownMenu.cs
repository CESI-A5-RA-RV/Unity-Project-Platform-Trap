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
    AudioSource[] audioSource;

    AudioSource raceStart;
    AudioSource countdownStartSound;
    AudioSource count;
    private bool countdownStarted = false;

    private int countdownStart = 10;
    private string display;

    void Start(){
        countdownMenu.SetActive(false);
        goMessage.SetActive(false);
        audioSource = gameObject.GetComponents<AudioSource>();
        raceStart = audioSource[0];
        countdownStartSound = audioSource[1];
        count = audioSource[2];
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
        countdownStartSound.Play();
        while (countdown > 0){
            player.DisableMovementClientRpc(false);
            
            if(countdown < 6){
                display = countdown.ToString();
                Countdown_TMP.text = display;
            }

            countdown--;
            count.Play();
            countdownMenu.SetActive(true);
            yield return new WaitForSeconds(1f);
            if(countdown == 0){
                Countdown_TMP.text = "";
                goMessage.SetActive(true);
                raceStart.Play();
                yield return new WaitForSeconds(1f);
                countdownMenu.SetActive(false);
                barrier.SetActive(false);
                player.EnableMovementClientRpc();
            }
        }
          
    }
}
