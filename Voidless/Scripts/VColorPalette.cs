using Godot;
using System;
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

        public static List<ColorPalette> LoadPalettesFromDirectory(string directoryPath)
        {
            List<ColorPalette> palettes = new List<ColorPalette>();

            // Open the directory in Godot
            DirAccess dirAccess = DirAccess.Open(directoryPath);

            if (dirAccess == null)
            {
                GD.PrintErr("Could not open directory: " + directoryPath);
                return palettes; // Return empty list if folder doesn't exist yet
            }

            // Iterate through all files in the folder
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

        //Gimp
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

        public static Color[] ParsePng(string filePath)
        {
            // 1. Use HashSet for O(1) lookups instead of List's O(N) Contains
            HashSet<Color> uniqueColors = new HashSet<Color>();

            // 2. Load the image directly from the file path
            Image image = Image.LoadFromFile(filePath);

            if (image == null)
            {
                GD.PrintErr("Failed to load PNG image from: " + filePath);
                return new Color[0];
            }

            // 3. Iterate through every pixel in the image
            for (int y = 0; y < image.GetHeight(); y++)
            {
                for (int x = 0; x < image.GetWidth(); x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    // 4. Filter out fully transparent pixels
                    if (pixelColor.A > 0.0f)
                    {
                        // HashSet.Add automatically checks for duplicates!
                        // It returns true if the color was added, and false if it already existed.
                        uniqueColors.Add(pixelColor);
                    }
                }
            }

            // 5. Convert the HashSet back to an Array for the return type
            return uniqueColors.ToArray();
        }

        public static Color[] ParseAseprite(string filePath)
        {
            List<Color> colors = new List<Color>();

            using (FileStream fileStream = File.OpenRead(filePath))
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                // Ensure file is large enough
                if (fileStream.Length < 16) return colors.ToArray();

                // 1. Skip the 16-byte custom header ("ASEF" + metadata)
                reader.ReadBytes(16);

                // 2. Scan through the file
                while (reader.BaseStream.Position < fileStream.Length - 4)
                {
                    // This custom format uses 2-byte sizes and 2-byte types
                    ushort chunkSize = reader.ReadUInt16();
                    ushort chunkType = reader.ReadUInt16();

                    // Calculate where this chunk ends
                    long chunkEndPosition = reader.BaseStream.Position + (chunkSize - 4);

                    // 3. Check for Palette Chunk (Type 0x0007)
                    if (chunkType == 0x0007)
                    {
                        // Seek to 12 bytes before the end of the chunk to find the RGB floats
                        reader.BaseStream.Position = chunkEndPosition - 12;

                        float r = reader.ReadSingle();
                        float g = reader.ReadSingle();
                        float b = reader.ReadSingle();

                        colors.Add(new Color(r, g, b, 1.0f));
                    }

                    // Move to the start of the next chunk
                    reader.BaseStream.Position = chunkEndPosition;
                }
            }

            return colors.ToArray();
        }
    }
}