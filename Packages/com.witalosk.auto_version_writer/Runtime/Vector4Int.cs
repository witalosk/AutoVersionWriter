using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AutoVersionWriter
{
     /// <summary>
    /// Representation of 4D vectors and points using integers.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4Int : IEquatable<Vector4Int>, IFormattable
    {
        public int x { get => m_X; set => m_X = value; }
        public int y { get => m_Y; set => m_Y = value; }
        public int z { get => m_Z; set => m_Z = value; }
        public int w { get => m_W; set => m_W = value; }

        [SerializeField] private int m_X;
        [SerializeField] private int m_Y;
        [SerializeField] private int m_Z;
        [SerializeField] private int m_W;

        public Vector4Int(int x, int y, int z, int w)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = w;
        }

        // Set x, y and z components of an existing Vector.
        public void Set(int x, int y, int z, int w)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
            m_W = w;
        }

        // Access the /x/, /y/ or /z/ component using [0], [1] or [2] respectively.
        public int this[int index]
        {
            get => index switch
            {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {index}!")
            };

            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    case 3:
                        w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException($"Invalid Vector4Int index addressed: {index}!");
                }
            }
        }

        // Returns the length of this vector (RO).
        public float magnitude => Mathf.Sqrt((float)(x * x + y * y + z * z + w * w));

        // Returns the squared length of this vector (RO).
        public int sqrMagnitude => x * x + y * y + z * z + w * w;

        // Returns the distance between /a/ and /b/.
        public static float Distance(Vector4Int a, Vector4Int b) { return (a - b).magnitude; }

        // Returns a vector that is made from the smallest components of two vectors.
        public static Vector4Int Min(Vector4Int lhs, Vector4Int rhs) { return new Vector4Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z), Mathf.Min(lhs.w, rhs.w)); }

        // Returns a vector that is made from the largest components of two vectors.
        public static Vector4Int Max(Vector4Int lhs, Vector4Int rhs) { return new Vector4Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z), Mathf.Max(lhs.w, rhs.w)); }

        // Multiplies two vectors component-wise.
        public static Vector4Int Scale(Vector4Int a, Vector4Int b) { return new Vector4Int(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w); }

        // Multiplies every component of this vector by the same component of /scale/.
        public void Scale(Vector4Int scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        public void Clamp(Vector4Int min, Vector4Int max)
        {
            x = Math.Max(min.x, x);
            x = Math.Min(max.x, x);
            y = Math.Max(min.y, y);
            y = Math.Min(max.y, y);
            z = Math.Max(min.z, z);
            z = Math.Min(max.z, z);
            w = Math.Max(min.w, w);
            w = Math.Min(max.w, w);
        }

        // Converts a Vector4Int to a [[Vector4]].
        public static implicit operator Vector4(Vector4Int v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }

        // Converts a Vector4Int to a [[Vector2Int]].
        public static explicit operator Vector3Int(Vector4Int v)
        {
            return new Vector3Int(v.x, v.y, v.z);
        }

        // Converts a Vector4Int to a [[Vector2Int]].
        public static explicit operator Vector2Int(Vector4Int v)
        {
            return new Vector2Int(v.x, v.y);
        }

        public static Vector4Int FloorToInt(Vector4 v)
        {
            return new Vector4Int(
                Mathf.FloorToInt(v.x),
                Mathf.FloorToInt(v.y),
                Mathf.FloorToInt(v.z),
                Mathf.FloorToInt(v.w)
            );
        }

        public static Vector4Int CeilToInt(Vector4 v)
        {
            return new Vector4Int(
                Mathf.CeilToInt(v.x),
                Mathf.CeilToInt(v.y),
                Mathf.CeilToInt(v.z),
                Mathf.CeilToInt(v.w)
            );
        }

        public static Vector4Int RoundToInt(Vector4 v)
        {
            return new Vector4Int(
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z),
                Mathf.RoundToInt(v.w)
            );
        }

        public static Vector4Int operator+(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public static Vector4Int operator-(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
        }

        public static Vector4Int operator*(Vector4Int a, Vector4Int b)
        {
            return new Vector4Int(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public static Vector4Int operator-(Vector4Int a)
        {
            return new Vector4Int(-a.x, -a.y, -a.z, -a.w);
        }

        public static Vector4Int operator*(Vector4Int a, int b)
        {
            return new Vector4Int(a.x * b, a.y * b, a.z * b, a.w * b);
        }

        public static Vector4Int operator*(int a, Vector4Int b)
        {
            return new Vector4Int(a * b.x, a * b.y, a * b.z, a * b.w);
        }

        public static Vector4Int operator/(Vector4Int a, int b)
        {
            return new Vector4Int(a.x / b, a.y / b, a.z / b, a.w / b);
        }

        public static bool operator==(Vector4Int lhs, Vector4Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
        }

        public static bool operator!=(Vector4Int lhs, Vector4Int rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object other)
        {
            return other is Vector4Int vector4Int && Equals(vector4Int);
        }

        public bool Equals(Vector4Int other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            int yHash = y.GetHashCode();
            int zHash = z.GetHashCode();
            int wHash = w.GetHashCode();
            return x.GetHashCode() ^ (yHash << 8) ^ (yHash >> 24) ^ (zHash << 16) ^ (zHash >> 16) ^ (wHash << 24) ^ (wHash >> 8);
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format)
        {
            return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"({x.ToString(format, formatProvider)}, {y.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)}, {z.ToString(format, formatProvider)})";
        }

        public static Vector4Int zero => s_Zero;
        public static Vector4Int one => s_One;

        private static readonly Vector4Int s_Zero = new(0, 0, 0, 0);
        private static readonly Vector4Int s_One = new(1, 1, 1, 1);
    }

    #if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(Vector4Int))]
    public class Vector4IntDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            label = UnityEditor.EditorGUI.BeginProperty(position, label, property);
            position = UnityEditor.EditorGUI.PrefixLabel(position, label);
            position.height = UnityEditor.EditorGUIUtility.singleLineHeight;

            var x = property.FindPropertyRelative("m_X");
            var y = property.FindPropertyRelative("m_Y");
            var z = property.FindPropertyRelative("m_Z");
            var w = property.FindPropertyRelative("m_W");

            float spacing = 2f;
            float fieldWidth = (position.width - 3 * spacing) / 4;

            var xRect = new Rect(position.x, position.y, fieldWidth, position.height);
            var yRect = new Rect(position.x + fieldWidth + spacing, position.y, fieldWidth, position.height);
            var zRect = new Rect(position.x + 2 * (fieldWidth + spacing), position.y, fieldWidth, position.height);
            var wRect = new Rect(position.x + 3 * (fieldWidth + spacing), position.y, fieldWidth, position.height);

            UnityEditor.EditorGUI.PropertyField(xRect, x, GUIContent.none);
            UnityEditor.EditorGUI.PropertyField(yRect, y, GUIContent.none);
            UnityEditor.EditorGUI.PropertyField(zRect, z, GUIContent.none);
            UnityEditor.EditorGUI.PropertyField(wRect, w, GUIContent.none);

            UnityEditor.EditorGUI.EndProperty();
        }
    }
    #endif
}
