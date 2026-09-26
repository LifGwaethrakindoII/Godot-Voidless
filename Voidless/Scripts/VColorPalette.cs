using Godot;
using System;
using System.Linq;

namespace Voidless
{
    public static class VColorPalette
    {
        public static ColorPalette Create(params Color[] palette)
        {
            return new ColorPalette() { Colors = palette };
        }

        public static void AppendUnique(this ColorPalette colorPalette, params Color[] palette)
        {
            if(colorPalette == null) return;

            colorPalette.Colors = colorPalette.Colors.Concat(palette).Distinct().ToArray();
        }

        public static bool TryLoad(string path, ref ColorPalette colorPalette, ColorPalette defaultPalette = null)
        {
            colorPalette = Load(path);

            if(colorPalette == null)
            {
                colorPalette = defaultPalette;
                return false;
            }

            return true;
        }

        public static void Save(this ColorPalette colorPalette, string path)
        {
            Error saveResult = ResourceSaver.Save(colorPalette, path);

            if (saveResult == Error.Ok) GD.Print("Successfully saved Game Boy palette to: " + path + ".");
            else GD.Print("Failed to save palette. Error code: " + saveResult.ToString() + ".");
        }

        public static ColorPalette Load(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                GD.Print("Path not provided.");
                return null;
            }

            // 1. Load the resource from the file path
            Resource loadedResource = GD.Load<Resource>(path);

            if(loadedResource == null)
            {
                GD.Print("Failed to load ColorPalette at path " + path + ".");
                return null;
            }

            ColorPalette palette = loadedResource as ColorPalette;

            // 2. Check if the loaded resource is actually a ColorPalette
            if(palette != null)
            {
                GD.Print("Successfully loaded palette with " + palette.Colors.Length + " colors.");

                // 3. Iterate through the PackedColorArray
                foreach (Color color in palette.Colors)
                {
                    // ToHtml() is a handy Godot method that converts a Color to a Hex string (e.g., "9bbc0f")
                    GD.Print("Loaded Color Hex: " + color.ToHtml()); 
                }
            }
            else GD.Print("Failed to load ColorPalette. The file might be corrupted or the wrong type.");

            return palette;
        }
    }
}