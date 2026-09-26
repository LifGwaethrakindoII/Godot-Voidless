using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    [Serializable]
    public struct PixelArtToolkitSettings
    {
        // --- Enhancements ---
        public bool EnhanceEdges { get; set; }
        public bool DarkOutline { get; set; }
        public float Contrast { get; set; }
        public float Brightness { get; set; }

        // --- Dimensions ---
        public int Width { get; set; }
        public int Height { get; set; }
        public int Dimension { get; set; } // I assume this is for the preset size (16, 32, 64...)
        public bool ProportionalEditing { get; set; }

        // --- Palette & Dithering ---
        public int MaxColors { get; set; }
        public int PixelScale { get; set; }
        public float DitheringStrength { get; set; }
        
        // --- System ---
        public string LastFilePath { get; set; }
        
        // Enums for dropdowns
        public ColorPaletteType PaletteType { get; set; }
        public DitheringType DitheringType { get; set; }
        public ColorExtractionMethod ColorExtractionMethod { get; set; }

        public static PixelArtToolkitSettings Default()
        {
            return new PixelArtToolkitSettings
            {
                EnhanceEdges = false,
                DarkOutline = false,
                ProportionalEditing = true,
                Contrast = 0.0f,
                Brightness = 0.0f,
                DitheringStrength = 0.2f,
                Width = 64,
                Height = 64,
                Dimension = 64,
                MaxColors = 16,
                PixelScale = 1,
                LastFilePath = "C:/My/Images",
                PaletteType = ColorPaletteType.AutoExtract, // Assuming this is your enum name
                DitheringType = DitheringType.None,
                ColorExtractionMethod = ColorExtractionMethod.Octree
            };
        }
    }
}