using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ExperimentLogger
{
    public static int runTime;
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

    public static string LogTrial(ExperimentTrial trial)
    {
        if (subjectId == "-1")
        {
            return "SKIPPING WRITING BECAUSE SUBJECTID = -1";
        }

        List<(string, string)> trialValues = new List<(string, string)>
        {
            ( "subject_id", $"sub{subjectId}"),
            ( "technique", $"{selectionTechnique}" ),
            ( "density", $"{densityLevel}" ),
            ( "trial_id", $"{trial.trialIdx}" ),
            ( "randObjIdx", $"{trial.randObjIdx}" ),
            ( "successful_selections", $"{trial.GenNumSuccessful()}" ),
            ( "total_attempts", $"{trial.GetNumAttempts()}" ),
            ( "trial_time", $"{trial.ComputeTrialTime()}" ),
        };

        var fname = $"sub{subjectId}_{runTime}.csv";
        fname = Path.Combine(logDir, fname);

        using StreamWriter writer = new StreamWriter(fname, true);
        if (!File.Exists(fname))
            writer.Write(string.Join(",", trialValues.Select(x => x.Item1)) + "\n");
        writer.Write(string.Join(",", trialValues.Select(x => x.Item2)) + "\n");

        return fname;
    }
}