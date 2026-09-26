using Godot;
using System;
using System.Collections.Generic;

namespace Voidless
{
#region ChannelEnums:
    [Flags]
    public enum RGBChannels
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 4,
        Alpha = 8,
        All = Red | Blue | Green | Alpha
    }

    [Flags]
    public enum CMYKChannels
    {
        None = 0,
        Cyan = 1,
        Magenta = 2,
        Yellow = 4,
        Black = 8,
        All = Cyan | Magenta | Yellow | Black
    }
#endregion

#region PaletteEnums:
    public enum HardwareColorPalette
    {
        CGA = 0,
        EGA = 1,
        NES = 2,
        GameBoy = 3,
        Commodore64 = 4,
        ZXSpectrum = 5,
    }

    public enum FantasyConsoleColorPalette
    {
        PICO_8 = 6,
        Sweetie_16 = 7
    }

    public enum ModernColorPalette
    {
        Endesga32 = 8,
        DB32 = 9
    }

    public enum ColorPalettePreset
    {
        CGA,
        EGA,
        NES,
        GameBoy,
        Commodore64,
        ZXSpectrum,
        PICO_8,
        Sweetie_16,
        Endesga32,
        DB32,
        Grayscale,
        OneBit
    }
#endregion

    public static class VColor
    {
        public static readonly Color[] PALETTE_GAMEBOY;
        public static readonly Color[] PALETTE_CGA;
        public static readonly Color[] PALETTE_NES;
        public static readonly Color[] PALETTE_GRAYSCALE_8;
        public static readonly Color[] PALETTE_PICO8;
        public static readonly Color[] PALETTE_SWEETIE16;
        public static readonly Color[] PALETTE_ENDESGA32;
        public static readonly Color[] PALETTE_1BIT;
        public static readonly Color[] PALETTE_EGA;
        public static readonly Color[] PALETTE_COMMODORE64;
        public static readonly Color[] PALETTE_ZX_SPECTRUM;
        public static readonly Color[] PALETTE_DB32;

        static VColor()
        {
#region PaletteDefinitions:
            // Game Boy (4 colors)
            PALETTE_GAMEBOY = new Color[]
            {
                new Color("#9BBC0F"), // Lightest (Background)
                new Color("#8BAC0F"), // Light
                new Color("#306230"), // Dark
                new Color("#0F380F")  // Darkest (Foreground)
            };

            // CGA - Color Graphics Adapter (16 colors)
            PALETTE_CGA = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#0000AA"), // Blue
                new Color("#00AA00"), // Green
                new Color("#00AAAA"), // Cyan
                new Color("#AA0000"), // Red
                new Color("#AA00AA"), // Magenta
                new Color("#AA5500"), // Brown
                new Color("#AAAAAA"), // Light Gray
                new Color("#555555"), // Dark Gray
                new Color("#5555FF"), // Light Blue
                new Color("#55FF55"), // Light Green
                new Color("#55FFFF"), // Light Cyan
                new Color("#FF5555"), // Light Red
                new Color("#FF55FF"), // Light Magenta
                new Color("#FFFF55"), // Yellow
                new Color("#FFFFFF")  // White
            };

            // NES - Nintendo Entertainment System (54 colors)
            PALETTE_NES = new Color[]
            {
                new Color("#000000"), new Color("#FCFCFC"), new Color("#F8F8F8"), new Color("#BCBCBC"),
                new Color("#B0B0B0"), new Color("#888888"), new Color("#7C7C7C"), new Color("#545454"),
                new Color("#00FC00"), new Color("#00E400"), new Color("#00B800"), new Color("#008C00"),
                new Color("#00FCFC"), new Color("#00E4E4"), new Color("#00B8B8"), new Color("#008C8C"),
                new Color("#0000FC"), new Color("#0000E4"), new Color("#0000B8"), new Color("#00008C"),
                new Color("#F8B8F8"), new Color("#F898F8"), new Color("#F878F8"), new Color("#F838F8"),
                new Color("#F8B800"), new Color("#F89800"), new Color("#F87800"), new Color("#F83800"),
                new Color("#F8B8B8"), new Color("#F89898"), new Color("#F87878"), new Color("#F83838"),
                new Color("#BCF8BC"), new Color("#98F898"), new Color("#78F878"), new Color("#38F838"),
                new Color("#BCF8F8"), new Color("#98F8F8"), new Color("#78F8F8"), new Color("#38F8F8"),
                new Color("#BCBCF8"), new Color("#9898F8"), new Color("#7878F8"), new Color("#3838F8"),
                new Color("#FCB8FC"), new Color("#FC98FC"), new Color("#FC78FC"), new Color("#FC38FC"),
                new Color("#FCB8B8"), new Color("#FC9898"), new Color("#FC7878"), new Color("#FC3838"),
                new Color("#FCFCB8"), new Color("#FCFC98")
            };

            // Grayscale (8 colors)
            PALETTE_GRAYSCALE_8 = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#242424"), // Very Dark Gray
                new Color("#484848"), // Dark Gray
                new Color("#6C6C6C"), // Medium Dark Gray
                new Color("#909090"), // Medium Light Gray
                new Color("#B4B4B4"), // Light Gray
                new Color("#D8D8D8"), // Very Light Gray
                new Color("#FFFFFF")  // White
            };

            // PICO-8 (16 colors)
            PALETTE_PICO8 = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#1D2B53"), // Dark Blue
                new Color("#7E2553"), // Dark Purple
                new Color("#008751"), // Dark Green
                new Color("#AB5236"), // Brown
                new Color("#5F574F"), // Dark Gray
                new Color("#C2C3C7"), // Light Gray
                new Color("#FFF1E8"), // White
                new Color("#FF004D"), // Red
                new Color("#FFA300"), // Orange
                new Color("#FFEC27"), // Yellow
                new Color("#00E436"), // Green
                new Color("#29ADFF"), // Blue
                new Color("#83769C"), // Purple
                new Color("#FF77A8"), // Pink
                new Color("#FFCCAA")  // Peach
            };

            // Sweetie-16 (16 colors)
            PALETTE_SWEETIE16 = new Color[]
            {
                new Color("#1A1C2C"), // Dark Purple/Black
                new Color("#5D275D"), // Dark Purple
                new Color("#B13E53"), // Dark Pink/Red
                new Color("#EF7D57"), // Orange/Peach
                new Color("#FFCD75"), // Light Orange/Yellow
                new Color("#A7F070"), // Light Green
                new Color("#38B764"), // Green
                new Color("#257179"), // Teal
                new Color("#29366F"), // Dark Blue
                new Color("#3B5DC9"), // Blue
                new Color("#41A6F6"), // Light Blue
                new Color("#73EFF7"), // Cyan
                new Color("#F4F4F4"), // White
                new Color("#94B0C2"), // Light Gray/Blue
                new Color("#566C86"), // Gray/Blue
                new Color("#333C57")  // Dark Gray/Blue
            };

            // Endesga 32 (32 colors)
            PALETTE_ENDESGA32 = new Color[]
            {
                new Color("#000000"), new Color("#1D1D1D"), new Color("#3E3E3E"), new Color("#5C5C5C"),
                new Color("#7D7D7D"), new Color("#9E9E9E"), new Color("#BFBFBF"), new Color("#E0E0E0"),
                new Color("#F24040"), new Color("#FF6B6B"), new Color("#FF9F9F"), new Color("#FFD4D4"),
                new Color("#FF8B2E"), new Color("#FFA94D"), new Color("#FFC76D"), new Color("#FFE68D"),
                new Color("#FFD93D"), new Color("#FFE663"), new Color("#FFF28A"), new Color("#FFFFB0"),
                new Color("#6BCF48"), new Color("#8AE06D"), new Color("#A9F092"), new Color("#C8FFB7"),
                new Color("#3FA8D8"), new Color("#63B8E3"), new Color("#87C8EE"), new Color("#ABD8F9"),
                new Color("#6B68D8"), new Color("#8784E3"), new Color("#A3A0EE"), new Color("#BFBCF9")
            };

            // 1-Bit (2 colors)
            PALETTE_1BIT = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#FFFFFF")  // White
            };

            // EGA - Enhanced Graphics Adapter (16 colors)
            PALETTE_EGA = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#0000AA"), // Blue
                new Color("#00AA00"), // Green
                new Color("#00AAAA"), // Cyan
                new Color("#AA0000"), // Red
                new Color("#AA00AA"), // Magenta
                new Color("#AA5500"), // Brown
                new Color("#AAAAAA"), // Light Gray
                new Color("#555555"), // Dark Gray
                new Color("#5555FF"), // Light Blue
                new Color("#55FF55"), // Light Green
                new Color("#55FFFF"), // Light Cyan
                new Color("#FF5555"), // Light Red
                new Color("#FF55FF"), // Light Magenta
                new Color("#FFFF55"), // Yellow
                new Color("#FFFFFF")  // White
            };

            // Commodore 64 (16 colors)
            PALETTE_COMMODORE64 = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#FFFFFF"), // White
                new Color("#68372B"), // Red-Brown
                new Color("#70A4B2"), // Cyan
                new Color("#6F3D86"), // Purple
                new Color("#588D43"), // Green
                new Color("#352879"), // Blue
                new Color("#B8C76F"), // Yellow
                new Color("#6F4F25"), // Orange-Brown
                new Color("#433900"), // Dark Brown
                new Color("#9A6759"), // Light Brown
                new Color("#444444"), // Dark Gray
                new Color("#6C6C6C"), // Gray
                new Color("#9AD284"), // Light Green
                new Color("#6C5EB5"), // Light Blue
                new Color("#959595")  // Light Gray
            };

            // ZX Spectrum (15 colors - original hardware)
            PALETTE_ZX_SPECTRUM = new Color[]
            {
                new Color("#000000"), // Black
                new Color("#0000D7"), // Blue
                new Color("#D70000"), // Red
                new Color("#D700D7"), // Magenta
                new Color("#00D700"), // Green
                new Color("#00D7D7"), // Cyan
                new Color("#D7D700"), // Yellow
                new Color("#D7D7D7"), // White
                new Color("#00005C"), // Bright Black (Blue)
                new Color("#0000FF"), // Bright Blue
                new Color("#FF0000"), // Bright Red
                new Color("#FF00FF"), // Bright Magenta
                new Color("#00FF00"), // Bright Green
                new Color("#00FFFF"), // Bright Cyan
                new Color("#FFFF00")  // Bright Yellow
            };

            // DragonBones 32 (32 colors)
            PALETTE_DB32 = new Color[]
            {
                new Color("#000000"), new Color("#1D2B53"), new Color("#7E2553"), new Color("#008751"),
                new Color("#AB5236"), new Color("#5F574F"), new Color("#C2C3C7"), new Color("#FFF1E8"),
                new Color("#FF004D"), new Color("#FFA300"), new Color("#FFEC27"), new Color("#00E436"),
                new Color("#29ADFF"), new Color("#83769C"), new Color("#FF77A8"), new Color("#FFCCAA"),
                new Color("#2E1437"), new Color("#69344F"), new Color("#9B4F5A"), new Color("#C96A68"),
                new Color("#F0897B"), new Color("#F5B393"), new Color("#F5D4A8"), new Color("#F5F0C0"),
                new Color("#5F574F"), new Color("#7A7269"), new Color("#968E83"), new Color("#B2AAA1"),
                new Color("#CEC7BF"), new Color("#EAE3DB"), new Color("#FFFFFF"), new Color("#000000")
            };
#endregion
        }

        public static Color[] GetColorPalette(ColorPalettePreset palette)
        {
            switch(palette)
            {
                case ColorPalettePreset.CGA:            return PALETTE_CGA;
                case ColorPalettePreset.EGA:            return PALETTE_EGA;
                case ColorPalettePreset.NES:            return PALETTE_NES;
                case ColorPalettePreset.GameBoy:        return PALETTE_GAMEBOY;
                case ColorPalettePreset.Commodore64:    return PALETTE_COMMODORE64;
                case ColorPalettePreset.ZXSpectrum:     return PALETTE_ZX_SPECTRUM;
                case ColorPalettePreset.PICO_8:         return PALETTE_PICO8;
                case ColorPalettePreset.Sweetie_16:     return PALETTE_SWEETIE16;
                case ColorPalettePreset.Endesga32:      return PALETTE_ENDESGA32;
                case ColorPalettePreset.DB32:           return PALETTE_DB32;
                case ColorPalettePreset.Grayscale:      return PALETTE_GRAYSCALE_8;
                case ColorPalettePreset.OneBit:         return PALETTE_1BIT;
                default:                                return PALETTE_1BIT;
            }
        }

#region ConsoleColorMapping:
        /// <summary>
        /// Quantizes a color to SNES color space (15-bit, 5 bits per RGB channel)
        /// </summary>
        public static Color MapToSNES(Color color)
        {
            const int bitsPerChannel = 5;
            const float maxVal = 31.0f; // 2^5 - 1
            
            return new Color(
                Mathf.Floor(color.R * maxVal) / maxVal,
                Mathf.Floor(color.G * maxVal) / maxVal,
                Mathf.Floor(color.B * maxVal) / maxVal,
                color.A
            );
        }

        /// <summary>
        /// Quantizes a color to Genesis/Mega Drive color space (9-bit, 3 bits per RGB channel)
        /// </summary>
        public static Color MapToGenesis(Color color)
        {
            const int bitsPerChannel = 3;
            const float maxVal = 7.0f; // 2^3 - 1
            
            return new Color(
                Mathf.Floor(color.R * maxVal) / maxVal,
                Mathf.Floor(color.G * maxVal) / maxVal,
                Mathf.Floor(color.B * maxVal) / maxVal,
                color.A
            );
        }

        /// <summary>
        /// Quantizes a color to Game Boy Color (15-bit, same as SNES)
        /// </summary>
        public static Color MapToGameBoyColor(Color color)
        {
            return MapToSNES(color);
        }

        /// <summary>
        /// Quantizes a color to 16-bit High Color (5-6-5 RGB). 
        /// Common in older PC games and the Game Boy Advance.
        /// </summary>
        public static Color MapTo16Bit565(Color color)
        {
            float maxR = 31.0f; // 2^5 - 1
            float maxG = 63.0f; // 2^6 - 1
            float maxB = 31.0f; // 2^5 - 1
            
            return new Color(
                Mathf.Floor(color.R * maxR) / maxR,
                Mathf.Floor(color.G * maxG) / maxG,
                Mathf.Floor(color.B * maxB) / maxB,
                color.A
            );
        }

        /// <summary>
        /// Quantizes a color to PlayStation 1 color space (15-bit, 5 bits per RGB channel).
        /// </summary>
        public static Color MapToPS1(Color color)
        {
            // PS1 uses the same 15-bit color space as the SNES
            return MapToSNES(color);
        }

        /// <summary>
        /// Quantizes a color to Nintendo 64 standard 16-bit texture format (5-5-5-1 RGBA).
        /// </summary>
        public static Color MapToN64(Color color)
        {
            const float maxRGB = 31.0f; // 5 bits for RGB
            const float maxA = 1.0f;    // 1 bit for Alpha (fully transparent or fully opaque)
            
            // For the alpha channel, if it's greater than 0.5, make it 1.0, else 0.0
            float quantizedAlpha = color.A > 0.5f ? 1.0f : 0.0f;

            return new Color(
                Mathf.Floor(color.R * maxRGB) / maxRGB,
                Mathf.Floor(color.G * maxRGB) / maxRGB,
                Mathf.Floor(color.B * maxRGB) / maxRGB,
                quantizedAlpha
            );
        }
#endregion

        public static Color Regular(float c, float a = 1.0f)
        {
            return new Color(c, c, c, a);
        }

        public static Color Clamp(this Color c, float min = -1.0f, float max = 1.0f)
        {
            c.R = Mathf.Clamp(c.R, min, max);
            c.G = Mathf.Clamp(c.G, min, max);
            c.B = Mathf.Clamp(c.B, min, max);
            c.A = Mathf.Clamp(c.A, min, max);

            return c;
        }

        public static float EuclideanDistance(Color a, Color b)
        {
            float dr = a.R - b.R;
            float dg = a.G - b.G;
            float db = a.B - b.B;

            return (dr * dr) + (dg * dg) + (db * db);
        }

        public static Color AddValue(this Color color, float value)
        {
            return new Color
            (
                Mathf.Clamp(color.R + value, 0.0f, 1.0f),
                Mathf.Clamp(color.G + value, 0.0f, 1.0f),
                Mathf.Clamp(color.B + value, 0.0f, 1.0f),
                color.A
            );
        }

        public static Color Brightness(Color color, float b)
        {
            b = Mathf.Clamp(b, -1.0f, 1.0f);
            Color brightness = new Color(b, b, b, 0.0f);
            return color + brightness; 
        }

        public static Color Contrast(Color color, float c)
        {
            c = Mathf.Clamp(c, -1.0f, 1.0f);
    
            // Map c from [-1, 1] to [0, 2]
            // c = 0 → factor = 1 (no change)
            // c = 1 → factor = 2 (max contrast)
            // c = -1 → factor = 0 (no contrast, all gray)
            float factor = c + 1.0f;
            
            Color gray = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            
            // Apply the formula: (color - 0.5) * factor + 0.5
            return (color - gray) * factor + gray;
        }

        public static Color FindClosestColor(this Color color, params Color[] palette)
        {
            if(palette == null || palette.Length == 0) return color;

            float minDistance = Mathf.Inf;
            int index = -1;

            for(int i = 0; i < palette.Length; i++)
            {
                Color c = palette[i];
                float euclideanDistance = EuclideanDistance(color, c);

                if(euclideanDistance < minDistance)
                {
                    minDistance = euclideanDistance;
                    index = i;
                }
            }

            return palette[index];
        }

#region Dithering:
        public static Color[,] OrderedDithering(Image image, float strength = 0.2f, params Color[] palette)
        {
            if(image == null || palette == null || palette.Length == 0) return null;

            const float i = 1.0f / 16.0f;

            int width = image.GetWidth();
            int height = image.GetHeight();
            int matrixSize = 4;
            Color[,] pixels = new Color[width, height];

            for(int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    Color pixel = image.GetPixel(x, y);

                    if(pixel.A == 0.0f)
                    {
                        image.SetPixel(x, y, pixel);
                        continue;
                    }

                    float matrixValue = VImage.BAYERMATRIX_4X4[x % matrixSize, y % matrixSize];
                    float threshold = ((matrixValue * i) - 0.5f) * strength;
                    Color adjustedColor = pixel + VColor.Regular(threshold, 0.0f);
                    pixels[x, y] = VColor.FindClosestColor(adjustedColor, palette);
                }
            }

            return pixels;
        }

        public static Color[,] FloydSteinbergDithering(Image image, float strength = 0.2f, params Color[] palette)
        {
            if(image == null || palette == null || palette.Length == 0) return null;

            const float r = 7.0f / 16.0f;   // Right (x + 1, y)
            const float bl = 3.0f / 16.0f;  // Bottom-left (x - 1, y + 1)
            const float b = 5.0f / 16.0f;   // Bottom (x, y + 1)
            const float br = 1.0f / 16.0f;   // Bottom-right (x + 1, y + 1)

            int width = image.GetWidth();
            int height = image.GetHeight();
            Color[,] pixels = new Color[width, height];

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    pixels[x, y] = image.GetPixel(x, y);
                }
            }

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    Color pixel = pixels[x, y];

                    pixel = pixel.Clamp(0.0f, 1.0f);

                    if(pixel.A == 0.0f) continue;

                    Color closestColor = VColor.FindClosestColor(pixel, palette);
                    Color dc = pixel - closestColor;
                    bool leftN = (x - 1) >= 0;
                    bool rightN = (x + 1) < width;
                    bool bottomN = (y + 1) < height;

                    dc *= strength;

                    pixels[x, y] = closestColor;
                    if(rightN) pixels[x + 1, y] += dc * r;                  // Right
                    if(leftN && bottomN) pixels[x - 1, y + 1] += dc * bl;   // Bottom-left
                    if(bottomN) pixels[x, y + 1] += dc * b;                 // Bottom
                    if(rightN && bottomN) pixels[x + 1, y + 1] += dc * br;  // Bottom-right
                }
            }
            
            return pixels;
        }
#endregion

#region PaletteExtractions:
        /// <summary>
        /// Extracts a dominant color palette from an image using an Octree algorithm.
        /// </summary>
        /// <param name="image">The source image.</param>
        /// <param name="maxColors">The target number of colors for the palette.</param>
        /// <returns>An array of Colors representing the extracted palette.</returns>
        public static Color[] OctreePaletteExtraction(Image image, int maxColors)
        {
            if (image == null || maxColors <= 0)
            {
                return new Color[0];
            }

            // Call the proven PaletteExtraction method directly
            return ColorOctree.Quantize(image, maxColors);
        }

        public static Color[] MedianCutPaletteExtraction(Image image, int maxColors)
        {
            if (image == null || maxColors <= 0)
            {
                return new Color[0];
            }

            int width = image.GetWidth();
            int height = image.GetHeight();
            List<Color> allColors = new List<Color>();

            // 1. Extract all non-transparent pixels from the image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0.0f)
                    {
                        allColors.Add(pixel);
                    }
                }
            }

            if (allColors.Count == 0) 
            {
                return new Color[0];
            }

            // 2. Create our first giant ColorBox containing all pixels
            ColorBox mainBox = new ColorBox();
            foreach (Color color in allColors)
            {
                mainBox.Colors.Add(color);
            }

            // 3. Create a list to hold our boxes and add the main box
            List<ColorBox> boxes = new List<ColorBox>();
            boxes.Add(mainBox);

            // 4. Split the widest box until we reach the target color count
            while (boxes.Count < maxColors)
            {
                int bestIndex = -1;
                float maxRange = -1.0f;

                // Find the box with the widest color range
                for (int i = 0; i < boxes.Count; i++)
                {
                    float rRange = boxes[i].GetRange(RGBChannels.Red);
                    float gRange = boxes[i].GetRange(RGBChannels.Green);
                    float bRange = boxes[i].GetRange(RGBChannels.Blue);
                    
                    float currentMax = Mathf.Max(rRange, Mathf.Max(gRange, bRange));

                    if (currentMax > maxRange)
                    {
                        maxRange = currentMax;
                        bestIndex = i;
                    }
                }

                // If the max range is 0, all colors are identical. Stop splitting.
                if (maxRange <= 0.0f || bestIndex == -1) 
                {
                    break;
                }

                // Split the chosen box and add the new half to our list
                ColorBox newBox = boxes[bestIndex].Split();
                boxes.Add(newBox);
            }

            // 5. Calculate the average color for each box to build the final palette
            Color[] palette = new Color[boxes.Count];
            for (int i = 0; i < boxes.Count; i++)
            {
                float totalR = 0.0f;
                float totalG = 0.0f;
                float totalB = 0.0f;
                int count = boxes[i].Colors.Count;

                foreach (Color c in boxes[i].Colors)
                {
                    totalR += c.R;
                    totalG += c.G;
                    totalB += c.B;
                }

                palette[i] = new Color(totalR / count, totalG / count, totalB / count);
            }

            return palette;
        }

        public static Color[] KMeansPaletteExtraction(Image image, int maxColors)
        {
            if (image == null || maxColors <= 0)
            {
                return new Color[0];
            }

            int width = image.GetWidth();
            int height = image.GetHeight();
            List<Color> allColors = new List<Color>();

            // 1. Extract all non-transparent pixels from the image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0.0f)
                    {
                        allColors.Add(pixel);
                    }
                }
            }

            if (allColors.Count == 0)
            {
                return new Color[0];
            }

            // 2. Initialize K random centroids
            List<Color> centroids = new List<Color>();
            Random random = new Random();

            for (int i = 0; i < maxColors; i++)
            {
                int randomIndex = random.Next(allColors.Count);
                centroids.Add(allColors[randomIndex]);
            }

            // We will add the iteration loop in the next step!
            int maxIterations = 10;
            int pixelCount = allColors.Count;

            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                // Arrays to accumulate the new average colors
                float[] sumR = new float[maxColors];
                float[] sumG = new float[maxColors];
                float[] sumB = new float[maxColors];
                int[] count = new int[maxColors];

                // 1. Assignment Step: Find the closest centroid for each pixel
                foreach (Color pixel in allColors)
                {
                    int closestIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < maxColors; i++)
                    {
                        float dR = pixel.R - centroids[i].R;
                        float dG = pixel.G - centroids[i].G;
                        float dB = pixel.B - centroids[i].B;
                        float distance = (dR * dR) + (dG * dG) + (dB * dB);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestIndex = i;
                        }
                    }

                    sumR[closestIndex] += pixel.R;
                    sumG[closestIndex] += pixel.G;
                    sumB[closestIndex] += pixel.B;
                    count[closestIndex]++;
                }

                // 2. Update Step: Calculate new centroids
                bool changed = false;
                for (int i = 0; i < maxColors; i++)
                {
                    if (count[i] > 0)
                    {
                        Color newCentroid = new Color(sumR[i] / count[i], sumG[i] / count[i], sumB[i] / count[i]);
                        
                        // Check if the centroid moved significantly
                        float dR = centroids[i].R - newCentroid.R;
                        float dG = centroids[i].G - newCentroid.G;
                        float dB = centroids[i].B - newCentroid.B;
                        
                        if ((dR * dR) + (dG * dG) + (dB * dB) > 0.0001f)
                        {
                            changed = true;
                        }
                        centroids[i] = newCentroid;
                    }
                }

                // If centroids stopped moving, we are done!
                if (!changed) break;
            }

            return centroids.ToArray();
        }

        /// <summary>
        /// Refines an initial palette using the K-Means clustering algorithm.
        /// This is the "Hybrid" approach: using a fast algorithm (like Octree) 
        /// to get smart starting colors, and K-Means to perfect them.
        /// </summary>
        public static Color[] HybridKMeansPaletteExtraction(Image image, int maxColors, Color[] initialPalette)
        {
            if (image == null || maxColors <= 0 || initialPalette == null || initialPalette.Length == 0)
            {
                return new Color[0];
            }

            int width = image.GetWidth();
            int height = image.GetHeight();
            List<Color> allColors = new List<Color>();

            // 1. Extract all non-transparent pixels
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0.0f)
                    {
                        allColors.Add(pixel);
                    }
                }
            }

            if (allColors.Count == 0)
            {
                return initialPalette; // Return initial if image is empty
            }

            // 2. Use the provided initial palette as our starting centroids
            List<Color> centroids = new List<Color>(initialPalette);
            
            // Ensure we don't have more centroids than requested (trim if necessary)
            while (centroids.Count > maxColors)
            {
                centroids.RemoveAt(centroids.Count - 1);
            }
            
            // Ensure we have exactly maxColors centroids (duplicate the last one if we have too few)
            while (centroids.Count < maxColors)
            {
                centroids.Add(centroids.Count > 0 ? centroids[0] : new Color(0.5f, 0.5f, 0.5f));
            }

            int maxIterations = 10;

            // 3. The K-Means Optimization Loop
            for (int iteration = 0; iteration < maxIterations; iteration++)
            {
                float[] sumR = new float[maxColors];
                float[] sumG = new float[maxColors];
                float[] sumB = new float[maxColors];
                int[] count = new int[maxColors];

                // Assignment Step
                foreach (Color pixel in allColors)
                {
                    int closestIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < maxColors; i++)
                    {
                        float dR = pixel.R - centroids[i].R;
                        float dG = pixel.G - centroids[i].G;
                        float dB = pixel.B - centroids[i].B;
                        float distance = (dR * dR) + (dG * dG) + (dB * dB);

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            closestIndex = i;
                        }
                    }

                    sumR[closestIndex] += pixel.R;
                    sumG[closestIndex] += pixel.G;
                    sumB[closestIndex] += pixel.B;
                    count[closestIndex]++;
                }

                // Update Step
                bool changed = false;
                for (int i = 0; i < maxColors; i++)
                {
                    if (count[i] > 0)
                    {
                        Color newCentroid = new Color(sumR[i] / count[i], sumG[i] / count[i], sumB[i] / count[i]);
                        
                        float dR = centroids[i].R - newCentroid.R;
                        float dG = centroids[i].G - newCentroid.G;
                        float dB = centroids[i].B - newCentroid.B;
                        
                        if ((dR * dR) + (dG * dG) + (dB * dB) > 0.0001f)
                        {
                            changed = true;
                        }
                        centroids[i] = newCentroid;
                    }
                }

                if (!changed) break;
            }

            return centroids.ToArray();
        }
#endregion
    }   
}