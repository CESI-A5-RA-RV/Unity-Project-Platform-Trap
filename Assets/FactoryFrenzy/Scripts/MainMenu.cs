using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using UnityEngine.UI;


namespace MainMenu
{

    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject HomeLayout;
        [SerializeField] private GameObject SettingsLayout;
        [SerializeField] private GameObject QuitLayout;
        [SerializeField] private GameObject LobbyLayout;
        [SerializeField] private GameObject PopUpPanel;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private TMP_Text Version_TMP;
        //[SerializeField] public TMP_InputField NameInput;
        //[SerializeField] public TMP_Dropdown TypeLobby;
        [SerializeField] public TMP_InputField PrivateCodeInput;
        //[SerializeField] public GameObject LobbyItemPrefab;
        //[SerializeField] private LobbyDataManager lobbyDataManager;
        private static GameObject _previousLayout;
        private static GameObject _currentLayout;
        //public Transform contentParent;
               
        private TaskCompletionSource<bool> _taskCompletionSource;

        // Start is called before the first frame update
        async void Start()
        {
            if (!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
            {
                await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            await UnityServices.InitializeAsync();
            HomeLayout.SetActive(true);
            SettingsLayout.SetActive(false);
            QuitLayout.SetActive(false);
            LobbyLayout.SetActive(false);
            PopUpPanel.SetActive(false);

            _currentLayout = HomeLayout;
            Version_TMP.text = "V." + Application.version;
            await InitializeUnityServicesAsync();
        }

        private async Task InitializeUnityServicesAsync()
        {
            // Initialize Unity Services
            await UnityServices.InitializeAsync();

            // Authenticate the player
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                //Debug.Log("Player signed in with ID: " + AuthenticationService.Instance.PlayerId);
            }
            
        }

        public void PlayGame()
        {
            HomeLayout.SetActive(false);
            SwitchLayout(LobbyLayout);
        }


        public void OnSettings()
        {
            SwitchLayout(SettingsLayout);
        }

        public void OnQuit()
        {
            SwitchLayout(QuitLayout);
        }

        public void confirmQuit()
        {
            Application.Quit();
        }

        public void onBack()
        {
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


    

}


