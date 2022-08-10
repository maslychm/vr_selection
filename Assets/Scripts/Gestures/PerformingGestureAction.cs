using Jackknife;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gestures
{
    public class PerformingGestureAction : MonoBehaviour
    {
        [SerializeField] private InputActionReference performGestureInputAction;
        [SerializeField] private Transform rotationInvarianceTransform;

        [Tooltip("Right hand controller from hierarcy")]
        public Transform rightHand;

        [Header("Recognizer Settings")]
        [Tooltip("The Recognizer instance")]
        public Recognizer recognizer;

        //public bool usePosition = true;
        //public bool useRotation = true;

        [Tooltip("Rotate the recorded trajectory with HMD rotation at first recorded frame. This allows to face any direction in the world while inputing gestures.")]
        public bool rotateWithLook = true;

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

        private List<Vector3> rightHandTrajectory;
        //private List<Quaternion> rightHandRotations;

        private void Start()
        {
            //Events.free_hands.AddListener(FreeBothHands);

            ResetGestureTrajectoryBufferValues();

            InitLineRenderers();

            playerHmdTransformAtGestureBegin = rotationInvarianceTransform;
        }

        public void FixedUpdate()
        {
            if (performingNow)
            {
                UpdateControllerTrails();
                UpdateGestureTrajectoryBuffer();
            }
        }

        public void Update()
        {
            ProcessInput();
        }

        private void ProcessInput()
        {
            if (performGestureInputAction.action.WasPressedThisFrame())
            {
                GestureActionDown();
            }

            if (performGestureInputAction.action.WasReleasedThisFrame())
            {
                GestureActionUp();
            }
        }

        private void InitLineRenderers()
        {
            lastTrajectoryFrameIdx = 0;

            rightHandTrajectory = new List<Vector3>();

            //rightHandRotations = new List<Quaternion>();

            rightHandInputLineRenderer.startWidth = trajectoryLineWidth;

            rightHandInputLineRenderer.endWidth = trajectoryLineWidth;
        }

        // SteamVR sets this to ~71 per second, which is ~0.011

        public void GestureActionDown()
        {
            ClearHandTrajectories();
            performingNow = true;

            playerHmdTransformAtGestureBegin = rotationInvarianceTransform;
        }

        public void GestureActionUp()
        {
            performingNow = false;
            PassGestureTrajectoryBufferToRecognizer();
            ResetGestureTrajectoryBufferValues();
        }

        private void FreeBothHands()
        {
            //FreeHand(rightHand);
        }

        //private void FreeHand(Hand hand)
        //{
        //    hand.DetachObject(hand.currentAttachedObject);
        //}

        private void ClearHandTrajectories()
        {
            lastTrajectoryFrameIdx = 0;

            rightHandInputLineRenderer.positionCount = 0;

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
            if (rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            // Get the current controller positions
            var rightHandPosition = rightHand.transform.position;

            // Call this on the hand positions w.r.t. hmd
            //rightHand.transform.InverseTransformDirection()

            // Save them to a list
            rightHandTrajectory.Add(rightHandPosition);

            rightHandInputLineRenderer.positionCount++;
            rightHandInputLineRenderer.SetPosition(lastTrajectoryFrameIdx, rightHandPosition);

            lastTrajectoryFrameIdx++;
        }

        private Vector GetCurrentControllersPositionAsJKVector()
        {
            if (rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            var rightHandPosition = rightHand.transform.position;

            //var leftHandRotation = leftHand.transform.rotation;
            //var rightHandRotation = rightHand.transform.rotation;

            return new Vector(new List<double>
            {
                //leftHandPosition.x,
                //leftHandPosition.y,
                //leftHandPosition.z,

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
            //Vector3 leftInHMD = playerHmdTransformAtGestureBegin.InverseTransformVector(leftHandTrajectory[lastTrajectoryFrameIdx - 1]);
            Vector3 rightInHMD = playerHmdTransformAtGestureBegin.InverseTransformPoint(rightHandTrajectory[lastTrajectoryFrameIdx - 1]);

            if (rightHand == null)
            {
                Debug.Log("Hands missing in the Recognizer Script!");
            }

            return new Vector(new List<double>
            {
                //leftInHMD.x,
                //leftInHMD.y,
                //leftInHMD.z,

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