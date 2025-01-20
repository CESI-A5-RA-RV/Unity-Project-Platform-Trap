using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using MainMenu;

public class JoinLobby : MonoBehaviour
{

    [SerializeField] public TMP_Text NameLobby;
    [SerializeField] public TMP_Text TypeLobby;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnJoin()
    {
        string lobbyName = NameLobby.text;
        string lobbyType = TypeLobby.text;


        if (lobbyType == "Private")
        {
            ShowCodeInput(true);
        }
        else
        {
            PlayerPrefs.SetString("Lobby Name", lobbyName);
            SceneManager.LoadSceneAsync("Lobby");
        }
    }

    private void ShowCodeInput(bool show)
    {

        GameObject codeInputPanel = GameObject.Find("CodeInputPanel");
        if (codeInputPanel != null)
        {
            codeInputPanel.SetActive(show);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
