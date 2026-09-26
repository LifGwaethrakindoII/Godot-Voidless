using Godot;
using System;

namespace Voidless
{
    public partial class PixelArtToolKitEnhancements : Control
    {
        [ExportSubgroup("Sliders:")]
        [Export] private HSlider contrastSlider;
        [Export] private HSlider brightnessSlider;
    }
}