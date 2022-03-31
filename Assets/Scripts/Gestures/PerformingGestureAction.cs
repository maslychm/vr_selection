using Jackknife;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gestures
{
    public class PerformingGestureAction : MonoBehaviour
    {
        // TODO CONTINUE HERE => ADD TRACKING OF THIS HAPPENING
        //if (flyInputAction_Right.action.IsPressed())
        //{
        //    AddFlyForceIfNotTooHigh();
        //}
        [SerializeField] private InputActionReference performGestureInputAction;

        [Header("Tracking-Related Settings")]
        public Transform leftHand;

        //[Tooltip("Assing the Right Hand under Player/SteamVRObjects")]
        public Transform rightHand;

        //[Tooltip("Select Performing Gesture action from SteamVR action set - Listeners for Presses (Up and Down) are attached to this")]
        //public SteamVR_Action_Boolean performingGestureActionBoolean;

        //[Tooltip("Which input source? (Usually Any or right or left)")]
        //public SteamVR_Input_Sources handType;

        [Header("Recognizer Settings")]
        [Tooltip("The Recognizer instance")]
        public Recognizer recognizer;

        //public bool usePosition = true;
        //public bool useRotation = true;

        [Tooltip("Rotate the recorded trajectory with HMD rotation at first recorded frame. This allows to face any direction in the world while inputing gestures.")]
        public bool rotateWithLook = true;

        [Tooltip("From Child: Left Line")]
        public LineRenderer leftHandInputLineRenderer;

        [Tooltip("From Child: Right Line")]
        public LineRenderer rightHandInputLineRenderer;

        [Header("Line Settings")]
        [Range(0.01f, 0.2f)]
        public float trajectoryLineWidth = .03f;

        private bool performingNow = false;
        private int lastTrajectoryFrameIdx;

        private List<Vector> previousGestureTrajectoryBuffer;
        private List<Vector> currentGestureTrajectoryBuffer; // Format that recognizer accepts

        private Transform playerHmdTransformAtGestureBegin;

        private List<Vector3> leftHandTrajectory;
        private List<Vector3> rightHandTrajectory;
        //private List<Quaternion> leftHandRotations;
        //private List<Quaternion> rightHandRotations;

        private void Start()
        {
            //performingGestureActionBoolean.AddOnStateDownListener(TriggerDown, handType);
            //performingGestureActionBoolean.AddOnStateUpListener(TriggerUp, handType);
            //Events.free_hands.AddListener(FreeBothHands);

            ResetGestureTrajectoryBufferValues();

            InitLineRenderers();
        }

        private void InitLineRenderers()
        {
            lastTrajectoryFrameIdx = 0;

            leftHandTrajectory = new List<Vector3>();
            rightHandTrajectory = new List<Vector3>();

            //leftHandRotations = new List<Quaternion>();
            //rightHandRotations = new List<Quaternion>();

            leftHandInputLineRenderer.startWidth = trajectoryLineWidth;
            rightHandInputLineRenderer.startWidth = trajectoryLineWidth;

            leftHandInputLineRenderer.endWidth = trajectoryLineWidth;
            rightHandInputLineRenderer.endWidth = trajectoryLineWidth;
        }

        // SteamVR sets this to ~71 per second, which is ~0.011
        public void FixedUpdate()
        {
            if (performingNow)
            {
                UpdateControllerTrails();
                UpdateGestureTrajectoryBuffer();
                //FreeHand(leftHand);
                //FreeHand(rightHand);
            }
        }

        //public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    //Debug.Log("Finished Performing");
        //    performingNow = false;
        //    PassGestureTrajectoryBufferToRecognizer();
        //    ResetGestureTrajectoryBufferValues();
        //}

        //public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        //{
        //    //Debug.Log("Started Performing");
        //    ClearHandTrajectories();
        //    performingNow = true;
        //    playerHmdTransformAtGestureBegin = Valve.VR.InteractionSystem.Player.instance.hmdTransform;
        //    //FreeHand(leftHand);
        //    //FreeHand(rightHand);
        //}

        private void FreeBothHands()
        {
            //FreeHand(leftHand);
            //FreeHand(rightHand);
        }

        //private void FreeHand(Hand hand)
        //{
        //    hand.DetachObject(hand.currentAttachedObject);
        //}

        private void ClearHandTrajectories()
        {
            lastTrajectoryFrameIdx = 0;

            leftHandInputLineRenderer.positionCount = 0;
            rightHandInputLineRenderer.positionCount = 0;

            leftHandTrajectory.Clear();
            rightHandTrajectory.Clear();
        }

        private void PassGestureTrajectoryBufferToRecognizer()
        {
            recognizer.ProcessGestureBuffer(currentGestureTrajectoryBuffer);
        }

        private void ResetGestureTrajectoryBufferValues()
        {
            previousGestureTrajectoryBuffer = currentGestureTrajectoryBuffer;
            currentGestureTrajectoryBuffer = new List<Vector>();
            performingNow = false;
        }

        private void UpdateGestureTrajectoryBuffer()
        {
            if (rotateWithLook)
            {
                currentGestureTrajectoryBuffer.Add(GetCurrentControllersPositionAsJKVectorInHMDCoords());
            }
            else
            {
                currentGestureTrajectoryBuffer.Add(GetCurrentControllersPositionAsJKVector());
            }
        }

        private void UpdateControllerTrails()
        {
            if (leftHand == null || rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            // Get the current controller positions
            var leftHandPosition = leftHand.transform.position;
            var rightHandPosition = rightHand.transform.position;

            // Call this on the hand positions w.r.t. hmd
            //rightHand.transform.InverseTransformDirection()

            // Save them to a list
            leftHandTrajectory.Add(leftHandPosition);
            rightHandTrajectory.Add(rightHandPosition);

            leftHandInputLineRenderer.positionCount++;
            leftHandInputLineRenderer.SetPosition(lastTrajectoryFrameIdx, leftHandPosition);

            rightHandInputLineRenderer.positionCount++;
            rightHandInputLineRenderer.SetPosition(lastTrajectoryFrameIdx, rightHandPosition);

            lastTrajectoryFrameIdx++;
        }

        private Vector GetCurrentControllersPositionAsJKVector()
        {
            if (leftHand == null || rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            var leftHandPosition = leftHand.transform.position;
            var rightHandPosition = rightHand.transform.position;

            //var leftHandRotation = leftHand.transform.rotation;
            //var rightHandRotation = rightHand.transform.rotation;

            return new Vector(new List<double>
            {
                leftHandPosition.x,
                leftHandPosition.y,
                leftHandPosition.z,

                //leftHandRotation.x,
                //leftHandRotation.y,
                //leftHandRotation.z,
                //leftHandRotation.w,

                rightHandPosition.x,
                rightHandPosition.y,
                rightHandPosition.z,

                //rightHandRotation.x,
                //rightHandRotation.y,
                //rightHandRotation.z,
                //rightHandRotation.w,
            });
        }

        private Vector GetCurrentControllersPositionAsJKVectorInHMDCoords()
        {
            Vector3 leftInHMD = playerHmdTransformAtGestureBegin.InverseTransformVector(leftHandTrajectory[lastTrajectoryFrameIdx - 1]);
            Vector3 rightInHMD = playerHmdTransformAtGestureBegin.InverseTransformVector(rightHandTrajectory[lastTrajectoryFrameIdx - 1]);

            if (leftHand == null || rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            return new Vector(new List<double>
            {
                leftInHMD.x,
                leftInHMD.y,
                leftInHMD.z,

                rightInHMD.x,
                rightInHMD.y,
                rightInHMD.z,
            });
        }

        public List<Vector> GetPreviousGestureTrajectoryBuffer()
        {
            return previousGestureTrajectoryBuffer;
        }
    }
}