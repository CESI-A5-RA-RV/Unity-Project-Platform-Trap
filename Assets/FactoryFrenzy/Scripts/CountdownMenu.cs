using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountdownMenu : MonoBehaviour
{
    [SerializeField] private GameObject countdownMenu;
    [SerializeField] private TMP_Text Version_TMP;

    private int countdownStart = 10;

    void Start(){
        gameObject.SetActive(false);
        countdownStart = 10;
        StartCoroutine(startCountdown());
    }

    private IEnumerator startCountdown(){
        gameObject.SetActive(true);
        int countdown = countdownStart;
        string display;
        while (countdown > 0){
            display = countdown.ToString();
            Version_TMP.text = display;
            countdown -= 1;
            yield return new WaitForSeconds(1f);
        }

        display = "GO !";
        
    }
}
