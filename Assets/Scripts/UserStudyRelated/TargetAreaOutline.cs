using cakeslice;
using UnityEngine;

public class TargetAreaOutline : MonoBehaviour
{
    static Outline searchOutline;
    static Transform _transform;
    static Vector3 searchOffset = Vector3.zero;

    void Start()
    {
        searchOutline = GetComponent<Outline>();
        if (searchOutline == null) Debug.LogError("Search Area outline not found!");

        searchOutline.enabled = true;
        searchOutline.enabled = false;

        _transform = transform;
    }

    /// <summary>
    /// Enable a shader outline around the position, which should be same
    /// as vector of where the target is.
    /// </summary>
    /// <param name="searchPosition"></param>
    public static void EnableSearchOutlineAroundPosition(Vector3 camPosition, Vector3 searchPosition, bool useNewOffset)
    {
        if (useNewOffset)
            searchOffset = Random.insideUnitSphere * 1.5f;

        //print($"offset: {searchOffset}");

        // direction of the object from camera
        Vector3 objDir = (searchPosition - camPosition).normalized;
        // 5 meters from the camera position in the direction of search position
        objDir = camPosition + objDir * 10f;
        // Add random offset
        objDir += searchOffset;

        _transform.position = objDir;

        searchOutline.enabled = true;
    }

    public static void DisableSearchOutlineAroundPosition()
    {
        searchOutline.enabled = false;
    }
}
