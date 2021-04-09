using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallowBoss_Wake_Music_Play : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioSource WakeUpMusic = GetComponent<AudioSource>();
        WakeUpMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
