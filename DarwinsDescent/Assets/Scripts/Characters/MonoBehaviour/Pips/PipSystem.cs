using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour;
using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Priority_Queue;

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

        public Queue<HPPipModel> hpPips = new Queue<HPPipModel>();
        //public FastPriorityQueue<HPPriorityQueueNode> hpPQPips;
        public Stack<HPPipModel> tempDecayStack = new Stack<HPPipModel>();
        public float timeElapsedOnTopTemp;
        public float lastTimeofTopTemp = 0;
        public Vector3 curTempDecayScale;

        public PlayerCharacter PlayerCharacter;
        public GameObject pipPrefab;
        public GameObject Hp_PipPool;
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
        public delegate void UpdatePipPoolDisplay(HPPipModel pipModel);
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
            //hpPQPips = new FastPriorityQueue<HPPriorityQueueNode>(playerHealth.MaxHP);
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
            AssignPips();
            //PlayerInput.Instance.Horizontal.Value
        }

        /// <summary>
        /// Adds pip HP images to the ui using the starting PlayerCharacater Health as the amount of pips.
        /// </summary>
        public void InitializePipPool(PlayerHealth PipPoolInfo)
        {
            if (PipPoolInfo.MaxHP == 0)
            {
                return;
            }

            GameObject previousPipObj = new GameObject();
            //while (hpPQPips.Count < PipPoolInfo.MaxHP)
            while (hpPips.Count < PipPoolInfo.MaxHP)
            {
                HPPipModel NewPipToAdd;
                
                //If the normal queue implementation is slow take another look at the pri queue
                //if (hpPQPips.Count == 0)
                if (hpPips.Count == 0)
                {
                    NewPipToAdd = Instantiate(pipPrefab,
                    new Vector3(0, 0),
                    new Quaternion(),
                    Hp_PipPool.transform).GetComponent<HPPipModel>();
                    NewPipToAdd.PipDisplayImage = NewPipToAdd.gameObject.GetComponent<Image>();
                    NewPipToAdd.TempTime = PipTempTime;
                    NewPipToAdd.CurState = HPPipModel.state.Real;

                    RectTransform newrectTransform = (RectTransform)NewPipToAdd.gameObject.transform;
                    newrectTransform.anchoredPosition = new Vector2(0, 0);
                }
                else
                {
                    // Check to make sure this makes sense.
                    // Debug above and make sure when previousPipObj is instatiated its null
                    if (previousPipObj.name == "New Game Object")
                    {
                        previousPipObj = hpPips?.Peek().gameObject;
                    }

                    RectTransform rectTransform = (RectTransform)previousPipObj.transform;

                    // unfortunately, something about setting the Vector 3 in the Instantiate is thrown off, and it ends up adding the Canvas transform to the new items transform
                    // instead of the value passed in. Setting the anchored position afterworlds makes it so the Instantiated spawn exactly where they need to.
                    NewPipToAdd = Instantiate(pipPrefab,
                        new Vector3(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f,
                                    rectTransform.anchoredPosition.y),
                        rectTransform.transform.rotation,
                        Hp_PipPool.transform).GetComponent<HPPipModel>();
                    NewPipToAdd.PipDisplayImage = NewPipToAdd.gameObject.GetComponent<Image>();
                    NewPipToAdd.TempTime = PipTempTime;
                    NewPipToAdd.CurState = HPPipModel.state.Real;

                    RectTransform newrectTransform = (RectTransform)NewPipToAdd.gameObject.transform;
                    newrectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f, rectTransform.anchoredPosition.y);
                }

                NewPipToAdd.gameObject.name = pipPrefab.name + (hpPips.Count + 1).ToString();
                NewPipToAdd.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                hpPips.Enqueue(NewPipToAdd);
                //HPPriorityQueueNode newHPNode = new HPPriorityQueueNode(NewPipToAdd);
                //hpPQPips.Enqueue(new HPPriorityQueueNode(NewPipToAdd), (float)HPPipModel.state.Real);
                previousPipObj = NewPipToAdd.gameObject;
            }

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

        public void AssignPips()
        {
            if (PipPoolCap == MinimumRequiredPipsInPool)
                return;

            if (DpadVertWasDown == true &&
                PlayerInput.Instance.DPadVertical.ReceivingInput == false)
            {
                if (DpadPrevVertVal > 0)
                {
                    MovePips(Head);
                }
                if (DpadPrevVertVal < 0)
                {
                    MovePips(Legs);
                }
            }

            if (DpadHorWasDown == true && 
                PlayerInput.Instance.DPadHorizontal.ReceivingInput == false)
            {
                if (DpadPrevHorVal > 0)
                {
                    MovePips(Arms);
                }
                if (DpadPrevHorVal < 0)
                {
                    MovePips(Chest);
                }
            }

            DpadVertWasDown = PlayerInput.Instance.DPadVertical.ReceivingInput;
            DpadHorWasDown = PlayerInput.Instance.DPadHorizontal.ReceivingInput;
            DpadPrevVertVal = PlayerInput.Instance.DPadVertical.Value;
            DpadPrevHorVal = PlayerInput.Instance.DPadHorizontal.Value;

        }

        public void MovePips(PipModel PipSection)
        {
            if (Updated == null)
            {
                throw new ArgumentNullException(nameof(Updated));
            }

            // if there are not enough pips to give or the pip in question is locked or the max cap has been reached, return.
            if (PipSection.MaxCap <= 1  || 
                PipSection.Locked)
            {
                return;
            }

            // If the left trigger or num pad plus is held then return all pips
            if(PlayerInput.Instance.RefundPip.Value != 0)
            {
                // TODO: Call Damageable to refund health, filling up slots 
                // and using the rest as temp.
                Damageable.GetBackLoanHealth(PipSection);
                if (playerHealth.CurHealth > playerHealth.MaxHP)
                {
                    RectTransform rectTransform = new RectTransform();
                    for (int tempPipsToAdd = playerHealth.CurHealth - playerHealth.MaxHP;
                        tempPipsToAdd > 0; tempPipsToAdd--)
                    {
                        // Get the first instance from the function
                        if(tempPipsToAdd == playerHealth.CurHealth - playerHealth.MaxHP)
                                rectTransform = (RectTransform)GetLastItemInQueue(true).transform;

                        // unfortunately, something about setting the Vector 3 in the Instantiate is thrown off, and it ends up adding the Canvas transform to the new items transform
                        // instead of the value passed in. Setting the anchored position afterworlds makes it so the Instantiated spawn exactly where they need to.
                        HPPipModel NewPipToAdd = Instantiate(pipPrefab,
                            new Vector3(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f,
                                        rectTransform.anchoredPosition.y),
                            rectTransform.transform.rotation,
                            Hp_PipPool.transform).GetComponent<HPPipModel>();
                        NewPipToAdd.PipDisplayImage = NewPipToAdd.gameObject.GetComponent<Image>();
                        NewPipToAdd.TempTime = PipTempTime;
                        NewPipToAdd.CurState = HPPipModel.state.Real;

                        RectTransform newrectTransform = (RectTransform)NewPipToAdd.gameObject.transform;
                        newrectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f, rectTransform.anchoredPosition.y);

                        NewPipToAdd.gameObject.name = pipPrefab.name + (hpPips.Count + 1).ToString();
                        NewPipToAdd.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                        hpPips.Enqueue(NewPipToAdd);
                        //HPPriorityQueueNode newHPNode = new HPPriorityQueueNode(NewPipToAdd);
                        //hpPQPips.Enqueue(new HPPriorityQueueNode(NewPipToAdd), (float)HPPipModel.state.Real);
                        rectTransform = (RectTransform)NewPipToAdd.gameObject.transform;
                    }
                }
                Updated.Invoke(PipSection, PipPadTextHolder, PipPadImageHolder);
                UpdateHPPips(playerHealth);
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
            }
        }

        public void UpdateHPPips(PlayerHealth playerHP)
        {
            //if(playerHP.CurHealth > playerHP.MaxHP &&
            //    playerHP.RealHp == playerHP.MaxHP)
            //{
            //    // If there is an incoming full health this looks like it will skip that, fix it
            //    //Add new temp HPPips to pippool
            //    return;
            //}

            #region PriQueue Implimentation To come back to

            //FastPriorityQueue<HPPriorityQueueNode> tempQueue = new FastPriorityQueue<HPPriorityQueueNode>(hpPQPips.Count);
            //for(int RealPips = playerHP.RealHp; RealPips > 0; RealPips--)
            //{
            //    if (hpPQPips.First.Priority == (float)HPPipModel.state.Real)
            //    {
            //        tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Real);
            //        continue;
            //    }
            //    hpPQPips.First.PipModel.CurState = HPPipModel.state.Real;
            //    tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Real);
            //}

            //for(int tempPips = playerHP.TempHp; tempPips > 0; tempPips--)
            //{
            //    if (hpPQPips.First.Priority == (float)HPPipModel.state.Temp)
            //    {
            //        tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Temp);
            //        continue;
            //    }
            //    hpPQPips.First.PipModel.CurState = HPPipModel.state.Temp;
            //    tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Temp);
            //}
            //for (int lentPips = playerHP.LentHp; lentPips > 0; lentPips--)
            //{
            //    if (hpPQPips.First.Priority == (float)HPPipModel.state.Lent)
            //    {
            //        tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Lent);
            //        continue;
            //    }
            //    hpPQPips.First.PipModel.CurState = HPPipModel.state.Lent;
            //    tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Lent);
            //}

            //while(hpPQPips.Count > 0)
            //{
            //    if (hpPQPips.First.Priority == (float)HPPipModel.state.Damaged)
            //    {
            //        tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Damaged);
            //        continue;
            //    }
            //    hpPQPips.First.PipModel.CurState = HPPipModel.state.Damaged;
            //    tempQueue.Enqueue(hpPQPips.Dequeue(), (float)HPPipModel.state.Damaged);
            //}

            //hpPQPips = tempQueue;       
            #endregion


            Queue<HPPipModel> tempQueue = new Queue<HPPipModel>();

            if (playerHP.RealHp <= 0)
            {
                while(hpPips.Count > 0)
                {
                    hpPips.Peek().CurState = HPPipModel.state.Damaged;
                    DisplayUpdated.Invoke(hpPips.Peek());
                    tempQueue.Enqueue(hpPips.Dequeue());
                }
                hpPips = tempQueue;
                return;
            }

            
            for (int RealPips = playerHP.RealHp; RealPips > 0; RealPips--)
            {
                hpPips.Peek().gameObject.transform.localScale = new Vector3(1, 1, 1);
                if (hpPips.Peek().CurState == HPPipModel.state.Real)
                {
                    tempQueue.Enqueue(hpPips.Dequeue());
                    continue;
                }
                hpPips.Peek().CurState = HPPipModel.state.Real;
                DisplayUpdated.Invoke(hpPips.Peek());
                tempQueue.Enqueue(hpPips.Dequeue());
            }

            for (int tempPips = playerHP.TempHp; tempPips > 0; tempPips--)
            {
                hpPips.Peek().gameObject.transform.localScale = new Vector3(1, 1, 1);
                if (hpPips.Peek().CurState == HPPipModel.state.Temp)
                {
                    tempDecayStack.Push(hpPips.Peek());
                    tempQueue.Enqueue(hpPips.Dequeue());
                    continue;
                }
                hpPips.Peek().CurState = HPPipModel.state.Temp;
                tempDecayStack.Push(hpPips.Peek());
                DisplayUpdated.Invoke(hpPips.Peek());
                tempQueue.Enqueue(hpPips.Dequeue());
            }

            if(tempDecayStack.Count != 0 && playerHP.TempHp != 0)
                tempDecayStack.Peek().gameObject.transform.localScale = curTempDecayScale;

            for (int lentPips = playerHP.LentHp; lentPips > 0; lentPips--)
            {
                if (hpPips.Count > 0)
                {
                    hpPips.Peek().gameObject.transform.localScale = new Vector3(1, 1, 1);
                    if (hpPips.Peek().CurState == HPPipModel.state.Lent)
                    {
                        tempQueue.Enqueue(hpPips.Dequeue());
                        continue;
                    }
                    hpPips.Peek().CurState = HPPipModel.state.Lent;
                    DisplayUpdated.Invoke(hpPips.Peek());
                    tempQueue.Enqueue(hpPips.Dequeue());
                }
            }

            while (playerHealth.CurHealth + playerHealth.LentHp < hpPips.Count + tempQueue.Count)
            {
                Destroy(GetLastItemInQueue(false).gameObject);
                hpPips.Dequeue();
            }

            while (hpPips.Count > 0)
            {
                hpPips.Peek().gameObject.transform.localScale = new Vector3(1, 1, 1);
                if (hpPips.Peek().CurState == HPPipModel.state.Damaged)
                {
                    tempQueue.Enqueue(hpPips.Dequeue());
                    continue;
                }
                hpPips.Peek().CurState = HPPipModel.state.Damaged;
                DisplayUpdated.Invoke(hpPips.Peek());
                tempQueue.Enqueue(hpPips.Dequeue());
            }

            hpPips = tempQueue;
        }

        public void UpdateTempPipTime()
        {
            if (playerHealth.TempHp == 0 || tempDecayStack.Count == 0)
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
                HPPipModel pipModel = tempDecayStack.Peek();
                pipModel.gameObject.transform.localScale -= new Vector3(sectionAmount, 0, 0);
                if(pipModel.gameObject.transform.localScale.x < .1f)
                {
                    pipModel.gameObject.transform.localScale = new Vector3(1, 1, 1);
                    tempDecayStack.Pop();
                    if (playerHealth.CurHealth > playerHealth.MaxHP)
                    {
                        Destroy(GetLastItemInQueue(false).gameObject);
                        // TODO: Super Inefficient refactor this whole function and any related functions.
                        pipModel = GetLastItemInQueue(true);
                    }
                    playerHealth.TempHp -= 1;
                    //pipModel.CurState = HPPipModel.state.Lent;
                    
                    UpdateHPPips(playerHealth);
                }
                curTempDecayScale = pipModel.gameObject.transform.localScale;
            }
        }

        public HPPipModel GetLastItemInQueue(bool KeepLastInQueue)
        {
            if (hpPips?.Count == 0)
                return null;
            if (hpPips?.Count == 1)
                return hpPips.Peek();


            HPPipModel first = hpPips.Peek();
            HPPipModel current = null;
            while (true)
            {
                current = hpPips.Dequeue();
                if (hpPips.Peek() == first)
                {
                    break;
                }
                hpPips.Enqueue(current);
            }
            
            if(KeepLastInQueue)
                hpPips.Enqueue(current);
            return current;
        }
    }
}