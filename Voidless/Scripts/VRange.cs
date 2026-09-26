using Godot;
using System;

using Range = Godot.Range;

namespace Voidless.UI
{
    public static class VRange
    {
        public static void SetupFloatRange(this Range range, float min = -1.0f, float max = 1.0f)
        {
            range.MinValue = min;
            range.MaxValue = max;
        }

        public static void SetupIntRange(this Range range, int min = -1, int max = 1)
        {
            range.MinValue = min;
            range.MaxValue = max;
        }

        public static void SetupNormalized(this Range range, double step = 0.01, bool signed = false, Range partnerToLink = null)
        {
            range.SetupFloatRange(!signed ? 0.0f : -1.0f, 1.0f);
            range.Step = step;
            range.Rounded = false;

            if (partnerToLink != null) partnerToLink.Share(range);
        }
    }
}