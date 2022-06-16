using UnityEngine;

public class Trigger : MonoBehaviour
{
    public int health = 3;
    public bool isDead = false;

    private void Update()
    {
        if (health == 0 && !isDead)
        {
            // End screen or something, player loses
            print("GAME END");
            isDead = true;

            // TODO actually end game here
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("enemy"))
        {
            //print($"{other.tag} hit PLAYER");
            HealthDamage();
        }
    }

    private void HealthDamage()
    {
        // Add possible health damage affect
        health--;
    }
}