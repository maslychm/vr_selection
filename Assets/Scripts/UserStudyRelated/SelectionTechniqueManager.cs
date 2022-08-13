using UnityEngine;

/// <summary>
/// assign the right selection technique in real time upon click of button
/// G -> Grid
/// LS -> LenSelect
/// FC -> Flower Cone
/// MM -> MiniMap
/// </summary>

public class SelectionTechniqueManager : MonoBehaviour
{
    public enum SelectionTechnique
    { SimpleMiniMap, OhMiniMap, ThreeDMiniMap, RayKebab }

    [SerializeField] private GameObject SimpleMiniMap_root, OhMiniMap_root, RayKebabGameObjectRoot, ThreeDMiniMapRoot;
    [SerializeField] private MiniMap SimpleMiniMap, OhMiniMap, ThreeDMiniMap;
    [SerializeField] private MiniMapInteractor SimpleMiniMapInteractor, OhMiniMapInteractor, ThreeDMiniMapInteractor;
    [SerializeField] private RayManager instanceOfRayManager;
    [SerializeField] private GrabbingHand grabbingHand;

    public static bool allowKeySelectionTechniqueSwitching = true;

    private void Start()
    {
        DisableAllTechniques();
    }

    private void Update()
    {
        if (!allowKeySelectionTechniqueSwitching)
            return;

        if (Input.GetKeyDown(KeyCode.W))
            ActivateSimpleMinimap();
        else if (Input.GetKeyDown(KeyCode.M))
            ActivateMinimapWithCircumference();
        else if (Input.GetKeyDown(KeyCode.F))
            ActivateFlowerCone();
        else if (Input.GetKeyDown(KeyCode.R))
            ActivateRayKebab();
        else if (Input.GetKeyDown(KeyCode.T))
            Activate3DMiniMap();
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            DisableAllTechniques();
            print("End of the experiment, thank you, back to starting position :D");
        }
    }

    public void DisableAllTechniques()
    {
        SimpleMiniMap_root.SetActive(false);
        OhMiniMap_root.SetActive(false);
        RayKebabGameObjectRoot.SetActive(false);
        ThreeDMiniMapRoot.SetActive(false);

        grabbingHand.miniMap = null;
        grabbingHand.miniMapIntreractor = null;
        grabbingHand.instanceOfRayManager = null;
        grabbingHand.circumferenceDisplayInUse = false;
        grabbingHand.ClearGrabbed();
    }

    public void ActivateTechnique(SelectionTechnique technique)
    {
        switch (technique)
        {
            case SelectionTechnique.SimpleMiniMap:
                ActivateSimpleMinimap();
                break;

            case SelectionTechnique.OhMiniMap:
                ActivateMinimapWithCircumference();
                break;

            case SelectionTechnique.ThreeDMiniMap:
                Activate3DMiniMap();
                break;

            case SelectionTechnique.RayKebab:
                ActivateRayKebab();
                break;
        }
    }

    private void ActivateFlowerCone()
    {
        DisableAllTechniques();
    }

    private void ActivateRayKebab()
    {
        DisableAllTechniques();
        print("Enabling the RayKebab Technique");

        grabbingHand.miniMap = null;
        grabbingHand.miniMapIntreractor = null;
        grabbingHand.circumferenceDisplayInUse = false;
        grabbingHand.instanceOfRayManager = instanceOfRayManager;
        RayKebabGameObjectRoot.SetActive(true);
    }

    private void ActivateMinimapWithCircumference()
    {
        DisableAllTechniques();
        print("Enabling OH Mini Map");

        grabbingHand.miniMap = OhMiniMap;
        grabbingHand.miniMapIntreractor = OhMiniMapInteractor;
        grabbingHand.circumferenceDisplayInUse = true;
        grabbingHand.instanceOfRayManager = null;

        OhMiniMap_root.SetActive(true);
    }

    private void Activate3DMiniMap()
    {
        DisableAllTechniques();
        print("Enabling 3D Mini Map");

        grabbingHand.miniMap = ThreeDMiniMap;
        grabbingHand.miniMapIntreractor = ThreeDMiniMapInteractor;
        grabbingHand.circumferenceDisplayInUse = false;
        grabbingHand.instanceOfRayManager = null;

        ThreeDMiniMapRoot.SetActive(true);
    }

    private void ActivateSimpleMinimap()
    {
        DisableAllTechniques();
        print("Enabling Simple Mini Map");

        grabbingHand.miniMap = SimpleMiniMap;
        grabbingHand.miniMapIntreractor = SimpleMiniMapInteractor;
        grabbingHand.instanceOfRayManager = null;

        SimpleMiniMap_root.SetActive(true);
    }
}