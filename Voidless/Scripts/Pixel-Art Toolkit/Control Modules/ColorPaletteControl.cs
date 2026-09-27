using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public partial class ColorPaletteControl : ControlModule
    {
        [Export] private ColorPicker colorPicker;
        [Export] private SpinBox maxColorsSpinBox;
        [ExportGroup("Containers")]
        [Export] private PanelContainer autoExtractPaletteContainer;
        [Export] private PanelContainer definedPresetPaletteContainer;
        [Export] private PanelContainer customPresetPaletteContainer;
        [Export] private HFlowContainer definedPaletteFlowContainer;
        [ExportGroup("Dropdowns")]
        [Export] private OptionButton colorPaletteTypeDropdown;
        [Export] private OptionButton definedColorPaletteDropdown;
        [Export] private OptionButton customColorPaletteDropdown;
        [Export] private OptionButton extractionMethodDropdown;
        [ExportGroup("Buttons")]
        [Export] private Button paletteLimitButton8;
        [Export] private Button paletteLimitButton16;
        [Export] private Button paletteLimitButton32;
        [Export] private Button paletteLimitButton64;
        [Export] private Button paletteLimitButton128;
        [Export] private Button paletteLimitButton256;
        [Export] private Button loadSwatchesButton;
        [Export] private Button saveSwatchesButton;
        [Export] private Button addSwatchesButton;
        private Color[] palette;
        private ColorPaletteType paletteType;
        private ColorExtractionMethod colorExtractionMethod;
        private Image image;

        public Color[] Palette { get { return palette; } }
        
        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public int MaxColors { get { return (int)maxColorsSpinBox.Value; } }

        public override void _Ready()
        {
            maxColorsSpinBox.ValueChanged += OnPaletteLimitSpinBoxValueChanged;
            colorPaletteTypeDropdown.ItemSelected += OnColorPaletteTypeOptionSelected;
            definedColorPaletteDropdown.ItemSelected += OnDefinedPaletteOptionSelected;
            customColorPaletteDropdown.ItemSelected += OnCustomPaletteOptionSelected;
            extractionMethodDropdown.ItemSelected += OnExtractionMethodOptionSelected;
            paletteLimitButton8.Pressed += ()=> OnPaletteLimitButtonPressed(8);
            paletteLimitButton16.Pressed += ()=> OnPaletteLimitButtonPressed(16);
            paletteLimitButton32.Pressed += ()=> OnPaletteLimitButtonPressed(32);
            paletteLimitButton64.Pressed += ()=> OnPaletteLimitButtonPressed(64);
            paletteLimitButton128.Pressed += ()=> OnPaletteLimitButtonPressed(128);
            paletteLimitButton256.Pressed += ()=> OnPaletteLimitButtonPressed(256);
        }

        public void SetValues(ColorExtractionMethod colorExtractionMethod, ColorPaletteType paletteType, bool sendSignal = false)
        {
            int extractionIndex = (int)colorExtractionMethod;
            int paletteIndex = (int) paletteType;

            extractionMethodDropdown.Selected = extractionIndex;
            colorPaletteTypeDropdown.Selected = paletteIndex;

            switch(sendSignal)
            {
                case true:
                    extractionMethodDropdown.EmitSignal(OptionButton.SignalName.ItemSelected, extractionIndex);
                    colorPaletteTypeDropdown.EmitSignal(OptionButton.SignalName.ItemSelected, paletteIndex);
                break;

                case false:
                break;
            }
        }

        private void CreateAutoExtractionPalette()
        {
            if(Image == null)
            {
                InvokeErrorSignal("NullReferenceException: Image not found.");
                return;
            }

            switch(colorExtractionMethod)
            {
                case ColorExtractionMethod.Octree:
                    palette = VColor.OctreePaletteExtraction(Image, MaxColors);
                break;

                case ColorExtractionMethod.MedianCut:
                    palette = VColor.MedianCutPaletteExtraction(Image, MaxColors);
                break;

                case ColorExtractionMethod.KMeans:
                    palette = VColor.KMeansPaletteExtraction(Image, MaxColors);
                break;

                case ColorExtractionMethod.Hybrid:
                    palette = VColor.HybridKMeansPaletteExtraction(
                        Image,
                        MaxColors,
                        VColor.OctreePaletteExtraction(Image, MaxColors)
                    );
                break;
            }
        }

        private void CreateSwatches(params Color[] palette)
        {
            definedPaletteFlowContainer.QueueFreeChildren();

            float d = Constants.DIMENSION_SWATCH;

            foreach (Color color in palette)
            {
                ColorRect swatch = new ColorRect();
                swatch.Color = color;
                swatch.CustomMinimumSize = new Vector2(d, d); 
                definedPaletteFlowContainer.AddChild(swatch);
            }
        }

#region Callbacks:
        private void OnPaletteLimitSpinBoxValueChanged(double value)
        {
            OnAutoExtractPaletteOptionSelected();
        }

        private void OnColorPaletteTypeOptionSelected(long selection)
        {
            paletteType = (ColorPaletteType)selection;

            VCanvasItem.SetMultipleVisible(false, autoExtractPaletteContainer, definedPresetPaletteContainer, customPresetPaletteContainer);

            switch(paletteType)
            {
                case ColorPaletteType.AutoExtract:
                    autoExtractPaletteContainer.Visible = true;
                    OnAutoExtractPaletteOptionSelected();
                break;

                case ColorPaletteType.DefinedPreset:
                    definedPresetPaletteContainer.Visible = true;
                break;

                case ColorPaletteType.CustomPreset:
                    customPresetPaletteContainer.Visible = true;
                break;
            }
        }

        private void OnDefinedPaletteOptionSelected(long selection)
        {
            ColorPalettePreset preset = (ColorPalettePreset)selection;
            palette = VColor.GetColorPalette(preset);
            CreateSwatches(palette);
        }

        private void OnCustomPaletteOptionSelected(long selection)
        {

        }

        private void OnExtractionMethodOptionSelected(long selection)
        {
            colorExtractionMethod = (ColorExtractionMethod)selection;

            CreateAutoExtractionPalette();
            CreateSwatches(palette);
        }

        private void OnPaletteLimitButtonPressed(int limit)
        {
            maxColorsSpinBox.SetValueNoSignal(limit);
            OnAutoExtractPaletteOptionSelected();
        }

        private void OnAutoExtractPaletteOptionSelected()
        {
            //if(Image == null) return;

            CreateAutoExtractionPalette();
            CreateSwatches(palette);
        }
#endregion
    }
}