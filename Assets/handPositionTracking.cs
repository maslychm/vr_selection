using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handPositionTracking : MonoBehaviour
{

    // declare a game object that stores the current hand 
    private GameObject handObject;
    private Vector3 handPosition;

    // declare a list of all the hand positions over time 
    private List<Vector3> handPositions = new List<Vector3>();

    // hold the distance travelled 
    public static Vector3 totalDistanceTravelled = new Vector3(0,0,0);

    // Start is called before the first frame update
    void Start()
    {

        handObject = this.gameObject;
        handPosition = transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {

        if(ExperimentTrial.isTrialOngoingNow == false)
        {
            flushList();
            return;
        }

        handPosition = transform.position;
        handPositions.Add(handPosition);
        totalDistanceTravelled = new Vector3(0, 0, 0);
        calculateAllDistance();
        
    }

    // clear the list of vectors 
    private void flushList()
    {
        
        handPositions.Clear();

        totalDistanceTravelled = new Vector3(0, 0, 0);
        
    }

    private void calculateAllDistance()
    {
        // sum up all distance travelled 
        for(int i = 0; i < handPositions.Count - 1; i++)
        {
            totalDistanceTravelled += handPositions[i + 1] - handPositions[i];
        }

    }

    public static Vector3 getDistanceTravelledByLeftHand()
    {
        return totalDistanceTravelled;
    }
}
