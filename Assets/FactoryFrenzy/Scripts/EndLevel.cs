using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private TMP_Text Countdown_TMP;
    [SerializeField] private GameObject Victory_TMP;
    [SerializeField] private GameObject countdownMenu;

    ThirdPersonController player;
    Rigidbody rbPlayer;

    private List<string> playerRanking = new List<string>();
    private int countdownStart = 60;
    private string display;
    void Start(){
        countdownMenu.SetActive(false);
    }

    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            player = other.gameObject.GetComponent<ThirdPersonController>();
            rbPlayer = other.gameObject.GetComponent<Rigidbody>();
            PlayerData playerData = other.gameObject.GetComponent<PlayerData>();
            addPlayer(playerData.Username.Value);
            StartCoroutine(startCelebrate());
        }
    }

    private IEnumerator startCelebrate(){
        rbPlayer.velocity = Vector3.zero;
        player.DisableMovement();
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
        Debug.Log(playerRanking);
    }
}
