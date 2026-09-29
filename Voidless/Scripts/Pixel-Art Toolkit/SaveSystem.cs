using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public class SaveSystem
    {
        private static SaveSystem instance;

        public static SaveSystem Instance
        {
            get
            {
                if(instance == null) instance = new SaveSystem();
                return instance;
            }
        }

        public static void Initialize()
        {
            GenerateAllDefaultPalettes();
        }

        public static void GenerateAllDefaultPalettes()
        {
            string path = Constants.PATH_RESOURCES_PALETTES;
    
            // 1. Check and create directory
            if (!DirAccess.DirExistsAbsolute(path))
            {
                Error dirError = DirAccess.MakeDirRecursiveAbsolute(path);
                if (dirError != Error.Ok) return; // Stop entirely if we can't make the folder
            }

            // 2. Iterate through presets
            ColorPalettePreset[] presets = (ColorPalettePreset[])Enum.GetValues(typeof(ColorPalettePreset));

            foreach (ColorPalettePreset preset in presets)
            {
                string paletteName = preset.ToString();
                string savePath = path + "/" + paletteName + Constants.EXTENSION_TRES;

                if (!ResourceLoader.Exists(savePath))
                {
                    // 3. Get colors and validate
                    Color[] colors = VColor.GetColorPalette(preset);
                    
                    if (colors == null || colors.Length == 0) continue;

                    // 4. Create and save resource
                    ColorPalette newPalette = new ColorPalette();
                    newPalette.Colors = colors;

                    Error saveError = ResourceSaver.Save(newPalette, savePath);
                }
            }
        }

        public static void SaveColorPalette(Color[] palette, string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            switch(extension)
            {
                case Constants.EXTENSION_TRES:
                    VColorPalette.Save(palette, path);
                break;

                case Constants.EXTENSION_GPL:
                    VColorPalette.ExportToGpl(palette, path);
                break;

                case Constants.EXTENSION_PNG:
                    VColorPalette.ExportToPng(palette, path);
                break;

                case Constants.EXTENSION_ASE:
                    // TODO: Yet to implement...
                break;

                default:
                    GD.PrintErr(string.Concat("Extension ", extension, " not supported."));
                break;
            }
        }

        public static Color[] LoadColorPalette(string path)
        {
            string extension = System.IO.Path.GetExtension(path);
            Color[] newPalette = null;

            switch(extension)
            {
                case Constants.EXTENSION_TRES:
                    ColorPalette colorPalette = VColorPalette.Load(path);
                    if(colorPalette != null) newPalette = colorPalette.Colors;
                break;

                case Constants.EXTENSION_GPL:
                    newPalette = VColorPalette.ParseGpl(path);
                break;

                case Constants.EXTENSION_PNG:
                    newPalette = VColorPalette.ParsePng(path);
                break;

                case Constants.EXTENSION_ASE:
                    newPalette = VColorPalette.ParseAseprite(path);
                break;

                default:
                    GD.PrintErr(string.Concat("Extension ", extension, " not supported."));
                break;
            }

            if(newPalette.IsNullOrEmpty()) GD.PrintErr(string.Concat("Couldn't load Color palette at path: ", path, "."));

            return newPalette;
        }
    }
}