using UnityEngine;

/// <summary>
/// this file manages the hide from user rectangle that shows up
/// </summary>
public class HideViewOfSpheresController : MonoBehaviour
{
    private void Start()
    {
        ShowTheBarrier();
    }

    public void ShowTheBarrier()
    {
        Debug.Log("Hiding the User View");
        gameObject.SetActive(true);
    }

    public void HideTheBarrier()
    {
        Debug.Log("showing the User the View");
        gameObject.SetActive(false);
    }
}