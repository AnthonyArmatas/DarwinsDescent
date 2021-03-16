using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Priority_Queue;
using UnityEngine.InputSystem;

namespace DarwinsDescent
{
    /// <summary>
    /// Manages the logic behind the pipsystem.
    /// </summary>
    //[RequireComponent(typeof(Actor))]
    //[RequireComponent(typeof(Damageable))]

    public class PipSystem : MonoBehaviour
    {
        #region CopyPasteRegion
        #endregion
        public DamageablePlayer Damageable;
        public PlayerHealth playerHealth;


        // public int PipPool; is Actor.health

        //Top
        public PipModel Head = new PipModel();
        //Left
        public PipModel Chest = new PipModel();
        //Right
        public PipModel Arms = new PipModel();
        //Bottom
        public PipModel Legs = new PipModel();

        //public Queue<HPPipModel> hpPips = new Queue<HPPipModel>();
        //public FastPriorityQueue<HPPriorityQueueNode> hpPQPips;
        public Stack<HPPipModel> tempDecayStack = new Stack<HPPipModel>();
        public float timeElapsedOnTopTemp;
        public float lastTimeofTopTemp = 0;
        public Vector3 curTempDecayScale;
        public PipLinkedList pipLinkedList;// = new PipLinkedList(new PipNode(PipNode.StatusKey.Real));
        public bool Refunding;
        public bool HeadToggled;
        public bool ChestToggled;
        public bool ArmsToggled;
        public bool LegsToggled;

        public PlayerCharacter PlayerCharacter;
        public GameObject pipPrefab;
        public GameObject Hp_PipPool;
        public Queue<GameObject> hpPips = new Queue<GameObject>();
        //TODO: Currently Unused, Update UpdateTempPipTime to use this instead of the hardcoded values
        public float PipTempTime = 5f;

        public GameObject PipPad;
        public Dictionary<string, GameObject> PipPadHolder = new Dictionary<string, GameObject>();
        public Dictionary<string, Image> PipPadImageHolder = new Dictionary<string, Image>();
        public Dictionary<string, Text> PipPadTextHolder = new Dictionary<string, Text>();

        private int PipPoolCap;
        private int MinimumRequiredPipsInPool = 1;

        // Used as a work around since the dpad cannot be used as buttons in unity, only as an axis, and by extension you cannot get button up, down , or held. 
        private bool DpadHorWasDown = false;
        private bool DpadVertWasDown = false;
        private float DpadPrevHorVal = 0f;
        private float DpadPrevVertVal = 0f;

        #region Events
        public delegate void InitializePipParts(PipModel Head, PipModel Arms, PipModel Chest, PipModel Legs);
        public InitializePipParts Initialized;

        // Sets up the delegate so that the subscriber knows what it function needs to contain
        public delegate void UpdatePipPoolDisplay(PipNode pipModel);
        // The Event publish. This is what the reviving methods subscribe to. So when update is invoked those other methods will run.
        public event UpdatePipPoolDisplay DisplayUpdated;

        // Sets up the delegate so that the subscriber knows what it function needs to contain
        public delegate void UpdatePipCount(PipModel PipSection, Dictionary<string, Text> PipPadTextHolder, Dictionary<string, Image> PipPadImageHolder);
        // The Event publish. This is what the reviving methods subscribe to. So when update is invoked those other methods will run.
        public event UpdatePipCount Updated;

        #endregion

        // Start is called before the first frame update
        void Start()
        {
            Damageable = GetComponent<DamageablePlayer>();
            if (Damageable != null)
            {
                // Setting this because we cannot cast a property of an object, specifically we cannot cast the health object as player health to get and cast its innards.
                playerHealth = Damageable.playerHealth;

                // Subscribes to UpdateHp so UpdateHPPips will run when called.
                Damageable.UpdateHp += UpdateHPPips;
            }

            if (Hp_PipPool == null)
                Hp_PipPool = GameObject.Find("HP_PipPool");

            if (PipPad == null)
                PipPad = GameObject.Find("PipPad");


            PlayerCharacter = GetComponent<PlayerCharacter>();
            pipPrefab = (GameObject)Resources.Load("Prefabs/Pip", typeof(GameObject));

            // Initializing PipPoolCap in start because DamageablePlayer is initialized in awake and that needs to be set up first
            PipPoolCap = Damageable.StartingHealth;
            // Call function which sets the default/saved values for each of the pip models
            Initialized.Invoke(Head, Arms, Chest, Legs);
            curTempDecayScale = new Vector3(1, 1, 1);

            // Sets the PipLinkedList
            InitializePipPool(playerHealth);
            
            InitializePipPadDisplay();

            if (Updated != null)
            {
                Updated.Invoke(Head, PipPadTextHolder, PipPadImageHolder);
                Updated.Invoke(Arms, PipPadTextHolder, PipPadImageHolder);
                Updated.Invoke(Chest, PipPadTextHolder, PipPadImageHolder);
                Updated.Invoke(Legs, PipPadTextHolder, PipPadImageHolder);
            }
        }

        void Update()
        {
            UpdateTempPipTime();
        }

        void FixedUpdate()
        {
        }

        /// <summary>
        /// Adds pip HP images to the ui using the starting PlayerCharacater Health as the amount of pips.
        /// </summary>
        public void InitializePipPool(PlayerHealth PipPoolInfo)
        {
            if (PipPoolInfo.MaxHP == 0 ||
                Hp_PipPool.transform.childCount > 0)
            {
                 Debug.Log("MaxHP was Zero or had Hp_PipPool.transform.childCount > 0");
                throw new Exception("MaxHP was Zero or had Hp_PipPool.transform.childCount > 0");
            }

            //pipLinkedList.UpdateNodes(PipPoolInfo);
            GameObject newHPPip = Instantiate(pipPrefab,
                        new Vector3(0, 0),
                        new Quaternion(),
                        Hp_PipPool.transform);
            RectTransform newrectTransform = (RectTransform)newHPPip.gameObject.transform;
            newrectTransform.anchoredPosition = new Vector2(0, 0);
            newHPPip.gameObject.name = pipPrefab.name + (hpPips.Count + 1).ToString();
            newHPPip.GetComponent<Image>().color = new Color(255f, 255f, 0f);
            pipLinkedList = new PipLinkedList(
                newHPPip.GetComponent<PipNode>(),
                pipPrefab,
                Hp_PipPool,
                hpPips);
            hpPips.Enqueue(newHPPip);
            UpdateHPPips(playerHealth);
        }

        public void InitializePipPadDisplay()
        {
            foreach (Transform child in PipPad.transform)
            {
                string partName = child.name.Replace("PipPort", "");
                PipPadHolder.Add(partName, child.gameObject);

                Image PipImage = child.GetComponent<Image>();
                if (PipImage != null)
                {
                    PipPadImageHolder.Add(partName, PipImage);
                }

                Text pipText = child.GetComponentInChildren<Text>();
                if (pipText != null)
                {
                    PipPadTextHolder.Add(partName, pipText);
                }
            }
        }

        public void MovePips(PipModel PipSection)
        {
            if (Updated == null)
            {
                throw new ArgumentNullException(nameof(Updated));
            }

            // if there are not enough pips to give or the pip in question is locked or the max cap has been reached, return.
            if (PipSection.MaxCap <= 0 ||
                PipSection.Locked)
            {
                return;
            }

            // If the left trigger or num pad plus is held then return all pips
            if (Refunding)
            {
                Damageable.GetBackLoanHealth(PipSection);
                if (playerHealth.CurHealth > playerHealth.MaxHP)
                {
                    RectTransform rectTransform = new RectTransform();
                    for (int tempPipsToAdd = playerHealth.CurHealth - playerHealth.MaxHP;
                        tempPipsToAdd > 0; tempPipsToAdd--)
                    {
                        
                        // Just add the largest key so it is added to the end of the LL. The visuals will be updated later.
                        pipLinkedList.AddNode(new PipNode(PipNode.StatusKey.Damaged));

                    }
                }
                Updated.Invoke(PipSection, PipPadTextHolder, PipPadImageHolder);
                UpdateHPPips(playerHealth);
                PipSection.ApplyPipModifications(PlayerCharacter);
                return;
            }


            if (playerHealth.RealHp > playerHealth.MinRealHp &&
                PipSection.Allocated < PipSection.MaxCap)
            {
                // Call Pip Display to remove a pip and replace it with an empty one
                // Call the Damagable script lose a perm health
                PipSection.Allocated++;
                Damageable.LoanHealth(1);
                Updated.Invoke(PipSection, PipPadTextHolder, PipPadImageHolder);
                PipSection.ApplyPipModifications(PlayerCharacter);
            }
        }

        /// from there was can walk through the list linked list
        /// updating each and every game object image.
        /// when it gets to the end of the list it:
        ///  - Checks if there are more nodes connected to it
        ///  - If so it recursively goes in it and checks to see if it has more connected
        ///  - Once it does not, it destroys that game object and returns 
        ///  and the objects do the same until it gets back to the begining
        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerHP"></param>
        public void UpdateHPPips(PlayerHealth playerHP)
        {
            pipLinkedList.UpdateNodes(playerHealth);
            DisplayUpdated.Invoke(pipLinkedList.Head);
        }

        public void UpdatePipPad(PipModel pipModel)
        {
            Updated.Invoke(pipModel, PipPadTextHolder, PipPadImageHolder);
        }

        public void UpdateTempPipTime()
        {
            if (playerHealth.TempHp == 0 || pipLinkedList.LastTempPip == null)
            {
                timeElapsedOnTopTemp = 0;
                return;
            }

            //float sectionAmount = 1 / PipTempTime;
            float sectionAmount = .2f;
            int secondToDecreaseBy = 2;
            timeElapsedOnTopTemp += Time.deltaTime;

            //Update it every two sections
            if((timeElapsedOnTopTemp - lastTimeofTopTemp) >= secondToDecreaseBy)
            {
                lastTimeofTopTemp = timeElapsedOnTopTemp;
                pipLinkedList.LastTempPip.gameObject.transform.localScale -= new Vector3(sectionAmount, 0, 0);
                if(pipLinkedList.LastTempPip.gameObject.transform.localScale.x < .1f)
                {
                    pipLinkedList.LastTempPip.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    playerHealth.TempHp -= 1;
                    pipLinkedList.LastTempPip = null;
                }
                UpdateHPPips(playerHealth);
            }
        }

        public void IsRefunding(InputAction.CallbackContext Value)
        {
            Debug.Log(Value);
            if (Value.performed)
                Refunding = true;
            else if (Value.canceled)
                Refunding = false;
        }

        public void IsTogglingPip(InputAction.CallbackContext Value)
        {
            if (Value.performed)
            {
                switch (Value.action.name)
                {
                    case "NorthPip":
                        MovePips(Head);
                        break;
                    case "EastPip":
                        MovePips(Arms);
                        break;
                    case "SouthPip":
                        MovePips(Legs);
                        break;
                    case "WestPip":
                        MovePips(Chest);
                        break;
                    default:
                        return;
                }
            }
        }
    }
}