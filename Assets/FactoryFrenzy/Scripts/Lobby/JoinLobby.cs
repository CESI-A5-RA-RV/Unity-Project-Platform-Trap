using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoinLobby : MonoBehaviour
{

    [SerializeField] public TMP_Text NameLobby;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnJoin()
    {
        string Lobby = NameLobby.text;
        PlayerPrefs.SetString("Lobby Name", Lobby);
        SceneManager.LoadSceneAsync("Lobby");//doesn't load, why ?
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
