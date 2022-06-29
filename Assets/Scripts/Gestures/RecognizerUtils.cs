using Jackknife;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Gestures
{
    public class RecognizerUtils
    {
        /*
         * Loads trajectories of valid gestures at directory.
         * Returns a list of Jackknife.Sample objects
         * Gesture filename format: gt_{GestureName}_{ExampleID}.gesture
         */

        private const int NUM_COMPONENTS = 3;

        public static List<Sample> LoadSamples(string directory, Dictionary<string, int> GestureNameToID)
        {
            if (!Directory.Exists(directory))
                throw new System.Exception($"Training gesture dir. {directory} does not exist!");

            var samplePaths = new List<(string, string, int, int)>(); // Path, Gesture Name, Gesture ID, Example ID

            foreach (var file in Directory.GetFiles(directory))
            {
                var fname = Path.GetFileName(file);

                if (fname.Contains(".meta") ||
                    !fname.StartsWith("gt_") ||
                    !fname.EndsWith(".gesture"))
                    continue;

                fname = Path.GetFileNameWithoutExtension(file);
                string[] pieces = fname.Split('_');

                if (pieces.Length != 3)
                    throw new System.Exception($"Incomplete gesture filename in {directory}");

                var gname = pieces[1];
                var exampleID = int.Parse(pieces[2]);

                if (!GestureNameToID.ContainsKey(gname))
                    throw new System.Exception($"Unknown Gesture Name in {directory}");

                var gestureID = GestureNameToID[gname];

                samplePaths.Add((file, gname, gestureID, exampleID));
            }

            List<Sample> samples = new List<Sample>();

            foreach ((var fpath, var gname, var gid, var ex) in samplePaths)
            {
                Sample sample = new Sample(0, gid, ex);
                List<Vector> trajectory = LoadTrajectoryFromFile(fpath);
                sample.AddTrajectory(trajectory);

                samples.Add(sample);
            }

            return samples;
        }

        /*
         * Load trajectory of the controlers from fpath
         *
         * Format: n-pts = int, m-dimensions = int; pt{x} = 6-tuple of doubles;
         * ------------------------
         * n_pts
         * pt1
         * ..
         * pt_n
         * ------------------------
         *
         * Format ex:
         * ------------------------
         * 16 6
         * 0 0 1 1 1 1
         * ..
         * 8 1 3 3 1 2
         * ------------------------
         */

        // THE ABOVE WAS CHANGED TO ONLY THE RIGHT HAND

        public static List<Vector> LoadTrajectoryFromFile(string fpath)
        {
            List<Vector> trajectory = new List<Vector>();

            var lines = File.ReadAllLines(fpath);

            // Read the number of timestamps that will follow
            var numTimestamps = int.Parse(lines[0].Split(' ')[0]);

            for (var i = 1; i <= numTimestamps; i += 1)
            {
                // Read each timestamp
                string[] pieces = lines[i].Split(' ');

                if (pieces.Length != NUM_COMPONENTS)
                    throw new System.Exception($"Incorrect trajectory in {fpath}");

                Vector v = new Vector(NUM_COMPONENTS);
                for (int j = 0; j < NUM_COMPONENTS; j++)
                {
                    v[j] = double.Parse(pieces[j]);
                }
                trajectory.Add(v);
            }
            return trajectory;
        }

        public static string BuildSamplePath(string root, string gname, int gex)
        {
            // "gt_{ GestureName}_{ ExampleID}.gesture"
            return Path.Combine(root, $"gt_{gname}_{gex}.gesture");
        }

        public static bool WriteTrajectoryToFile(List<Vector> trajectory, string path)
        {
            using (StreamWriter outputFile = new StreamWriter(path))
            {
                outputFile.WriteLine($"{trajectory.Count} {trajectory[0].Size}");

                int local_NUM_COMPONENTS = NUM_COMPONENTS;

                if (local_NUM_COMPONENTS == 6)
                {
                    foreach (var vec in trajectory)
                        outputFile.WriteLine($"{vec[0]} {vec[1]} {vec[2]} {vec[3]} {vec[4]} {vec[5]}");
                }
                else if (local_NUM_COMPONENTS == 3)
                {
                    foreach (var vec in trajectory)
                        outputFile.WriteLine($"{vec[0]} {vec[1]} {vec[2]}");
                }
            }

            return true;
        }

        public static RecognitionResult constructRecognitionResult(
            int gestureId,
            string gestureName,
            Vector startPtJKVEC,
            Vector endPtJKVEC)
        {
            return new RecognitionResult(
                gestureId,
                gestureName,
                JKVector3toUnityVector3(startPtJKVEC),
                JKVector3toUnityVector3(endPtJKVEC));
        }

        public static Vector3 JKVector3toUnityVector3(Vector jkVector)
        {
            return new Vector3((float)jkVector[0], (float)jkVector[1], (float)jkVector[2]);
        }
    }
}