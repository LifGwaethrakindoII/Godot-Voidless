using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class ImageViewerControl : ControlModule
    {
        [Export] private TextureRect sourceDisplay;
        [Export] private TextureRect processedDisplay;
        private Image sourceImage;
        private Image processedImage;

        public Image SourceImage
        {
            get { return sourceImage; }
            set { sourceImage = value; }    
        }

        public Image ProcessedImage
        {
            get { return processedImage; }
            set { processedImage = value; }    
        }

        public bool HasSource { get { return sourceDisplay.Texture != null; } }

        public bool HasProcessed { get { return processedDisplay.Texture != null; } }

        public void SetValues(Image sourceImage, Image processedImage = null)
        {
            SourceImage = sourceImage;
            ProcessedImage = processedImage;

            if(sourceImage != null) sourceDisplay.Texture = ImageTexture.CreateFromImage(sourceImage);
            if(processedImage != null) processedDisplay.Texture = ImageTexture.CreateFromImage(processedImage);
        }

        public void UpdateProcessedDisplay()
        {
            processedDisplay.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            processedDisplay.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        }

        public Image GetDuplicateSourceImage()
        {
            return HasSource ? sourceImage.Duplicate() as Image : null;
        }

        public Image GetDuplicateProcessedImage()
        {
            return HasProcessed ? processedImage.Duplicate() as Image : null;
        }
    }
}