using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class OpeningSceneHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(InputAction.CallbackContext Value)
    {
        if (Value.performed)
        {
            StartLevel();
        }
    }

    public void StartLevel()
    {
        SceneManager.LoadScene("FirstLevel");
    }
}
