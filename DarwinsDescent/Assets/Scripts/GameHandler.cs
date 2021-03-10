using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;
using System;
using UnityEngine.SceneManagement;
using DarwinsDescent;

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

    // Update is called once per frame
    void Update()
    {
        if (CreditsAnimator.GetBool("StartCredits"))
        {
            if (PlayerInput.Instance.Pause.Down)
            {
                RestartLevel();
            }
        }
        else
        {
            if (PlayerInput.Instance.Pause.Up)
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
        if (GameIsPaused)
        {
            if (PlayerInput.Instance.MeleeAttack.Up ||
                PlayerInput.Instance.Jump.Up)
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
