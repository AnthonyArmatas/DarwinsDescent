using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicHandler : MonoBehaviour
{
    public AudioSource AboveGroundMusic;
    public AudioSource BelowGroundMusic;
    public AudioSource AboveGroundAmbiance;
    public AudioSource BelowGroundAmbiance;
    public Animator SoundControllerAnimator;
    public InitializeBossFight initializeBossFight;

    // Start is called before the first frame update
    void Start()
    {
        initializeBossFight.StopTheMusic += StopMusic;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Darwin")
        {
            SoundControllerAnimator.SetBool("BelowGround", true);
            if (!BelowGroundMusic.isPlaying && !BelowGroundAmbiance.isPlaying)
            {
                BelowGroundMusic.Play();
                BelowGroundAmbiance.Play();
            }
            //AboveGroundMusic.Stop();
            //AboveGroundAmbiance.Stop();
            //BelowGroundMusic.Play();
            //BelowGroundAmbiance.Play();
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Darwin")
        {
            SoundControllerAnimator.SetBool("BelowGround", false);
            //BelowGroundMusic.Stop();
            //BelowGroundAmbiance.Stop();
            //AboveGroundMusic.Play();
            //AboveGroundAmbiance.Play();
        }        
    }

    public void StopMusic()
    {
        BelowGroundMusic.Stop();
        BelowGroundAmbiance.Stop();
        AboveGroundMusic.Stop();
        AboveGroundAmbiance.Stop();
    }
}
