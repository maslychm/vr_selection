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
    { SimpleMiniMap, OhMiniMap, ThreeDMiniMap, RayKebab, Flower, GravityZone }

    [SerializeField] private GameObject SimpleMiniMap_root, OhMiniMap_root, RayKebabGameObjectRoot, ThreeDMiniMapRoot, FlowerRoot, GravityZoneRoot;
    [SerializeField] private MiniMap SimpleMiniMap, OhMiniMap, ThreeDMiniMap;
    [SerializeField] private MiniMapInteractor SimpleMiniMapInteractor, OhMiniMapInteractor, ThreeDMiniMapInteractor;
    [SerializeField] private RayKebabManager instanceOfRayManager;
    [SerializeField] private GrabbingHand grabbingHand;
    [SerializeField] private PassInteractablesToGrid flowerCone;

    public static bool allowKeySelectionTechniqueSwitching = true;

    public static bool isRayKebab = false;

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
        else if (Input.GetKeyDown(KeyCode.G))
            ActivateGravityZone();
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
        FlowerRoot.SetActive(false);
        GravityZoneRoot.SetActive(false);
        isRayKebab = false;
        grabbingHand.miniMap = null;
        grabbingHand.miniMapIntreractor = null;
        grabbingHand.instanceOfRayManager = null;
        grabbingHand.flowerCone = null;
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

            case SelectionTechnique.Flower:
                ActivateFlowerCone();
                break;

            case SelectionTechnique.GravityZone:
                ActivateGravityZone();
                break;
        }
    }

    private void ActivateFlowerCone()
    {
        DisableAllTechniques();
        print("Enabling Flower Cone");
        grabbingHand.miniMap = null;
        grabbingHand.miniMapIntreractor = null;
        grabbingHand.circumferenceDisplayInUse = false;
        grabbingHand.instanceOfRayManager = null;
        grabbingHand.flowerCone = flowerCone;
        FlowerRoot.SetActive(true);
    }

    private void ActivateRayKebab()
    {
        DisableAllTechniques();
        print("Enabling the RayKebab Technique");
        isRayKebab = true;
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

    private void ActivateGravityZone()
    {
        DisableAllTechniques();
        print("Enabling Gravity Zone");
        GravityZoneRoot.SetActive(true);
    }

    public void clearCurrentTechnique(SelectionTechnique currentLevelTechnique)
    {
        switch (currentLevelTechnique)
        {
            case SelectionTechnique.SimpleMiniMap:
                SimpleMiniMap.ClearObjectCopies();
                SimpleMiniMapInteractor.FlushDuplicateDirections();
                break;

            case SelectionTechnique.OhMiniMap:
                OhMiniMap.ClearObjectCopies();
                OhMiniMapInteractor.FlushDuplicateDirections();
                FindObjectOfType<ClutterHandler_circumferenceDisplay>().FreeCircularSlots();
                break;

            case SelectionTechnique.ThreeDMiniMap:
                ThreeDMiniMapInteractor.FlushDuplicateDirections();
                ThreeDMiniMap.ClearObjectCopies();
                break;

            case SelectionTechnique.RayKebab:
                instanceOfRayManager.ReleaseInteractablesFromRay();
                break;

            case SelectionTechnique.Flower:
                flowerCone.AtTrialStart();
                break;
        }
    }
}