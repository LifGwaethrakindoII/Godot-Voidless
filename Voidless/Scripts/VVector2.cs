using Godot;
using System;

namespace Voidless
{
    public static class VVector2
    {
        public static Vector2 ClampedMagnitude(this Vector2 v, float m)
        {
            float mSqr = m * m;
            float vmSqr = v.LengthSquared();

            if(vmSqr > mSqr) v = ((v / Mathf.Sqrt(vmSqr)) * m);

            return v;
        }
    }
}