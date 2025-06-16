using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace AutoVersionWriter
{
    /// <summary>
    /// Write version information at build time
    /// </summary>
#if UNITY_EDITOR
    public class VersionDataWriter : IPreprocessBuildWithReport
#else
    public class VersionDataWriter
#endif
    {
#if UNITY_EDITOR
        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            var data = AssetDatabase.LoadAssetAtPath<VersionData>(VersionData.VersionDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<VersionData>();
                AssetDatabase.CreateAsset(data, VersionData.VersionDataPath);
            }

            string branchName = ExecutePowerShellCommand("git rev-parse --abbrev-ref HEAD");
            string hash = ExecutePowerShellCommand("git rev-parse --short HEAD");
            string last10Commits = ExecutePowerShellCommand("git log -10  --pretty=format:'%s \nby %cn (%ci) %h\n'");
            last10Commits = Encoding.UTF8.GetString(Encoding.GetEncoding("shift_jis").GetBytes(last10Commits));


            data.Version = GetVersion(data.Version, data.MajorTag);
            data.BuildDateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            data.BranchName = branchName;
            data.CommitHash = hash;
            data.Last10Commits = last10Commits;

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
#endif
        public static Vector4Int GetVersion(Vector4Int old, string majorTag)
        {
            Vector4Int result = Vector4Int.zero;

            // major
            result.x = int.Parse(ExecutePowerShellCommand($"git log --format=%s | Select-String '{majorTag}' | Measure-Object -Line | Select-Object -ExpandProperty Lines"));
            string lastMajorHash = ExecutePowerShellCommand($"git log --grep='{majorTag}' --format='%H' -n 1").Trim();

            // minor
            result.y = !string.IsNullOrEmpty(lastMajorHash)
                ? int.Parse(ExecutePowerShellCommand(@"git log " + lastMajorHash + @"..HEAD --grep='Merge pull request' --oneline | Measure-Object -Line | Select-Object -ExpandProperty Lines"))
                : int.Parse(ExecutePowerShellCommand(@"git log --grep='Merge pull request' --oneline | Measure-Object -Line | Select-Object -ExpandProperty Lines"));
            string lastMinorHash = result.y == 0 ? lastMajorHash : ExecutePowerShellCommand(@"git log --grep='Merge pull request' --format='%H' -n 1").Trim();

            // patch
            result.z = !string.IsNullOrEmpty(lastMinorHash)
                ? int.Parse(ExecutePowerShellCommand($"git rev-list --count {lastMinorHash}..HEAD"))
                : int.Parse(ExecutePowerShellCommand($"git rev-list --count HEAD"));

            if (old.x == result.x && old.y == result.y && old.z == result.z)
            {
                result.w = old.w + 1;
            }
            else
            {
                result.w = 0;
            }

            return result;
        }

        private static string ExecutePowerShellCommand(string command)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"{command}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Application.dataPath,
            };

            using Process process = new Process();
            process.StartInfo = psi;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
    }
}