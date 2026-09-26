using Godot;
using System;

namespace Voidless
{
    public static class VImage
    {
        public static readonly int[,] BAYERMATRIX_4X4;

        static VImage()
        {
            BAYERMATRIX_4X4 = new int[,]
            {
                {  0,  8,  2, 10 },
                { 12,  4, 14,  6 },
                {  3, 11,  1,  9 },
                { 15,  7, 13,  5 }
            };
        }

        public static void ApplyPalette(this Image image, params Color[] palette)
        {
            if(image == null || palette == null || palette.Length == 0) return;

            int width = image.GetWidth();
            int height = image.GetHeight();

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    
                    image.SetPixel(x, y, (pixel.A > 0.0f) ? pixel.FindClosestColor(palette) : pixel);
                }
            }
        }

        public static void ApplyOrderedDithering(this Image image, float strength = 0.2f, params Color[] palette)
        {   
            if(image == null || palette == null || palette.Length == 0) return;

            image.SetPixels(VColor.OrderedDithering(image, strength, palette));
        }

        public static void ApplyFloydSteinberg(this Image image, float strength = 0.2f, params Color[] palette)
        {
            if(image == null || palette == null || palette.Length == 0) return;

            image.SetPixels(VColor.FloydSteinbergDithering(image, strength, palette));
        }

        public static Image NearestNeighborScale(this Image image, int width, int height)
        {
            if(image == null) return null;

            int w = image.GetWidth();
            int h = image.GetHeight();

            Image scaledImage = Image.CreateEmpty(width, height, false, image.GetFormat());

            // Ratio: sourceDimension / targetDimension
            float rx = (float)w / (float)width;
            float ry = (float)h / (float)height;

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    int tx = (int)(x * rx);
                    int ty = (int)(y * ry);
                    Color tp = image.GetPixel(tx, ty);
                    scaledImage.SetPixel(x, y, tp);
                }
            }

            return scaledImage;
        }

        public static void AdjustColorBrightness(this Image image, float b)
        {
            if(image == null) return;

            int width = image.GetWidth();
            int height = image.GetHeight();
            
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    image.SetPixel(x, y, VColor.Brightness(image.GetPixel(x, y), b));
                }
            }
        }

        public static void AdjustColorContrast(this Image image, float c)
        {
            if(image == null) return;

            int width = image.GetWidth();
            int height = image.GetHeight();

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    image.SetPixel(x, y, VColor.Contrast(image.GetPixel(x, y), c));
                }
            }
        }

        public static void ApplyModifications(this Image image, params Func<Color, Color>[] colorModifiers)
        {
            int width = image.GetWidth();
            int height = image.GetHeight(); 

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    foreach (Func<Color, Color> function in colorModifiers)
                    {
                        pixel = function(pixel);
                    }

                    image.SetPixel(x, y, pixel);
                }
            }
        }

        public static void SetPixels(this Image image, Color[,] colors)
        {
            int width = colors.GetLength(0);  // First dimension size
            int height = colors.GetLength(1); // Second dimension size

            // Resize the image canvas if it doesn't match the array sizes
            if (image.GetWidth() != width || image.GetHeight() != height)
            {
                image.Resize(width, height);
            }

            // Loop through the 2D array and map it to pixels
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, colors[x, y]);
                }
            }
        }
    }
}