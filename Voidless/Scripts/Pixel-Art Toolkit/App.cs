using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public enum DitheringType { None, Ordered, Floyd_S }

    public enum ColorPaletteType { AutoExtract, DefinedPreset, CustomPreset }

    // Hybrid = Octree + K-Means
    public enum ColorExtractionMethod { Octree, MedianCut, KMeans, Hybrid }
    
    public static class App
    {
        public static readonly string[] FILTERS_IMAGES;
        public static readonly string[] FILTERS_PALETTES;

        // Titles:
        public const string TITLE_LOADIMAGE = "Load Image";
        public const string TITLE_SAVEIMAGE = "Save Image";
        public const string TITLE_IMPORTPALETTE = "Import Color-Palette";
        public const string TITLE_EXPORTPALETTE = "Export Color-Palette";
        // Sections:
        public const string SECTION_PATH = "Path";
        public const string SECTION_SETTINGS = "Settings";
        // Paths:
        public const string PATH_SETTINGS = "user://settings.cfg";
        public const string PATH_RESOURCES = "res://Resources";
        public const string PATH_RESOURCES_PALETTES = PATH_RESOURCES + "/Color-Palettes";
        public const string PATH_USER = "user://";
        public const string PATH_USER_PALETTES = PATH_USER + "Color-Palettes";
        // Keys:
        public const string KEY_LASTPATH_IMAGES = "LastPath_Images";
        public const string KEY_LASTPATH_PALETTES = "LastPath_Palettes";
        public const string KEY_SETTINGS_PIXELCONVERSION = "Settings_PixelConversion";
        public const string VARIANT_LASTPATH = "C:/My/Images";
        // Filters:
        public const string FILTER_ALL = "*";
        public const string FILTER_IMAGES = "*.png, *.jpg, *.jpeg, *.bmp, *.webp";
        public const string FILTER_PALETTES = "*.tres, *.res, *.gpl, *.ase, *.aseprite, *.png, *.jpg, *.jpeg";
        // Extensions:
        public const string EXTENSION_TRES = ".tres";
        public const string EXTENSION_GPL = ".gpl";
        public const string EXTENSION_PNG = ".png";
        public const string EXTENSION_ASE = ".ase";
        // File Names:
        public const string FILENAME_NEWIMAGE = "NewImage";
        public const string FILENAME_NEWPALETTE = "NewColor-Palette";
        // Other:
        public const float THRESHOLD_LANDSCAPE = 1.2f;
        public const float MIN_RANGEVALUE = -100.0f;
        public const float MAX_RANGEVALUE = 100.0f;
        public const float INVERSE_RANGE = 1.0f / MAX_RANGEVALUE;
        public const float DIMENSION_SWATCH = 24.0f;
        public const float RATIO_FILEDIALOG = 0.8f;
        public const double STEP_RANGE = 0.01;
        public const int MAX_PALETTECOLORS = 256;
        public const int MAX_DIMENSION = 4096;

        static App()
        {
            FILTERS_IMAGES = new string[] { "*.png", "*.jpg", "*.jpeg" };
            FILTERS_PALETTES = new string[] { "*.tres", "*.res", "*.gpl", "*.ase", "*.aseprite", "*.png", "*.jpg", "*.jpeg" };
        }
    }
}