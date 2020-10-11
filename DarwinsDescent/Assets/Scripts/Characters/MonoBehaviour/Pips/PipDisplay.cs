using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DarwinsDescent.Assets.Scripts.Characters.MonoBehaviour.Pips;

namespace DarwinsDescent
{
    /// <summary>
    /// PipDisplay handles the logic displaying the pipsystem gui 
    /// </summary>
    public class PipDisplay : MonoBehaviour
    {
        // TODO: Should health be a member variable of this class? Or should it come from another class (e.g. Actor subclass)?
        public int numHealthPips;
        //public Image[] healthPips;
        public GameObject pipPrefab;
        public Sprite pipFull;
        public Sprite pipEmpty;
        public PlayerCharacter playerCharacter;
        public PipSystem pipSystem;
        public GameObject Hp_PipPool;
        public GameObject PipPad;
        public Stack<GameObject> pipPoolFull = new Stack<GameObject>();
        public Stack<GameObject> pipPoolEmpty = new Stack<GameObject>();
        public Stack<GameObject> pipPoolTemp = new Stack<GameObject>();
        public Dictionary<string, GameObject> PipHolder = new Dictionary<string, GameObject>();
        public Dictionary<string, Image> PipImageHolder = new Dictionary<string, Image>();
        public Dictionary<string, Text> PipTextHolder = new Dictionary<string, Text>();
        protected Vector3 CanvasOffSet;
        protected Color Disabled = Color.gray;
        protected Color Enabled = Color.yellow;


        // Using OnEnable because it is being called after PlayerCharacter onawake but before pipsystems start
        // This allows the player character to become initialized the pipdisplay to be able to call it and sub to
        // the pipsystems delegates and then have them call the PipDisplay functions.
        void OnEnable()
        {
            if (playerCharacter == null)
            {
                Debug.LogError("Did Not Add A PlayerCharacter to PipDisplay. Fix It.");
                playerCharacter = GetComponent<PlayerCharacter>();
                if(playerCharacter == null)
                {
                    playerCharacter = GameObject.Find("Darwin").GetComponent<PlayerCharacter>();
                }
            }

            if (playerCharacter == null)
            {
                // DONT IGNORE ME SENPI
                EditorUtility.DisplayDialog("Compile Error", "Could Not Find playerCharacter class in PipDisplay", "Got it");
            }

            pipSystem = playerCharacter.GetComponent<PipSystem>();
            // pipSystem.Updated is the publisher and UpdatePipDisplay is subscribing to it.
            // When Updated is invoked the method UpdatePipDisplay will be called.
            if (pipSystem != null)
            {
                pipSystem.Updated += UpdatePipPadDisplay;
                pipSystem.DisplayUpdated += UpdatePipPoolDisplay;
            }
                

            if (Hp_PipPool == null || PipPad == null )
            {
                foreach (Transform child in this.transform)
                {
                    if (Hp_PipPool == null &&
                        child.gameObject.name == "HP_PipPool")
                    {
                        Hp_PipPool = child.gameObject;
                        continue;
                    }
                    if(PipPad == null &&
                        child.gameObject.name == "PipPad")
                    {
                        PipPad = child.gameObject;
                        continue;
                    }
                }
                if(Hp_PipPool == null)
                    Hp_PipPool = GameObject.Find("HP_PipPool");
                if (PipPad == null)
                    PipPad = GameObject.Find("PipPad");
            }
            CanvasOffSet = this.transform.position;
            pipPrefab = (GameObject)Resources.Load("Prefabs/Pip", typeof(GameObject));


            InitializePipPool((PlayerHealth)playerCharacter.damageable.health);
            InitializePipPadDisplay();
        }


        /// <summary>
        /// Adds pip HP images to the ui using the starting PlayerCharacater Health as the amount of pips.
        /// </summary>
        /// <param name="PipPoolCap"></param>
        public void InitializePipPool(PlayerHealth PipPoolInfo)
        {
            if (PipPoolInfo.MaxHP == 0)
            {
                return;
            }

            foreach (Transform child in Hp_PipPool.transform)
            {
                child.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                pipPoolFull.Push(child.gameObject);
            }

            GameObject walkingPipObj;
            while (pipPoolFull.Count > PipPoolInfo.MaxHP)
            {
                walkingPipObj = pipPoolFull.Pop();
                Destroy(walkingPipObj);
            }

            while(pipPoolFull.Count < PipPoolInfo.MaxHP)
            {
                GameObject NewPipToAdd = new GameObject();

                if (pipPoolFull.Count == 0)
                {
                    NewPipToAdd = Instantiate(pipPrefab,
                    new Vector3(0,0),
                    new Quaternion(),
                    Hp_PipPool.transform);
                    RectTransform newrectTransform = (RectTransform)NewPipToAdd.transform;
                    newrectTransform.anchoredPosition = new Vector2(0, 0);
                }
                else
                {
                    walkingPipObj = pipPoolFull?.Peek();
                    RectTransform rectTransform = (RectTransform)walkingPipObj.transform;

                    // unfortunately, something about setting the Vector 3 in the Instantiate is thrown off, and it ends up adding the Canvas transform to the new items transform
                    // instead of the value passed in. Setting the anchored position afterworlds makes it so the Instantiated spawn exactly where they need to.
                    NewPipToAdd = Instantiate(pipPrefab,
                        new Vector3(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f,
                                    rectTransform.anchoredPosition.y),
                        rectTransform.transform.rotation,
                        Hp_PipPool.transform);

                    RectTransform newrectTransform = (RectTransform)NewPipToAdd.transform;
                    newrectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.rect.width + 5f, rectTransform.anchoredPosition.y);
                }
                
                NewPipToAdd.name = pipPrefab.name + (pipPoolFull.Count + 1).ToString();
                NewPipToAdd.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                pipPoolFull.Push(NewPipToAdd);
            }
        }

        public void InitializePipPadDisplay()
        {
            foreach (Transform child in PipPad.transform)
            {
                string partName = child.name.Replace("PipPort", "");
                PipHolder.Add(partName, child.gameObject);

                Image PipImage = child.GetComponent<Image>();
                if (PipImage != null)
                {
                    PipImageHolder.Add(partName, PipImage);
                }

                Text pipText = child.GetComponentInChildren<Text>();
                if(pipText != null)
                {
                    PipTextHolder.Add(partName, pipText);
                }
            }

        }

        /// <summary>
        /// PipPoolDisplay Rulees. There are so far 4 states which need to be taken into consideration.
        /// 1. Normal - Health is stable and will only changed if funnled to the PipPad or depleted by damaged. Is used after any temnp for damage
        /// 2. Damaged - Health that is empty and caused by enemy damage. Can be refilled by back to Nomral by killing enemies.
        /// 3. Funneled - Health that is empty and caused by being funneled to the pippad. Can be filled to normal by returning pips from the pippad.
        /// Can be filled by a temp by killing an enemy.
        /// 4. Temp - Health is unstable and cannot be funneled into the pippad. Usually only gained when there is no damaged health and killed an enemy
        /// while there is funneled Health. When taking damage, used before normal Health. Temp health degrades over time. Degrading time is reset to 
        /// full if an enemy is hit before it is entirely depleted to funneled. If funneled health is returned while temp health exists, the temp stays
        /// as extra health but will not have its degrade time reset.
        /// Resets - 4 slots with extra temp NNDTT. one N gets filtered NFDTT and is in turn filled by temp NDTT. Since four slots and both temp are wirhin the max pool cap they can be reset
        /// Does Not Reset - 4 slots with extra temp NNDTT. one N gets filtered NFDTT and is in turn filled by temp NDTT. Since four slots and both temp are wirhin the max pool cap they can be reset
        /// </summary>
        public void UpdatePipPoolDisplay()
        {

        }

        // Should just update the pippad
        public void UpdatePipPadDisplay(PipModel PipSection)
        {
            //child.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 0f);
            //PipSection.Name.ToString();
            PipTextHolder[PipSection.Name.ToString()].text = PipSection.Allocated.ToString();
            PipTextHolder[PipSection.Name.ToString()].color = Color.black;
            PipImageHolder[PipSection.Name.ToString()].color = Color.grey;

            float percentageFilled = (float)PipSection.Allocated / (float)PipSection.MaxCap;

            if (percentageFilled > 0f &&percentageFilled < .34f)
            {
                PipTextHolder[PipSection.Name.ToString()].color = Color.red;
                PipImageHolder[PipSection.Name.ToString()].color = Color.yellow;
            }
            if(percentageFilled >= .34f && percentageFilled <= .67f )
            {
                PipTextHolder[PipSection.Name.ToString()].color = Color.white;
                // Orange Red https://answers.unity.com/questions/446203/cant-create-orange-label.html
                PipImageHolder[PipSection.Name.ToString()].color = new Color(1.0f, .64f, 0f);
            }
            if (percentageFilled > .67f)
            {
                PipTextHolder[PipSection.Name.ToString()].color = Color.yellow;
                PipImageHolder[PipSection.Name.ToString()].color = Color.red;
            }
        }
    }
}