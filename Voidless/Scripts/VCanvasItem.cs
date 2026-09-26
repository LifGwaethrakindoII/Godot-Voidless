using Godot;
using System;

namespace Voidless.UI
{
    public static class VCanvasItem
    {
        public static void SetMultipleVisible(bool visible = true, params CanvasItem[] items)
        {
            if(items == null || items.Length == 0) return;

            foreach(CanvasItem item in items)
            {
                if(item != null) item.Visible = visible;
            }
        }
    }
}