using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using System;
using UnityEngine.SceneManagement;
using DarwinsDescent;
using UnityEngine.InputSystem;

public class GameHandler : MonoBehaviour
{
    // Used to check if the demo is over;
    public Animator CreditsAnimator;
    public GameObject PauseMenuUI;
    public static bool GameIsPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        if(CreditsAnimator == null)
        {
            CreditsAnimator = GameObject.Find("Credits").GetComponent<Animator>();
        }
    }


    public void HitPause(InputAction.CallbackContext Value)
    {
        if (Value.performed)
        {
            if (CreditsAnimator.GetBool("StartCredits"))
            {
                RestartLevel();
            }
            else
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
    }

    public void ConfirmRestart(InputAction.CallbackContext Value)
    {
        if (GameIsPaused)
        {
            if (Value.performed)
            {
                Resume();
                RestartLevel();
            }
        }
    }

    private void Resume()
    {
        PauseMenuUI.SetActive(false);
        // Unpauses the gameworld
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        // Pauses the gameworld
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("FirstLevel");
    }
}
