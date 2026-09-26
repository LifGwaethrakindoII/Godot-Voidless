using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Voidless
{
    // TODO: Re-do the structure. Taking into account:
    /*
        - Boundary
        - Limits
        - Children
        - Etc.
    */
    public class ColorOctree
    {
        public static Color[] Quantize(Image image, int maxColors)
        {
            if (image == null || maxColors <= 0)
                return new Color[0];

            int width = image.GetWidth();
            int height = image.GetHeight();
            
            // Simple dictionary to count color occurrences
            var colorCounts = new Dictionary<ColorKey, ColorData>();
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    if (pixel.A > 0.0f)
                    {
                        // Quantize to 5-bit color to reduce uniqueness
                        ColorKey key = new ColorKey(
                            (byte)(pixel.R * 31),
                            (byte)(pixel.G * 31),
                            (byte)(pixel.B * 31)
                        );
                        
                        if (!colorCounts.ContainsKey(key))
                            colorCounts[key] = new ColorData();
                        
                        colorCounts[key].Count++;
                        colorCounts[key].TotalR += pixel.R;
                        colorCounts[key].TotalG += pixel.G;
                        colorCounts[key].TotalB += pixel.B;
                    }
                }
            }
            
            // Sort by frequency and take top N
            var sorted = colorCounts.OrderByDescending(kvp => kvp.Value.Count)
                                    .Take(maxColors)
                                    .Select(kvp => new Color(
                                        kvp.Value.TotalR / kvp.Value.Count,
                                        kvp.Value.TotalG / kvp.Value.Count,
                                        kvp.Value.TotalB / kvp.Value.Count
                                    ))
                                    .ToArray();
            
            return sorted;
        }
        
        private struct ColorKey : IEquatable<ColorKey>
        {
            public byte R, G, B;
            
            public ColorKey(byte r, byte g, byte b)
            {
                R = r;
                G = g;
                B = b;
            }
            
            public bool Equals(ColorKey other)
            {
                return R == other.R && G == other.G && B == other.B;
            }
            
            public override bool Equals(object obj)
            {
                return obj is ColorKey other && Equals(other);
            }
            
            public override int GetHashCode()
            {
                return (R << 16) | (G << 8) | B;
            }
        }
        
        private class ColorData
        {
            public int Count = 0;
            public float TotalR = 0, TotalG = 0, TotalB = 0;
        }
    }
}