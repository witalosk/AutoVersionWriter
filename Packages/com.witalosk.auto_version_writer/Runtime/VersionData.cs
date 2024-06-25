using UnityEngine;

namespace AutoVersionWriter
{
    public class VersionData : ScriptableObject
    {
        public const string VersionDataPath = "Assets/Resources/VersionData.asset";
        public const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        [Header("Build Info")]
        public Vector3Int Version;
        public string BuildDateTime;
        
        [Header("Commit Info")]
        public string BranchName;
        public string CommitHash;
        [TextArea] public string Last10Commits;
    }
}