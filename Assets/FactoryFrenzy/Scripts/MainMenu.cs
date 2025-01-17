using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject HomeLayout;
    [SerializeField] private GameObject SettingsLayout;
    [SerializeField] private GameObject QuitLayout;
    [SerializeField] private GameObject LobbyLayout;
    [SerializeField] private TMP_Text Version_TMP;
    private  static GameObject _previousLayout;
    private static GameObject _currentLayout;

    // Start is called before the first frame update
    void Start()
    {
        HomeLayout.SetActive(true);
        SettingsLayout.SetActive(false);
        QuitLayout.SetActive(false);
        LobbyLayout.SetActive(false);

        _currentLayout = HomeLayout;
        Version_TMP.text = "V."+ Application.version;
    }

    public void PlayGame(){
        SwitchLayout(LobbyLayout);
    }

    public void OnSettings(){
        SwitchLayout(SettingsLayout);
    }

    public void OnQuit(){
        SwitchLayout(QuitLayout);
    }

    public void confirmQuit(){
        Application.Quit();
    }

    public void onBack(){
        SwitchLayout(_previousLayout);
    }

    private void SwitchLayout(GameObject layout)
    {
        //TO-DO : Faite en sorte de switch de scene on fonction de la derniere et de la nouvelle
        _previousLayout = _currentLayout;
        layout.SetActive(true);
        _currentLayout = layout;
        _previousLayout.SetActive(false);
        
    } 
}
