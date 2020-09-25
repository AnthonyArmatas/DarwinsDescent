using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    [RequireComponent(typeof(Damageable))]

    public class PipSystem : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion
        public Damageable damageable;

        public int pipPool;
        public PipModel Head;
        public PipModel Chest;
        public PipModel Arms;
        public PipModel Legs;




        // Start is called before the first frame update
        void Awake()
        {
            damageable = GetComponent<Damageable>();
            pipPool = damageable.CurHealth;
        }


    }

}