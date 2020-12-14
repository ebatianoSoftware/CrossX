using System;
using System.Numerics;

namespace CrossX
{
    public static class Extensions
    {
        public static Vector2 Normalized(this Vector2 vec) => Vector2.Normalize(vec);
        public static Vector3 Normalized(this Vector3 vec) => Vector3.Normalize(vec);
        public static Vector4 Normalized(this Vector4 vec) => Vector4.Normalize(vec);
        public static Matrix4x4 Inverse(this Matrix4x4 mat)
        {
            if (!Matrix4x4.Invert(mat, out var inv)) return Matrix4x4.Identity;
            return inv;
        }

        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }

        public static Vector2 RotateAround(this Vector2 vector, Vector2 origin, float angle)
        {
            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            vector = vector - origin;

            return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos) + origin;
        }
    }
}
