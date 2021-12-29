using UnityEngine;

namespace TopDownCharacter
{
    public static class Extensions
    {
        /// <summary>
        /// Returns true if the Vector3 is not zero, adjusted for floating point error
        /// </summary>
        public static bool HasValue(this Vector3 vector)
        {
            return vector.sqrMagnitude > 0.001f;
        }

        /// <summary>
        /// Returns true if the Vector3 is zero, adjusted for floating point error
        /// </summary>
        public static bool IsBasicallyZero(this Vector3 vector)
        {
            return vector.sqrMagnitude < 0.001f;
        }
        
        /// <summary>
        /// Returns true if the Vector3 is not zero, adjusted for floating point error
        /// </summary>
        public static bool HasValue(this Vector2 vector)
        {
            return vector.sqrMagnitude > 0.001f;
        }

        /// <summary>
        /// Returns true if the Vector3 is zero, adjusted for floating point error
        /// </summary>
        public static bool IsBasicallyZero(this Vector2 vector)
        {
            return vector.sqrMagnitude < 0.001f;
        }

        /// <summary>
        /// Converts a Vector2 to Vector3 on a plane - translates X,Y to X,0,Y
        /// </summary>
        public static Vector3 ToVector3XZ(this Vector2 vector)
        {
            return new Vector3(vector.x, 0, vector.y);
        }
        
        /// <summary>
        /// Converts a Vector3 on the XZ plane to a Vector2 - translates X,Y,Z to X,Z
        /// </summary>
        public static Vector2 Flatten(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        /// <summary>
        /// Converts a Vector3 to a Vector2 on the XZ plane and preserves the magnitude of the vector
        /// </summary>
        public static Vector2 RotateOntoXZPlane(this Vector3 vector)
        {
            Vector3 direction = new Vector3(vector.x, 0, vector.z).normalized;
            return (direction * vector.magnitude).Flatten();
        }

        public static float Abs(this float f)
        {
            return Mathf.Abs(f);
        }
    }
}