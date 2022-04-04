using Jackknife;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Gestures
{
    public class Recognizer : MonoBehaviour
    {   // TODO make the classification async
        [Tooltip("Don't touch, unless want to use a different set (requires modifying the Dictionary)")]
        public string gestureDirectoryPath;

        [Tooltip("Disable action execution (for customization mode)")]
        public bool disableActionExecution = true;

        [Tooltip("Where do we print the classification result")]
        public TMP_Text debugText;

        private Jackknife.Jackknife jackknife;

        private readonly Dictionary<string, int> GestureNameToGestureId = new Dictionary<string, int>()
        {
            // Must use dashes because of file name parsing
            {"sphere", 1 },
            {"cylinder", 2 },
            {"cube", 3 },
            {"star", 4 },
            {"pyramid", 5 },
            {"infinity", 6 }
        };

        private Dictionary<int, string> GestureIdToGestureName;

        private void Start()
        {
            gestureDirectoryPath = Application.dataPath + "/Dataset/";
            Debug.Log("Gesture Dir:" + gestureDirectoryPath);

            CreateGestureIdToNameMapping();
            CreateGestureRecognizer();
            SummarizeTraining();
            TestClassification();

            SetActionCalls();
        }

        // Gets automatically called when the person releases the gesture performance button
        public void ProcessGestureBuffer(List<Vector> gestureBufferTrajectory)
        {
            // If either not enough points, or path traveled is less than half a me
            if (gestureBufferTrajectory.Count < 12 || Jackknife.Mathematics.PathLength(gestureBufferTrajectory) < .3)
            {
                return;
            }

            int classifiedGestureId = ClassifyTrajectory(gestureBufferTrajectory);
            string classifiedGestureName = GetGestureNameFromId(classifiedGestureId);

            Debug.Log($"Classified as {classifiedGestureName}");

            if (classifiedGestureId == -1)
            {
                return;
            }

            CallGestureAction(classifiedGestureId);
        }

        private void SetActionCalls()
        {
            if (disableActionExecution)
            {
                return;
            }
        }

        private void CallGestureAction(int gestureId)
        {
            if (disableActionExecution)
            {
                return;
            }

            switch (gestureId)
            {
                case 1:
                    break;

                case 2:
                    // Slow down time (or time warp, whichever)
                    break;

                case 3:
                    // Two hands up down - energy boost
                    //Events.intialize_boost_event.Invoke();
                    break;

                case 4:
                    // Two hands forward
                    //Events.spawn_lightning.Invoke();
                    break;

                case 5:
                    // heal - two hands up and forward, or cross
                    //Events.heal_event.Invoke();
                    break;

                case 6:
                    break;

                default:
                    throw new System.Exception("Call Gesture Action went wrong. Possible missclasification");
            }
        }

        private int ClassifyTrajectory(List<Vector> trajectory)
        {
            int ret = jackknife.Classify(trajectory);

            if (debugText != null)
            {
                debugText.text = $"Classified: {GetGestureNameFromId(ret)}";
            }

            return ret;
        }

        private void TestClassification()
        {
            double result = jackknife.TestClassificationGetAccuracy();
            Debug.Log($"Internal Classification Accuracy: {result}");
        }

        public void CreateGestureRecognizer()
        {
            JackknifeBlades blades = new JackknifeBlades();
            blades.SetIPDefaults();
            blades.ResampleCnt = 20;

            jackknife = new Jackknife.Jackknife(blades);

            foreach (Sample sample in LoadJKSamples())
                jackknife.AddTemplate(sample);
        }

        public List<Sample> LoadJKSamples()
        {
            return RecognizerUtils.LoadSamples(gestureDirectoryPath, GestureNameToGestureId);
        }

        public void SummarizeTraining()
        {
            //jackknife.Train(32, 3, 1.0);
            var summary = jackknife.Summarize(GestureIdToGestureName);
            Debug.Log(summary);
        }

        private void CreateGestureIdToNameMapping()
        {
            GestureIdToGestureName = GestureNameToGestureId.ToDictionary(x => x.Value, x => x.Key);
        }

        public int GetGestureIdFromName(string gname)
        {
            if (GestureNameToGestureId.ContainsKey(gname))
                return GestureNameToGestureId[gname];
            return -1;
        }

        public string GetGestureNameFromId(int gid)
        {
            if (GestureIdToGestureName.ContainsKey(gid))
                return GestureIdToGestureName[gid];
            return "Unknown";
        }

        public List<string> GetGestureNames()
        {
            return GestureNameToGestureId.Keys.ToList();
        }
    }
}