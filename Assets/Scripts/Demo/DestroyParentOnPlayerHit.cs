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
                SameStar();
                break;
            case "pyramid":
                SamePyramid();
                break;
            case "sphere":
                SameSphere();
                break;
            
            
        }
    }

    public void SameCube()
    {
        if (gameObject.tag.Contains("cube"))
        {
            DestroyEnemyParent();
        }
    }

    public void SameStar()
    {
        if (gameObject.tag.Contains("star"))
        {
            DestroyEnemyParent();
        }
    }

    public void SameSphere()
    {
        if (gameObject.tag.Contains("sphere"))
        {
            DestroyEnemyParent();
        }
    }

    public void SamePyramid()
    {
        if (gameObject.tag.Contains("pyramid"))
        {
            DestroyEnemyParent();
        }
    }

}
