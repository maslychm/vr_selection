using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class DestroyParentOnPlayerHit : MonoBehaviour
{
    
    private void DestroyEnemyParent()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {

        // string otherTag = "as";
        // if (otherTag.Lower().Contains())

        print($"collided with: {other.tag}");

        switch (other.tag)
        { 
            case "Player":
                DestroyEnemyParent();
                break;

            case "cube":
                print("RUN");
                if (gameObject.tag.Contains("cube"))
                    DestroyEnemyParent();
                break;

            case "star":
                if (gameObject.tag.Contains("star"))
                    DestroyEnemyParent();
                break;

            case "pyramid":
                if (gameObject.tag.Contains("pyramid"))
                    DestroyEnemyParent();
                break;

            case "sphere":
                if (gameObject.tag.Contains("shpere"))
                    DestroyEnemyParent();
                break;
            
        }
    }

}
