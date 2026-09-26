using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public static class Constants
    {
        public static readonly string[] FILTERS_IMAGES;
        public static readonly string[] FILTERS_COLORPALETTES;

        public const string SECTION_PATH = "Path";
        public const string PATH_SETTINGS = "user://settings.cfg";
        public const string KEY_LASTPATH = "LastPath";
        public const string VARIANT_LASTPATH = "C:/My/Images";
        public const float THRESHOLD_LANDSCAPE = 1.2f;
        public const float MIN_RANGEVALUE = -100.0f;
        public const float MAX_RANGEVALUE = 100.0f;
        public const float INVERSE_RANGE = 1.0f / MAX_RANGEVALUE;
        public const float DIMENSION_SWATCH = 24.0f;
        public const double STEP_RANGE = 0.01;
        public const int MAX_PALETTECOLORS = 256;
        public const int MAX_DIMENSION = 4096;

        static Constants()
        {
            FILTERS_IMAGES = new string[] { "*.png", "*.jpg", "*.jpeg" };
            FILTERS_COLORPALETTES = new string[] { "*.tres", "*.res" };
        }
    }
}