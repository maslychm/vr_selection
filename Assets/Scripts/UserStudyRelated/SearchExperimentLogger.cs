using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SearchExperimentLogger
{
    public static int softwareStartTime;
    public static string subjectId;
    public static int densityLevel;
    public static SelectionTechniqueManager.SelectionTechnique selectionTechnique;
    private static string logDir;

    public static void CreateLoggingDirectory(string dir, string subdir)
    {
        var path = dir;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        path = Path.Combine(dir, subdir);
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        logDir = path;
    }

    public static string LogTrial(SearchExperimentTrial trial)
    {
        if (subjectId == "-1")
        {
            return "SKIPPING WRITING BECAUSE SUBJECTID = 1";
        }

        List<(string, string)> trialValues = new List<(string, string)>
        {
            ( "subject_id", $"sub_{subjectId}"),
            ( "technique", $"{selectionTechnique}" ),
            ( "density", $"{densityLevel}" ),
            
            ( "trial_type", $"{trial.type}" ),
            ( "trial_id", $"{trial.trialIdx}" ),
            ( "rand_obj_idx", $"{trial.randObjIdx}" ),
            ( "distance_to_obj", $"{trial.distToTarget}" ),
            ( "num_attempts", $"{trial.GetNumAttempts()}" ),
            ( "trial_time", $"{trial.ComputeTrialTime()}" ),
            
            ( "left_hand_distance_travelled", $"{HandDistancesTraveled.GetRightHandPathLength()}" ),
            ( "right_hand_distance_travelled", $"{HandDistancesTraveled.GetLeftHandPathLength()}" ),
            ( "left_trigger_clicked", $"{BothHandsButtonClicksCountTracker.leftTriggerButton_Count}" ),
            ( "right_trigger_clicked", $"{BothHandsButtonClicksCountTracker.rightTriggerButton_Count}" ),
            ( "left_grip_clicked", $"{BothHandsButtonClicksCountTracker.leftGripButton_Count}" ),
            ( "right_grip_clicked", $"{BothHandsButtonClicksCountTracker.rightGripButton_Count}" ),
        };

        var fname = $"sr_sub{subjectId}_{softwareStartTime}.csv";
        fname = Path.Combine(logDir, fname);

        var writeHeader = !File.Exists(fname);

        using StreamWriter writer = new StreamWriter(fname, true);
        if (writeHeader)
            writer.Write(string.Join(",", trialValues.Select(x => x.Item1)) + "\n");
        writer.Write(string.Join(",", trialValues.Select(x => x.Item2)) + "\n");

        return fname;
    }
}
