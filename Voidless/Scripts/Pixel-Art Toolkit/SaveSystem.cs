using Godot;
using Godot.Collections;
using System;
using System.Text.Json;

namespace Voidless.PixelArtToolkit
{
    public class SaveSystem
    {
        private static SaveSystem instance;
        private static ConfigFile configFile;
        private static PixelConversionSettings pixelConversionSettings;

        public static SaveSystem Instance
        {
            get
            {
                if(instance == null) instance = new SaveSystem();
                return instance;
            }
        }

        public static PixelConversionSettings PixelConversionSettings { get { return pixelConversionSettings; } }

        public static void Initialize()
        {
            //GenerateAllDefaultPalettes();
            LoadConfigFile();
            LoadSettings();
        }

        private static void LoadConfigFile()
        {
            configFile = new ConfigFile();
            Error loadError = configFile.Load(App.PATH_SETTINGS);
        }

        private static void LoadSettings()
        {
            string json = GetValue<string>(App.SECTION_PATH, App.KEY_SETTINGS_PIXELCONVERSION);
            string lastPath = GetValue<string>(App.SECTION_PATH, App.KEY_LASTPATH_IMAGES, OS.GetSystemDir(OS.SystemDir.Documents));

            pixelConversionSettings = VJson.FromJsonString<PixelConversionSettings>(json, PixelConversionSettings.Default());
            pixelConversionSettings.LastFilePath = lastPath;
        }

        public static void SetValue(string section, string key, string item, bool save = false)
        {
            configFile.SetValue(App.SECTION_PATH, key, item);
            if(save) configFile.Save(App.PATH_SETTINGS);
        }

        public static T GetValue<[MustBeVariant] T>(string section, string key, T variant = default)
        {
            Variant defaultVariant = Variant.From(variant);
            return configFile.GetValue(section, key, defaultVariant).As<T>();
        }

        public static void SavePixelConversionSettings()
        {
            string Json = pixelConversionSettings.ToJson();
            SetValue(App.SECTION_PATH, App.KEY_SETTINGS_PIXELCONVERSION, Json, true);
        }

#region Color-Palettes:
/*==========================================================================
|       Color-Palette:                                                     |
==========================================================================*/
        public static void SaveColorPalette(Color[] palette, string path)
        {
            string extension = System.IO.Path.GetExtension(path);

            switch(extension)
            {
                case App.EXTENSION_TRES:
                    VColorPalette.Save(palette, path);
                break;

                case App.EXTENSION_GPL:
                    VColorPalette.ExportToGpl(palette, path);
                break;

                case App.EXTENSION_PNG:
                    VColorPalette.ExportToPng(palette, path);
                break;

                case App.EXTENSION_ASE:
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
                case App.EXTENSION_TRES:
                    ColorPalette colorPalette = VColorPalette.Load(path);
                    if(colorPalette != null) newPalette = colorPalette.Colors;
                break;

                case App.EXTENSION_GPL:
                    newPalette = VColorPalette.ParseGpl(path);
                break;

                case App.EXTENSION_PNG:
                    newPalette = VColorPalette.ParsePng(path);
                break;

                case App.EXTENSION_ASE:
                    newPalette = VColorPalette.ParseAseprite(path);
                break;

                default:
                    GD.PrintErr(string.Concat("Extension ", extension, " not supported."));
                break;
            }

            if(newPalette.IsNullOrEmpty()) GD.PrintErr(string.Concat("Couldn't load Color palette at path: ", path, "."));

            return newPalette;
        }

        public static void GenerateAllDefaultPalettes()
        {
            string path = App.PATH_RESOURCES_PALETTES;
    
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
                string savePath = path + "/" + paletteName + App.EXTENSION_TRES;

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
#endregion
    }
}