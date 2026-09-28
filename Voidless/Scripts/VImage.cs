using Godot;
using System;

namespace Voidless
{
    public static class VImage
    {
        public static readonly int[,] BAYERMATRIX_4X4;

        public const string EXTENSION_PNG = ".png";
        public const string EXTENSION_JPG = ".jpg";
        public const string EXTENSION_JPEG = ".jpeg";

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

        public static bool Equals(Image a, Image b)
        {
            if(a.GetSize() != b.GetSize() || a.GetFormat() != b.GetFormat()) return false;

            byte[] dataA = a.GetData();
            byte[] dataB = b.GetData();
            ReadOnlySpan<byte> spanA = dataA;
            ReadOnlySpan<byte> spanB = dataB;

            return spanA.SequenceEqual(spanB);
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

#region Serialization:
        public static bool SaveImageToDisk(this Image image, string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath).ToLower();
            Error saveError = Error.Ok;

            switch(extension)
            {
                case EXTENSION_PNG:
                    saveError = image.SavePng(filePath);
                break;

                case EXTENSION_JPG:
                case EXTENSION_JPEG:
                    saveError = image.SaveJpg(filePath);
                break;

                default:
                    GD.PrintErr("Unsupported image format: " + extension);
                return false;
            }

            string message = saveError != Error.Ok ? "Failed to save " + extension + " image. Error: " + saveError.ToString()
                            : "Successfuly saved " + extension + " image to: " + filePath;

            GD.Print(message);

            return saveError == Error.Ok;
        }
#endregion
    }
}