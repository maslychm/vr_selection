using UnityEngine;

public class SoundSystemHolder : MonoBehaviour
{
    [SerializeField] private AudioSource incorrectSelectionSound;
    [SerializeField] private AudioSource validSelectionSound;

    public void correctPlay()
    {
        incorrectSelectionSound.Stop();
        validSelectionSound.Play();
    }

    public void incorrectPlay()
    {
        validSelectionSound.Stop();
        incorrectSelectionSound.Play();
    }
}