using UnityEngine;

public class RightHandDebug : MonoBehaviour
{
    public TabletUI tabletUI;
    public GameObject hmdCamera;
    public GameObject reticle;

    private void Update()
    {
        var handInHMD = hmdCamera.transform.InverseTransformPoint(transform.position);
        var reticleInHMD = hmdCamera.transform.InverseTransformPoint(reticle.transform.position);
        var reticleToHand = handInHMD - reticleInHMD;

        var text = $"hand in cam: {handInHMD} \nreticle to hand: {reticleToHand}";

        tabletUI.UpdateText(text);
    }
}