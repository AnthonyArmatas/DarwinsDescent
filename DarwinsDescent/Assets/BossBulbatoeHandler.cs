using DarwinsDescent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulbatoeHandler : MonoBehaviour
{
    List<BossBulbatoes> UnactivatedBulbatoes = new List<BossBulbatoes>();
    List<BossBulbatoes> ActivatedBulbatoes = new List<BossBulbatoes>();

    public float TimeBetweenActivations;
    public float ActivationTimer;
    public bool Fightstarted;
    
    private static bool BossDefeated;

    // Start is called before the first frame update
    void Start()
    {
        foreach (BossBulbatoes bulbatoe in this.transform.GetComponentsInChildren<BossBulbatoes>())
        {
            UnactivatedBulbatoes.Add(bulbatoe);
        }

        if (TimeBetweenActivations == 0)
            TimeBetweenActivations = 5f;

        BossDefeated = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Fightstarted && !BossDefeated)
        {
            if (ActivationTimer >= TimeBetweenActivations &&
                UnactivatedBulbatoes.Count > 0)
            {
                int NewBulbatoeIndex = Random.Range(0, UnactivatedBulbatoes.Count - 1);

                ActivatedBulbatoes.Add(UnactivatedBulbatoes[NewBulbatoeIndex]);
                BossBulbatoes bulbatoe = UnactivatedBulbatoes[NewBulbatoeIndex];
                UnactivatedBulbatoes.Remove(bulbatoe);
                bulbatoe.StartGrowth();
                ActivationTimer = 0;
            }
            else
            {
                ActivationTimer += Time.deltaTime;
            }
        }
    }

    public void ResetBulbatoe(BossBulbatoes bulbatoe)
    {
        ActivatedBulbatoes.Remove(bulbatoe);
        UnactivatedBulbatoes.Add(bulbatoe);
    }

    public static void BossKilled()
    {
        BossDefeated = true;
    }
}
