using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DarwinsDescent
{
    /// <summary>
    /// Manages the logic behind the pipsystem.
    /// </summary>
    [RequireComponent(typeof(Actor))]
    [RequireComponent(typeof(Damageable))]

    public class PipSystem : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion
        public Damageable Damageable;

        public int PipPoolCap;
        public int PipPool;
        public int MinimumRequiredPipsInPool;
        public PipModel Head;
        public PipModel Chest;
        public PipModel Arms;
        public PipModel Legs;
        public Actor Actor;




        // Start is called before the first frame update
        void Awake()
        {
            Damageable = GetComponent<Damageable>();
            Actor = GetComponent<Actor>();
            PipPool = Actor.health;
        }


    }

}