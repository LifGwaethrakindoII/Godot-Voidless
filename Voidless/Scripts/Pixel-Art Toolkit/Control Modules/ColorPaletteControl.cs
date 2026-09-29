using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public enum PaletteExport { Extracted, Custom }

    public partial class ColorPaletteControl : ControlModule
    {
        [Export] private FileDialog fileDialog;
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
        [Export] private OptionButton extractionMethodDropdown;
        [ExportGroup("Buttons")]
        [Export] private Button paletteLimitButton8;
        [Export] private Button paletteLimitButton16;
        [Export] private Button paletteLimitButton32;
        [Export] private Button paletteLimitButton64;
        [Export] private Button paletteLimitButton128;
        [Export] private Button paletteLimitButton256;
        [Export] private Button exportExtractedButton;
        [Export] private Button addPaletteButton;
        [Export] private Button importPaletteButton;
        [Export] private Button exportPaletteButton;
        private Color[] palette;
        private ColorPaletteType paletteType;
        private ColorExtractionMethod colorExtractionMethod;
        private Image image;
        private string title;
        private string currentDir;
        private string[] filters;
        private PaletteExport paletteExport;

        public Color[] Palette { get { return palette; } }

        public Image Image
        {
            get { return image; }
            set { image = value; }
        }

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string CurrentDir
        {
            get { return currentDir; }
            set { currentDir = value; }
        }

        public string[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public int MaxColors { get { return (int)maxColorsSpinBox.Value; } }

        public override void _Ready()
        {
            maxColorsSpinBox.SetupIntRange(0, Constants.MAX_PALETTECOLORS);

            fileDialog.FileSelected += OnLoadFileDialogFileSelected;
            maxColorsSpinBox.ValueChanged += OnPaletteLimitSpinBoxValueChanged;
            colorPaletteTypeDropdown.ItemSelected += OnColorPaletteTypeOptionSelected;
            definedColorPaletteDropdown.ItemSelected += OnDefinedPaletteOptionSelected;
            extractionMethodDropdown.ItemSelected += OnExtractionMethodOptionSelected;
            paletteLimitButton8.Pressed += ()=> OnPaletteLimitButtonPressed(8);
            paletteLimitButton16.Pressed += ()=> OnPaletteLimitButtonPressed(16);
            paletteLimitButton32.Pressed += ()=> OnPaletteLimitButtonPressed(32);
            paletteLimitButton64.Pressed += ()=> OnPaletteLimitButtonPressed(64);
            paletteLimitButton128.Pressed += ()=> OnPaletteLimitButtonPressed(128);
            paletteLimitButton256.Pressed += ()=> OnPaletteLimitButtonPressed(256);
            colorPicker.PresetAdded += OnColorPresetAdded;
            exportExtractedButton.Pressed += OnExportExtractedButtonPressed;
            addPaletteButton.Pressed += OnAddPaletteButtonPressed;
            importPaletteButton.Pressed += OnImportPaletteButtonPressed;
            exportPaletteButton.Pressed += OnExportPaletteButtonPressed;
        }

        public void SetValues(ColorExtractionMethod colorExtractionMethod, ColorPaletteType paletteType, string title, string currentDir, string[] filters, bool sendSignal = false)
        {
            int extractionIndex = (int)colorExtractionMethod;
            int paletteIndex = (int) paletteType;

            extractionMethodDropdown.Selected = extractionIndex;
            colorPaletteTypeDropdown.Selected = paletteIndex;
            Title = title;
            CurrentDir = currentDir;
            Filters = filters;

            if(!sendSignal) return;

            extractionMethodDropdown.EmitSignal(OptionButton.SignalName.ItemSelected, extractionIndex);
            colorPaletteTypeDropdown.EmitSignal(OptionButton.SignalName.ItemSelected, paletteIndex);
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

            if(palette.IsNullOrEmpty()) return;

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

            VCanvasItem.SetMultipleVisible(false, autoExtractPaletteContainer, definedPresetPaletteContainer, customPresetPaletteContainer, colorPicker);

            switch(paletteType)
            {
                case ColorPaletteType.AutoExtract:
                    autoExtractPaletteContainer.Visible = true;
                    OnAutoExtractPaletteOptionSelected();
                break;

                case ColorPaletteType.DefinedPreset:
                    definedPresetPaletteContainer.Visible = true;
                    OnDefinedPaletteOptionSelected(definedColorPaletteDropdown.Selected);
                break;

                case ColorPaletteType.CustomPreset:
                    customPresetPaletteContainer.Visible = true;
                    colorPicker.Visible = true;
                    OnCustomPaletteOptionSelected();
                break;
            }
        }

        private void OnDefinedPaletteOptionSelected(long selection)
        {
            ColorPalettePreset preset = (ColorPalettePreset)selection;
            palette = VColor.GetColorPalette(preset);
            CreateSwatches(palette);
        }

        private void OnCustomPaletteOptionSelected()
        {
            palette = colorPicker.GetPresets();
            CreateSwatches(palette);
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

        private void OnColorPresetAdded(Color color)
        {
            OnCustomPaletteOptionSelected();
        }

        private void OnAutoExtractPaletteOptionSelected()
        {
            //if(Image == null) return;

            CreateAutoExtractionPalette();
            CreateSwatches(palette);
        }

        private void OnLoadFileDialogFileSelected(string path)
        {
            switch(fileDialog.FileMode)
            {
                case FileMode.OpenFile:
                {
                    Color[] newPalette = SaveSystem.LoadColorPalette(path);

                    if(newPalette.IsNullOrEmpty()) return;

                    palette = newPalette;
                    CreateSwatches(palette);
                }
                break;

                case FileMode.SaveFile:
                    if(palette == null) return;
                    
                    SaveSystem.SaveColorPalette(palette, path);
                    CreateSwatches(palette);
                break;
            }
        }

/*==========================================================================
|       Button Callbacks:                                                  |
==========================================================================*/
        private void OnExportExtractedButtonPressed()
        {
            if(palette.IsNullOrEmpty())
            {
                InvokeErrorSignal("Extract a Color-Palette to export.");
                return;
            }
            paletteExport = PaletteExport.Extracted;
            fileDialog.OpenInSaveMode
            (
                "Save Extracted Color-Palette",
                CurrentDir,
                "new-extracted-palette.tres",
                Constants.RATIO_FILEDIALOG,
                Constants.FILTERS_COLORPALETTES
            );
        }

        private void OnAddPaletteButtonPressed()
        {

        }

        private void OnImportPaletteButtonPressed()
        {
            fileDialog.OpenInLoadMode(Title, CurrentDir, Constants.RATIO_FILEDIALOG, Filters);
        }

        private void OnExportPaletteButtonPressed()
        {

        }
#endregion
    }
}