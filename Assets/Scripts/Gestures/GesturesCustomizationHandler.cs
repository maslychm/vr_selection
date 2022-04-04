using Jackknife;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gestures
{
    public class GesturesCustomizationHandler : MonoBehaviour
    {
        [SerializeField] private InputActionReference performGestureInputAction;

        public List<Button> gestureButtons;
        public Button recordButton;
        public Button playButton;
        public Button retryButton;
        public Button deleteButton;
        public Button saveButton;

        public TMP_Text customizingText;
        public TMP_Text statusText;

        public Recognizer recognizer;
        public PerformingGestureAction gestureAction;

        private List<Vector> trajectoryToSave;

        private int selectedGestureId = -1;
        private string selectedGestureName = "None";
        private int _nextEx = 0;

        private enum RecordingState
        { Idle, Ready, Recording, CanSave };

        private RecordingState state = RecordingState.Idle;

        // Start is called before the first frame update
        private void Start()
        {
            ResetInitialUIState();
            //SetExternalListenersForGestureActions();
            SetGestureNamesAndCounts();
            state = RecordingState.Idle;
        }

        // Update is called once per frame
        private void Update()
        {
            ProcessInputs();
        }

        private void ProcessInputs()
        {
            if (performGestureInputAction.action.WasPerformedThisFrame())
            {
                TriggerDown();
            }

            if (performGestureInputAction.action.WasReleasedThisFrame())
            {
                TriggerUp();
            }
        }

        public void TriggerUp()
        {
            // If we were in record state, move to possibly saving
            if (state == RecordingState.Recording)
            {
                state = RecordingState.CanSave;
                SwitchToAfterRecordingUI();
                trajectoryToSave = gestureAction.GetPreviousGestureTrajectoryBuffer();
            }
        }

        public void TriggerDown()
        {
            if (state == RecordingState.CanSave) // Same as the retry button
            {
                SwitchToRecordingUI();
                state = RecordingState.Recording;
            }
        }

        #region UI

        private void ResetInitialUIState()
        {
            // Set initial texts, enable all gesture buttons, but disable entire customization menu
            InitializeTexts();
            SetAllCustomizationButtonsTo(false);
            SetAllGestureButtonsTo(true);
        }

        private void SwitchUIToSelectedGesture()
        {
            ResetInitialUIState();

            // Update the selected gesture text
            selectedGestureName = recognizer.GetGestureNameFromId(selectedGestureId);
            customizingText.text = $"Customizing:\n{selectedGestureName}";

            // Load all, and set the UI to display the count for the selected gestures
            List<Sample> samples = recognizer.LoadJKSamples();
            Dictionary<int, int> gestureCountDict = new Dictionary<int, int>();
            Dictionary<int, int> gestureMaxExVal = new Dictionary<int, int>();
            foreach (var s in samples)
            {
                // Initialize
                if (!gestureCountDict.ContainsKey(s.GestureId))
                {
                    gestureCountDict[s.GestureId] = 0;
                    gestureMaxExVal[s.GestureId] = s.ExampleId;
                }

                // Update cound and max seen example value (to save under a unique name later)
                gestureCountDict[s.GestureId]++;
                gestureMaxExVal[s.GestureId] = Math.Max(gestureMaxExVal[s.GestureId], s.ExampleId);
            }

            int selectedGestureCount = 0;
            if (gestureCountDict.ContainsKey(selectedGestureId))
                selectedGestureCount = gestureCountDict[selectedGestureId];

            statusText.text = $"We currently have {selectedGestureCount} of {selectedGestureName}";

            // Enable recording
            recordButton.gameObject.SetActive(true);

            // If at least one template, enable the play button
            if (selectedGestureCount > 0)
            {
                playButton.gameObject.SetActive(true);
                _nextEx = gestureMaxExVal[selectedGestureId] + 1;
            }
        }

        private void SetGestureNamesAndCounts()
        {
            List<Sample> samples = recognizer.LoadJKSamples();
            Dictionary<int, int> gestureCountDict = new Dictionary<int, int>();
            foreach (var s in samples)
            {
                if (!gestureCountDict.ContainsKey(s.GestureId))
                    gestureCountDict[s.GestureId] = 0;
                gestureCountDict[s.GestureId]++;
            }

            for (int i = 1; i <= 6; i++)
            {
                if (!gestureCountDict.ContainsKey(i))
                {
                    gestureCountDict.Add(i, 0);
                }
                //Debug.Log($"{i}");
                SetGestureTextAndCount(i, gestureCountDict[i]);
            }
        }

        private void SwitchToRecordingUI()
        {
            Debug.Log("Switched to recording UI");

            SetAllGestureButtonsTo(false);
            SetAllCustomizationButtonsTo(false);

            customizingText.text = $"Recording...\nHold Trigger while drawing, \nthen release to finish";
        }

        private void SwitchToAfterRecordingUI()
        {
            SetAllGestureButtonsTo(false);
            SetAllCustomizationButtonsTo(false);

            retryButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
            saveButton.gameObject.SetActive(true);

            Debug.Log("Switched to after recording UI");
        }

        private void SetAllCustomizationButtonsTo(bool val)
        {
            recordButton.gameObject.SetActive(val);
            playButton.gameObject.SetActive(val);
            retryButton.gameObject.SetActive(val);
            deleteButton.gameObject.SetActive(val);
            saveButton.gameObject.SetActive(val);
        }

        private void SetAllGestureButtonsTo(bool val)
        {
            gestureButtons.ForEach(b => b.gameObject.SetActive(val));
        }

        private void InitializeTexts()
        {
            customizingText.text = $"Select a Gesture on the left to customize...";
            statusText.text = $"Idle...";
        }

        private void SetGestureTextAndCount(int gid, int count)
        {
            //Debug.Log($"{recognizer.GetGestureNameFromId(gid)}");
            string gname = recognizer.GetGestureNameFromId(gid);
            TMP_Text tpmText = gestureButtons[gid - 1].gameObject.GetComponentInChildren<TMP_Text>();
            tpmText.text = $"({count}) {gname}";
        }

        #endregion UI

        #region Listeners

        public void OnGesture1Clicked()
        {
            //Debug.Log("Clicked on 1");
            selectedGestureId = 1;
            SwitchUIToSelectedGesture();
        }

        public void OnGesture2Clicked()
        {
            //Debug.Log("Clicked on 2");
            selectedGestureId = 2;
            SwitchUIToSelectedGesture();
        }

        public void OnGesture3Clicked()
        {
            //Debug.Log("Clicked on 3");
            selectedGestureId = 3;
            SwitchUIToSelectedGesture();
        }

        public void OnGesture4Clicked()
        {
            //Debug.Log("Clicked on 4");
            selectedGestureId = 4;
            SwitchUIToSelectedGesture();
        }

        public void OnGesture5Clicked()
        {
            //Debug.Log("Clicked on 5");
            selectedGestureId = 5;
            SwitchUIToSelectedGesture();
        }

        public void OnGesture6Clicked()
        {
            //Debug.Log("Clicked on 6");
            selectedGestureId = 6;
            SwitchUIToSelectedGesture();
        }

        public void OnRecordClicked()
        {
            SwitchToRecordingUI();
            state = RecordingState.Recording;
        }

        public void OnPlayClicked()
        {
            Debug.Log("Clicked PLAY");
        }

        public void OnSaveClicked()
        {
            var path = RecognizerUtils.BuildSamplePath(recognizer.gestureDirectoryPath, selectedGestureName, _nextEx);

            bool writeSuccess = RecognizerUtils.WriteTrajectoryToFile(trajectoryToSave, path);

            if (writeSuccess)
            {
                ResetInitialUIState();
                SetGestureNamesAndCounts();

                Debug.Log($"Saved {selectedGestureName} at {path}");
                statusText.text = $"Saved {selectedGestureName} at\n {path}";

                state = RecordingState.Idle;
            }
        }

        public void OnRetryClicked()
        {
            //RefreshVariablesBeforRecording();
            //state = State.Ready;
            //Debug.Log("Clicked RETRY");

            OnRecordClicked();
        }

        public void OnDeleteClicked()
        {
            //Debug.Log("Clicked DELETE");
            ResetInitialUIState();
            state = RecordingState.Idle;
        }

        #endregion Listeners
    }
}