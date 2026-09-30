using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    [Serializable]
    public class PixelConversionSettings
    {
        public bool EnhanceEdges { get; set; }
        public bool DarkOutline { get; set; }
        public float Contrast { get; set; }
        public float Brightness { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int ScaleFactor { get; set; }
        public bool ProportionalEditing { get; set; }
        public int MaxColors { get; set; }
        public int PixelScale { get; set; }
        public float DitheringStrength { get; set; }
        public string LastFilePath { get; set; }
        public string LastPalettePath { get; set; }
        public ColorPaletteType PaletteType { get; set; }
        public DitheringType DitheringType { get; set; }
        public ColorExtractionMethod ColorExtractionMethod { get; set; }

        public static PixelConversionSettings Default()
        {
            return new PixelConversionSettings
            {
                EnhanceEdges = false,
                DarkOutline = false,
                ProportionalEditing = true,
                Contrast = 0.0f,
                Brightness = 0.0f,
                DitheringStrength = 0.2f,
                Width = 64,
                Height = 64,
                ScaleFactor = 1,
                MaxColors = 16,
                PixelScale = 1,
                LastFilePath = "C:/My/Images",
                LastPalettePath = "C:/My/Images",
                PaletteType = ColorPaletteType.AutoExtract, // Assuming this is your enum name
                DitheringType = DitheringType.None,
                ColorExtractionMethod = ColorExtractionMethod.Octree
            };
        }

        public string ToJson()
        {
            return VJson.ToJsonString(this);
        }
    }
}