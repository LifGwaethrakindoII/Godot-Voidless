using Godot;
using System;

namespace Voidless
{
    public static class VVector3
    {
        public static Vector3 ClampedMagnitude(this Vector3 v, float m)
        {
            float mSqr = m * m;
            float vmSqr = v.LengthSquared();

            if(vmSqr > mSqr) v = ((v / Mathf.Sqrt(vmSqr)) * m);

            return v;
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return a + ((b - a) * t);
        }
    }
}