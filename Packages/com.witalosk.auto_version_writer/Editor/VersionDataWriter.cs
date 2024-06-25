using System;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace AutoVersionWriter
{
    /// <summary>
    /// Write version information at build time
    /// </summary>
    public class VersionDataWriter : IPreprocessBuildWithReport
    {
        public int callbackOrder => -1;
        
        public void OnPreprocessBuild(BuildReport report)
        {
            var data = AssetDatabase.LoadAssetAtPath<VersionData>(VersionData.VersionDataPath);
            if (data == null)
            {
                data = ScriptableObject.CreateInstance<VersionData>();
                AssetDatabase.CreateAsset(data, VersionData.VersionDataPath);
            }

            string branchName = ExecuteGitCommand("rev-parse --abbrev-ref HEAD");
            string hash = ExecuteGitCommand("rev-parse --short HEAD");
            string last10Commits = ExecuteGitCommand("log -10  --pretty=format:\"%s \nby %cn (%ci) %h\n\"");
            int commitNum = int.Parse(ExecuteGitCommand("rev-list --count HEAD"));

            data.Version = GetVersion(data.Version, commitNum);
            data.BuildDateTime = DateTime.Now.ToString(VersionData.DateTimeFormat);
            data.BranchName = branchName;
            data.CommitHash = hash;
            data.Last10Commits = last10Commits;

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }


        /// <summary>
        /// Major.Minor.Patch
        /// Major - After the 100th digit of the number of commits
        /// Minor - Number of commits in tens and ones
        /// Patch - Build Count
        /// </summary>
        private Vector3Int GetVersion(Vector3Int old, int currentCommitNum)
        {
            Vector3Int result = old;
            int oldCommitNum = old.x * 100 + old.y;
            if (oldCommitNum < currentCommitNum)
            {
                result.x = currentCommitNum / 100;
                result.y = (currentCommitNum - result.x * 100);
                result.z = 0;
            }
            else
            {
                result.z++;
            }

            return result;
        }

        private static string ExecuteGitCommand(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,

                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Application.dataPath
            };

            try
            {
                using var p = Process.Start(psi);
                string output = p.StandardOutput.ReadToEnd().Trim();
                p.WaitForExit(1000);
                return output;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }
    }
}