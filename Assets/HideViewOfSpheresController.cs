using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this file manages the hide from user rectangle that shows up
/// </summary>
public class HideViewOfSpheresController : MonoBehaviour
{
    private void Start()
    {
        showTheBarrier();
    }
    public void showTheBarrier()
    {

        Debug.Log("Hiding the User View");
        this.gameObject.SetActive(true);
    }

    public void hideTheBarrier()
    {
        Debug.Log("showing the User the View");
        this.gameObject.SetActive(false);
    }
}
