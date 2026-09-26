using Godot;
using System;
using System.Collections.Generic;

namespace Voidless
{
    public class ColorBox
    {
        private List<Color> colors;

        public List<Color> Colors
        {
            get { return colors; }
        }

        public ColorBox()
        {
            colors = new List<Color>();
        }

        public float GetRange(RGBChannels channel)
        {
            if(channel == RGBChannels.None) return 0.0f;

            float min = 1.0f;
            float max = 0.0f;

            foreach (Color color in colors)
            {
                float value = 0.0f;

                if((channel | RGBChannels.Red) == channel) value = color.R;
                else if((channel | RGBChannels.Green) == channel) value = color.G;
                else if((channel | RGBChannels.Blue) == channel) value = color.B;

                if (value < min) min = value;
                if (value > max) max = value;
            }

            return max - min;
        }

        public void SortByChannel(RGBChannels channel)
        {
            colors.Sort((Color a, Color b) =>
            {
                float valA = 0.0f;
                float valB = 0.0f;

                if((channel | RGBChannels.Red) == channel) { valA = a.R; valB = b.R; }
                else if((channel | RGBChannels.Green) == channel) { valA = a.G; valB = b.G; }
                else if((channel | RGBChannels.Blue) == channel) { valA = a.B; valB = b.B; }

                return valA.CompareTo(valB);
            });
        }

        public ColorBox Split()
        {
            // 1. Find the channel with the widest range
            RGBChannels widestChannel = RGBChannels.Red;
            float maxRange = GetRange(RGBChannels.Red);
            
            float greenRange = GetRange(RGBChannels.Green);
            float blueRange = GetRange(RGBChannels.Blue);

            if (greenRange > maxRange) 
            { 
                maxRange = greenRange; 
                widestChannel = RGBChannels.Green; 
            }
            if (blueRange > maxRange) 
            { 
                maxRange = blueRange; 
                widestChannel = RGBChannels.Blue; 
            }

            // 2. Sort the colors by that widest channel
            SortByChannel(widestChannel);

            // 3. Find the median index (halfway point)
            int medianIndex = colors.Count / 2;

            // 4. Create a new box for the second half
            ColorBox newBox = new ColorBox();

            // Move the second half of the colors into the new box
            for (int i = medianIndex; i < colors.Count; i++)
            {
                newBox.Colors.Add(colors[i]);
            }

            // Remove the second half from the current box
            colors.RemoveRange(medianIndex, colors.Count - medianIndex);

            return newBox;
        }
    }
}