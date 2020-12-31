using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public PlayerCharacter playerCharacter;

    // Start is called before the first frame update
    void Start()
    {
        if(playerCharacter == null)
        {
            playerCharacter = transform.Find("Darwin")?.GetComponent<PlayerCharacter>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ShowInteractObj();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        HideInteractObj();
    }

    public void ShowInteractObj()
    {
        playerCharacter.InteractObjRenderer.enabled = true;
    }

    public void HideInteractObj()
    {
        playerCharacter.InteractObjRenderer.enabled = false;
    }
}
