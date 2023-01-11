using UnityEngine;

namespace _Client_.Scripts.Runtime.Utils
{
    public static class GameExtension
    {
        public static bool IsInsideCircle(this Vector2 point, Vector2 center, float radius)
        {
            return Vector2.Distance(point, center) < radius;
        }

        public static bool IsInsideCircle(this Vector3 point, Vector2 center, float radius)
        {
            return Vector2.Distance(point, center) < radius;
        }
    }
}