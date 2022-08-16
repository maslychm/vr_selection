using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SciptableObjects", menuName = "vr_selection/SciptableObjects", order = 0)]

public class SciptableObjects : ScriptableObject
{
    [SerializeField] public GameObject prefab;

    public Vector3 spawnPostition;

    public Quaternion spawnRotation;
}
