using Godot;
using System;

namespace Voidless
{
    public static class VNode
    {
        public static void QueueFreeChildren(this Node node)
        {
            foreach(Node child in node.GetChildren())
            {
                child.QueueFree();
            }
        }
    }
}