using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int health = 3;
    public bool isDead = false;

    void Update()
    {
        if (health == 0 && !isDead)
        {
            // End screen or something, player loses
            print("end");
            isDead = true;
        }
    }

    public void OnTriggerEnter(Collider enemy)
    {
        print(enemy.name);

        switch (enemy.tag)
        { 
            case "enemy":
                HealthDamage();
                break;
        }
    }

    private void HealthDamage()
    {
        // Add possible health damage affect
        health--;
    }
}
