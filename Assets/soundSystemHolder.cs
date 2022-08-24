using UnityEngine;

public class soundSystemHolder : MonoBehaviour
{
    public AudioSource incorrectSelectionSound;
    public AudioSource validSelectionSound;

    public void correctPlay()
    {
        validSelectionSound.Play();
    }

    public void incorrectPlay()
    {
        incorrectSelectionSound.Play();
    }
}