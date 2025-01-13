using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private GameObject Victory_TMP;
    [SerializeField] private GameObject countdownMenu;

    private List<string> playerRanking = new List<string>();
    private int countdownStart = 60;
    private string display;
    Deathzone deathzone;
    KillPlayer killPlayer;
    void Start(){
        countdownMenu.SetActive(false);
        deathzone = GameObject.Find("Deathzone").GetComponent<Deathzone>();
        killPlayer = GameObject.Find("GameManager").GetComponent<KillPlayer>();
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            deathzone.respawnPositions = gameObject.transform.position;
            killPlayer.lastCheckpoint = gameObject.transform.position;
            StartCoroutine(startCelebrate());
        }
    }

    private IEnumerator startCelebrate(){
        countdownMenu.SetActive(true);
        yield return new WaitForSeconds(3f);
        Victory_TMP.SetActive(false);
        int countdown = countdownStart;

        while (countdown >= 0){
            display = countdown.ToString();
            Countdown_TMP.text = display;
            countdown -= 1;
            countdownMenu.SetActive(true);
            yield return new WaitForSeconds(1f);
        }

        countdownMenu.SetActive(false);
    }

    private void addPlayer(string name){
        playerRanking.Add(name);
    }
}
