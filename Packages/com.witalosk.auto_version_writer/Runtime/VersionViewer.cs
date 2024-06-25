using UnityEngine;
using UnityEngine.UI;

namespace AutoVersionWriter
{
    public class VersionViewer : MonoBehaviour
    {
        [SerializeField] protected Text _versionText;
        [SerializeField] protected Text _buildDateTimeText;

        private void Start()
        {
            _versionText.text = $"ver. {VersionGetter.Version}";
            _buildDateTimeText.text = $"({VersionGetter.BuildDateTime:yyyy/MM/dd HH:mm:ss})";
        }
    }
}