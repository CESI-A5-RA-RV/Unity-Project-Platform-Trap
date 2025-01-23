using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused;

    public GameObject pauseMenu;
    public CinemachineFreeLook cinemachineCamera;
    // Start is called before the first frame update
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }
    }

    public void PauseGame(){
        pauseMenu.SetActive(true);
        isPaused = true;
        ToggleCamera(false);
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        isPaused = false;
        ToggleCamera(true);
    }

    public void GoToMainMenu(){
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
       
    }

    public void QuitGame(){
        Application.Quit();
    }

    private void ToggleCamera(bool isEnabled){
        if (cinemachineCamera != null){
            cinemachineCamera.enabled = isEnabled;
        } 
    }

    public void ChangeScene(){
        isPaused = false;
        if(NetworkManager.Singleton.IsHost){
            NetworkManager.Singleton.SceneManager.LoadScene("AvatarTest", LoadSceneMode.Single);
        }
        
    }
}
