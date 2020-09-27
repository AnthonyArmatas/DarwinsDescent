using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;


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
        public Actor actor;
        public PipSystem pipSystem;
        public GameObject Hp_PipPool;
        public Stack<GameObject> pipPool = new Stack<GameObject>();
        protected Vector3 CanvasOffSet;

        // Start is called before the first frame update
        void Start()
        {
            if (actor == null)
            {
                actor = GetComponent<Actor>();
                if(actor == null)
                {
                    actor = GameObject.Find("Darwin").GetComponent<Actor>();
                }
            }

            if (actor == null)
            {
                // DONT IGNORE ME SENPI
                EditorUtility.DisplayDialog("Compile Error", "Could Not Find Actor class in PipDisplay", "Got it");
            }

            pipSystem = actor.GetComponent<PipSystem>();

            if (Hp_PipPool == null)
            {
                foreach (Transform child in this.transform)
                {
                    if (child.gameObject.name == "HP_PipPool")
                    {
                        Hp_PipPool = child.gameObject;
                        break;
                    }
                }
                if(Hp_PipPool == null)
                    Hp_PipPool = GameObject.Find("HP_PipPool");
            }
            CanvasOffSet = this.transform.position;
            pipPrefab = (GameObject)Resources.Load("Prefabs/Pip", typeof(GameObject));


            InitializePipPool(actor.health);
            //InitializePipFunnelDisplay(actor.health);
        }

        // Update is called once per frame
        void Update()
        {
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
    }
}