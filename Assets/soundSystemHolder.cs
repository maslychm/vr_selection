using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundSystemHolder : MonoBehaviour
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
