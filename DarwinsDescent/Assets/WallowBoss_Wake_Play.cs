using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallowBoss_Wake_Play : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        AudioSource WakeSound = GetComponent<AudioSource>();
        WakeSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
