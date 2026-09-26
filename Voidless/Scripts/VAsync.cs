using Godot;
using System;
using System.Threading.Tasks;

namespace Voidless
{
    public static class VAsync
    {
        public static async Task LerpScale(this Node2D node, Vector2 a, Vector2 b, float d, Func<float, float> f = null/*, Action onScaleEnds = null*/)
        {
            float t = 0.0f;
            float i = 1.0f / d;

            if(f == null) f = VMath.F;
            node.Scale = a;

            while(t < 1.0f)
            {
                node.Scale = VVector2.Lerp(a, b, f(t));

                float dt = (float)node.GetProcessDeltaTime();
                t += (dt * i);

                await node.ToSignal(node.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            node.Scale = b;
            //if(onScaleEnds != null) onScaleEnds();
        }

        public static async Task LerpScale(this Control control, Vector2 a, Vector2 b, float d, Func<float, float> f = null/*, Action onScaleEnds = null*/)
        {
            float t = 0.0f;
            float i = 1.0f / d;

            if(f == null) f = VMath.F;
            control.Scale = a;

            while(t < 1.0f)
            {
                control.Scale = VVector2.Lerp(a, b, f(t));

                float dt = (float)control.GetProcessDeltaTime();
                t += (dt * i);

                await control.ToSignal(control.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            control.Scale = b;
            //if(onScaleEnds != null) onScaleEnds();
        }

        public static async Task LerpColor(this ColorRect colorRect, Color a, Color b, float d, Func<float, float> f = null)
        {
            float t = 0.0f;
            float i = 1.0f/d;

            if(f == null) f = VMath.F;
            colorRect.Color = a;

            while(t < 1.0f)
            {
                colorRect.Color = a.Lerp(b, f(t));

                float dt = (float)colorRect.GetProcessDeltaTime();
                t += (dt * i);

                await colorRect.ToSignal(colorRect.GetTree(), SceneTree.SignalName.ProcessFrame);
            }

            colorRect.Color = b;
        }
    }
}