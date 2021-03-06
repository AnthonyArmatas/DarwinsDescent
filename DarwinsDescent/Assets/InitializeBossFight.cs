﻿using Cinemachine;
using UnityEngine;

namespace DarwinsDescent
{
    public class InitializeBossFight : MonoBehaviour
    {
        public CinemachineVirtualCamera Cinemachine;
        public CinemachineFramingTransposer transposer;
        public Transform BossCamera;
        public Transform CameraOriginalTransform;
        public Collider2D BossBarrier;
        public PlayerCharacter playerCharacter;
        public WallowBoss wallowBoss;
        public AudioSource WallBossIdle;

        #region Events
        public delegate void StopAllMusic();
        public StopAllMusic StopTheMusic;

        #endregion

        void Start()
        {
            if (Cinemachine == null)
                Cinemachine = GameObject.Find("CM_vcam1").GetComponent<CinemachineVirtualCamera>();

            if (transposer == null)
                transposer = Cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();

            if (BossCamera == null)
                BossCamera = this.transform.Find("CameraTarget").GetComponent<Transform>();

            if (CameraOriginalTransform == null)
                CameraOriginalTransform = Cinemachine.Follow;

            if (BossBarrier == null)
                BossBarrier = GameObject.Find("BossBarrier").GetComponent<Collider2D>();

            if (playerCharacter == null)
                playerCharacter = GameObject.Find("Darwin").GetComponent<PlayerCharacter>();
            if (wallowBoss == null)
                wallowBoss = GameObject.Find("WallowBossCore").GetComponent<WallowBoss>();

        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.name == "Darwin")
            {
                Cinemachine.Follow = BossCamera;
                Cinemachine.m_Lens.OrthographicSize = 2.55f;
                transposer.m_SoftZoneHeight = 0.942f;
                transposer.m_SoftZoneWidth = 1.178f;
                transposer.m_DeadZoneWidth = 0;
                transposer.m_DeadZoneHeight = 0;
                BossBarrier.enabled = true;
                playerCharacter.movementDisabled = true;
                wallowBoss.animator.SetTrigger("EnteredRange");
                WallBossIdle.Stop();
                StopTheMusic.Invoke();
                Debug.Log("ENTERED");   
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.name == "Darwin")
            {
                Cinemachine.Follow = CameraOriginalTransform;
                Cinemachine.m_Lens.OrthographicSize = 1.75f;
                transposer.m_SoftZoneHeight = 0.485f;
                transposer.m_SoftZoneWidth = 0.571f;
                transposer.m_DeadZoneWidth = 0.328f;
                transposer.m_DeadZoneHeight = 0.299f;
                Debug.Log("ENTERED");
            }

        }

        ///Original Orthographic Size
        ///1.75
        ///Soft Zone Width
        ///0.485
        ///Soft Zone Height
        ///0.571
        ///New Orthographic Size
        ///2.55
        ///New Zone Width
        ///1.178
        ///New Zone Height
        ///0.942
    }
}
