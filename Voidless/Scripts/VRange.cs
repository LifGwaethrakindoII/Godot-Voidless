using Godot;
using System;
using System.Text;

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
            
            if(partnerToLink != null)
            {
                partnerToLink.SetupNormalized(step, signed);
                partnerToLink.Share(range);
            }
        }

        public static string RangeToString(this Range range)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Range: { Name = ");
            builder.Append(range.Name);
            builder.Append(", Min = ");
            builder.Append(range.MinValue);
            builder.Append(", Max = ");
            builder.Append(range.MaxValue);
            builder.Append(", Value = ");
            builder.Append(range.Value);
            builder.Append(" }");

            return builder.ToString();
        }
    }
}