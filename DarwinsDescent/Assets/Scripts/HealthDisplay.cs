using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    // TODO: Should health be a member variable of this class? Or should it come from another class (e.g. Actor subclass)?
    public int health;
    public int numHealthPips;
    public Image[] healthPips;
    public Sprite pipFull;
    public Sprite pipEmpty;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < healthPips.Length; i++) {
            if (health > numHealthPips) {
                health = numHealthPips;
            }

            if (i < health) {
                healthPips[i].sprite = pipFull;
            } else {
                healthPips[i].sprite = pipEmpty;
            }

            if (i < numHealthPips) {
                healthPips[i].enabled = true;
            } else {              
                healthPips[i].enabled = false;
            }
        }
    }
}
