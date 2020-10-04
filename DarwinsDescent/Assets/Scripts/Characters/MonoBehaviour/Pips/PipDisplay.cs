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
        public Stack<GameObject> pipPool = new Stack<GameObject>();
        public Dictionary<string, GameObject> PipHolder = new Dictionary<string, GameObject>();
        public Dictionary<string, Image> PipImageHolder = new Dictionary<string, Image>();
        public Dictionary<string, Text> PipTextHolder = new Dictionary<string, Text>();
        protected Vector3 CanvasOffSet;
        protected Color Disabled = new Color(127f, 127f, 127f);
        protected Color Enabled = new Color(191f, 191f, 0f);


        // Start is called before the first frame update
        void Start()
        {
            if (playerCharacter == null)
            {
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
                pipSystem.Updated += UpdatePipDisplay;

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


            InitializePipPool(playerCharacter.health.MaxHP);
            InitializePipPadDisplay();
        }


        /// <summary>
        /// Adds pip HP images to the ui using the starting PlayerCharacater Health as the amount of pips.
        /// </summary>
        /// <param name="PipPoolCap"></param>
        public void InitializePipPool(int PipPoolCap)
        {
            if (PipPoolCap == 0)
            {
                return;
            }

            foreach (Transform child in Hp_PipPool.transform)
            {
                child.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                pipPool.Push(child.gameObject);
            }

            GameObject walkingPipObj;
            while (pipPool.Count > PipPoolCap)
            {
                walkingPipObj = pipPool.Pop();
                Destroy(walkingPipObj);
            }

            while(pipPool.Count < PipPoolCap)
            {
                GameObject NewPipToAdd = new GameObject();

                if (pipPool.Count == 0)
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
                    walkingPipObj = pipPool?.Peek();
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
                
                NewPipToAdd.name = pipPrefab.name + (pipPool.Count + 1).ToString();
                NewPipToAdd.GetComponent<Image>().color = new Color(255f, 255f, 0f);
                pipPool.Push(NewPipToAdd);
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

        public void UpdatePipDisplay(PipModel PipSection)
        {
            //child.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 0f);
            PipSection.Name.ToString();
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