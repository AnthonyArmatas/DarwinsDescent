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
        public GameObject pipFullHPPrefab;
        public GameObject pipLentHPPrefab;
        public Sprite pipFull;
        public Sprite pipEmpty;
        public PipSystem pipSystem;

        protected Color Disabled = new Color(((127f * 100f / 255f) / 100f), ((127f * 100f / 255f) / 100f), ((127f * 100f / 255f) / 100f));
        protected Color Enabled = new Color(255f, 255f, 0f);
        protected Color Temp = new Color(0f, ((222f * 100f / 255f) / 100f), ((255f * 100f / 255f) / 100f));



        // Using OnEnable because it is being called after PlayerCharacter onawake but before pipsystems start
        // This allows the player character to become initialized the pipdisplay to be able to call it and sub to
        // the pipsystems delegates and then have them call the PipDisplay functions.
        void OnEnable()
        {
            if (pipSystem == null)
            {
                Debug.LogError("Did Not Add A pipSystem to PipDisplay. Fix It.");
                pipSystem = GetComponent<PipSystem>();
                if(pipSystem == null)
                {
                    pipSystem = GameObject.Find("Darwin").GetComponent<PipSystem>();
                }
            }

            if (pipSystem != null)
            {
                pipSystem.Updated += UpdatePipPadDisplay;
                pipSystem.DisplayUpdated += UpdatePipPoolDisplay;
            }

            pipFullHPPrefab = (GameObject)Resources.Load("Prefabs/Pip", typeof(GameObject));
            pipLentHPPrefab = (GameObject)Resources.Load("Prefabs/FunnelPip", typeof(GameObject));
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
        /// <param name="pipModel"></param>
        public void UpdatePipPoolDisplay(HPPipModel pipModel)
        {
            switch (pipModel.CurState)
            {
                case HPPipModel.state.Real:
                    pipModel.PipDisplayImage.sprite = pipFullHPPrefab.GetComponent<Image>().sprite;
                    pipModel.PipDisplayImage.color = Enabled;
                    break;
                case HPPipModel.state.Temp:
                    pipModel.PipDisplayImage.sprite = pipFullHPPrefab.GetComponent<Image>().sprite;
                    pipModel.PipDisplayImage.color = Temp;
                    break;
                case HPPipModel.state.Damaged:
                    pipModel.PipDisplayImage.sprite = pipFullHPPrefab.GetComponent<Image>().sprite;
                    pipModel.PipDisplayImage.color = Disabled;
                    break;
                case HPPipModel.state.Lent:
                    pipModel.PipDisplayImage.sprite = pipLentHPPrefab.GetComponent<Image>().sprite;
                    pipModel.PipDisplayImage.color = Color.white;
                    break;
                default:
                    break;
            }
        }

        public void DisplayFunnelHealth()
        {

        }
        /// <summary>
        /// Sets up taking Damage
        /// </summary>
        public void DisplayTakeDamage()
        {

        }

        public void DisplayHealHealth()
        {

        }





        /// <summary>
        /// Should just update the pippad
        /// </summary>
        /// <param name="PipSection"></param>
        /// <param name="PipPadTextHolder"></param>
        /// <param name="PipPadImageHolder"></param>
        public void UpdatePipPadDisplay(PipModel PipSection, Dictionary<string, Text> PipPadTextHolder, Dictionary<string, Image> PipPadImageHolder)
        {
            //child.gameObject.GetComponent<Image>().color = new Color(255f, 255f, 0f);
            //PipSection.Name.ToString();
            PipPadTextHolder[PipSection.Name.ToString()].text = PipSection.Allocated.ToString();
            PipPadTextHolder[PipSection.Name.ToString()].color = Color.black;
            PipPadImageHolder[PipSection.Name.ToString()].color = Color.grey;

            float percentageFilled = (float)PipSection.Allocated / (float)PipSection.MaxCap;

            if (percentageFilled > 0f &&percentageFilled < .34f)
            {
                PipPadTextHolder[PipSection.Name.ToString()].color = Color.red;
                PipPadImageHolder[PipSection.Name.ToString()].color = Color.yellow;
            }
            if(percentageFilled >= .34f && percentageFilled <= .67f )
            {
                PipPadTextHolder[PipSection.Name.ToString()].color = Color.white;
                // Orange Red https://answers.unity.com/questions/446203/cant-create-orange-label.html
                PipPadImageHolder[PipSection.Name.ToString()].color = new Color(1.0f, .64f, 0f);
            }
            if (percentageFilled > .67f)
            {
                PipPadTextHolder[PipSection.Name.ToString()].color = Color.yellow;
                PipPadImageHolder[PipSection.Name.ToString()].color = Color.red;
            }
        }
    }
}