using System;
using System.Globalization;
using UnityEngine;

namespace AutoVersionWriter
{
    /// <summary>
    /// VersionGetter
    /// </summary>
    public static class VersionGetter
    {
        private static VersionData _versionData;

        public static string Version
        {
            get
            {
                bool isLoaded = TryLoadVersionData();
#if UNITY_EDITOR
                var data = VersionDataWriter.GetVersion(Vector4Int.zero, isLoaded ? _versionData.MajorTag : "major");
                return $"{data.x}.{data.y}.{data.z}_{data.w}";
#else

                if (isLoaded)
                {
                    return $"{_versionData.Version.x}.{_versionData.Version.y}.{_versionData.Version.z}_{_versionData.Version.w}";;
                }

                return "0.0.0_0";
#endif
            }
        }

        public static DateTime BuildDateTime
        {
            get
            {
                if (TryLoadVersionData())
                {
                    if (DateTime.TryParseExact(_versionData.BuildDateTime, VersionData.DateTimeFormat, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
                    {
                        return dateTime;
                    }

                    return DateTime.MinValue;
                }

                return DateTime.MinValue;
            }
        }

        public static VersionData RawData
        {
            get
            {
                if (TryLoadVersionData())
                {
                    return _versionData;
                }

                return null;
            }
        }

        private static bool TryLoadVersionData()
        {
            if (_versionData != null) return true;
            _versionData = Resources.Load<VersionData>("VersionData");
            return _versionData != null;
        }

    }
}