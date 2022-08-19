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

        gameObject.SetActive(true);
    }

    public void HideTheBarrier()
    {

        gameObject.SetActive(false);
    }
}