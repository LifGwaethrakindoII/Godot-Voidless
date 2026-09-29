using Godot;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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

        public static ColorPalette ToColorPalette(Color[] palette)
        {
            ColorPalette colorPalette = new ColorPalette();
            colorPalette.Colors = palette;
            return colorPalette;
        }

        public static void Save(this ColorPalette colorPalette, string path)
        {
            Error saveResult = ResourceSaver.Save(colorPalette, path);

            if (saveResult == Error.Ok) GD.Print(string.Concat("[ColorPalette] Successfully saved Game Boy palette to: ", path, "."));
            else GD.PrintErr(string.Concat("[ColorPalette] Failed to save palette. Error code: ", saveResult.ToString(), "."));
        }

        public static void Save(Color[] palette, string path)
        {
            ColorPalette colorPalette = ToColorPalette(palette);
            if(colorPalette != null) colorPalette.Save(path);
        }

        public static ColorPalette Load(string path)
        {
            if(string.IsNullOrEmpty(path))
            {
                GD.PrintErr(string.Concat("[ColorPalette] Path", path, " not provided."));
                return null;
            }

            // 1. Load the resource from the file path
            Resource loadedResource = GD.Load<Resource>(path);

            if(loadedResource == null)
            {
                GD.PrintErr(string.Concat("[ColorPalette] Failed to load ColorPalette at path ", path, "."));
                return null;
            }

            ColorPalette palette = loadedResource as ColorPalette;

            if(palette == null)
            GD.PrintErr("[ColorPalette] Failed to load ColorPalette. The file might be corrupted or the wrong type.");

            return palette;
        }

        public static List<ColorPalette> LoadPalettesFromDirectory(string directoryPath)
        {
            List<ColorPalette> palettes = new List<ColorPalette>();

            DirAccess dirAccess = DirAccess.Open(directoryPath);

            if (dirAccess == null)
            {
                GD.PrintErr(string.Concat("[ColorPalette] Could not open directory: ", directoryPath));
                return palettes;
            }

            dirAccess.ListDirBegin();
            string fileName = dirAccess.GetNext();

            while (fileName != "")
            {
                // Check if it's a .tres file and not a directory
                if (!dirAccess.CurrentIsDir() && fileName.EndsWith(".tres"))
                {
                    string fullPath = directoryPath + "/" + fileName;
                    
                    // Load the resource and add it to our list
                    ColorPalette palette = ResourceLoader.Load<ColorPalette>(fullPath);
                    if (palette != null)
                    {
                        palettes.Add(palette);
                    }
                }
                fileName = dirAccess.GetNext();
            }

            dirAccess.ListDirEnd();
            return palettes;
        }

#region Gimp:
/*==========================================================================
|       Gimp Functions:                                                    |
==========================================================================*/
        public static Color[] ParseGpl(string path)
        {
            List<Color> colors = new List<Color>();

            // 1. Read all lines from the text file
            string[] lines = File.ReadAllLines(path);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                // 2. Skip empty lines
                if (string.IsNullOrEmpty(line)) continue;

                // 3. Skip ALL header/metadata lines (they start with '#')
                if (line.StartsWith("#")) continue;

                // 4. We are now on a color line. 
                // Example: "255	237	219	ffeddb"
                string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                // 5. Ensure we have at least R, G, and B values
                if (parts.Length >= 3)
                {
                    // TryParse is safer than Parse to prevent crashes on malformed files
                    if (int.TryParse(parts[0], out int red) &&
                        int.TryParse(parts[1], out int green) &&
                        int.TryParse(parts[2], out int blue))
                    {
                        // Convert 0-255 integers to 0.0-1.0 floats for Godot's Color
                        Color newColor = new Color(red / 255f, green / 255f, blue / 255f);
                        colors.Add(newColor);
                    }
                }
            }

            return colors.ToArray();
        }

        public static void ExportToGpl(Color[] colors, string filePath, string paletteName = "Custom Palette")
        {
            // 1. Build the text content using StringBuilder for performance
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("GIMP Palette");
            sb.AppendLine("Name: " + paletteName);
            sb.AppendLine("Columns: 0");
            sb.AppendLine("#");

            // 2. Write each color
            foreach (Color color in colors)
            {
                // Convert 0.0-1.0 floats back to 0-255 integers
                int r = (int)(color.R8); // R8 is a Godot 4 property that returns 0-255 byte
                int g = (int)(color.G8);
                int b = (int)(color.B8);

                // Format: R G B \t HexCode (Standard GPL format)
                string hex = color.ToHtml(false); // Gets hex without alpha
                sb.AppendLine($"{r}\t{g}\t{b}\t{hex}");
            }

            // 3. Write to disk
            File.WriteAllText(filePath, sb.ToString());
        }
#endregion

#region PNG:
/*==========================================================================
|       PNG Functions:                                                     |
==========================================================================*/
        public static Color[] ParsePng(string filePath)
        {
            HashSet<Color> uniqueColors = new HashSet<Color>();
            Image image = Image.LoadFromFile(filePath);

            if (image == null)
            {
                GD.PrintErr(string.Concat("[ColorPalette] Failed to load PNG image from: " + filePath));
                return new Color[0];
            }

            for (int y = 0; y < image.GetHeight(); y++)
            {
                for (int x = 0; x < image.GetWidth(); x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    if (pixelColor.A > 0.0f) uniqueColors.Add(pixelColor);
                }
            }

            return uniqueColors.ToArray();
        }

        public static void ExportToPng(Color[] colors, string filePath)
        {
            if (colors == null || colors.Length == 0) return;

            // 1. Create a 1D image strip (Width = number of colors, Height = 1)
            Image image = Image.CreateEmpty(colors.Length, 1, false, Image.Format.Rgba8);

            // 2. Set the pixels
            for (int x = 0; x < colors.Length; x++)
            {
                image.SetPixel(x, 0, colors[x]);
            }

            // 3. Save to disk
            Error saveError = image.SavePng(filePath);
            if (saveError != Error.Ok)
            {
                GD.PrintErr(string.Concat("[ColorPalette] Failed to export PNG palette. Error: " + saveError));
            }
        }
#endregion

#region Aseprite:
/*==========================================================================
|       Aseprite Functions:                                                |
==========================================================================*/
        public static Color[] ParseAseprite(string filePath)
        {
            // TODO: Implement function.
            return null;
        }

        public static void ExportToAseprite(Color[] colors, string path)
        {
            // TODO: Yes, implement function...
        }
#endregion
    }
}