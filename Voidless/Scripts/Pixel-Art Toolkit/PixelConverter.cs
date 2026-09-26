using Godot;
using System;
using System.IO;
using Voidless.UI;
using System.Reflection;

using Range = Godot.Range;

namespace Voidless.PixelArtToolkit
{
    public enum DitheringType { None, Ordered, Floyd_S }

    public enum ColorPaletteType { AutoExtract, DefinedPreset, CustomPreset }

    // Hybrid = Octree + K-Means
    public enum ColorExtractionMethod { Octree, MedianCut, KMeans, Hybrid }

    public partial class PixelConverter : Control
    {
        private static readonly string[] FILTERS_IMAGES;
        private static readonly string[] FILTERS_COLORPALETTES;

        private const string SECTION_PATH = "Path";
        private const string PATH_SETTINGS = "user://settings.cfg";
        private const string KEY_LASTPATH = "LastPath";
        private const string VARIANT_LASTPATH = "C:/My/Images";
        private const float THRESHOLD_LANDSCAPE = 1.2f;
        private const float MIN_RANGEVALUE = -100.0f;
        private const float MAX_RANGEVALUE = 100.0f;
        private const float DIMENSION_SWATCH = 24.0f;
        private const int MAX_PALETTECOLORS = 256;

        [ExportCategory("Controls:")]
        [Export] private EnhancementsControl enhancementsControl;
        [ExportCategory("UI Containers:")]
        [Export] private MarginContainer appMargins;
        [Export] private BoxContainer baseLayout;
        [Export] private Panel canvasPanel;
        [Export] private PanelContainer ditheringStrengthContainer;
        [Export] private PanelContainer autoExtractPaletteContainer;
        [Export] private PanelContainer definedPresetPaletteContainer;
        [Export] private PanelContainer customPresetPaletteContainer;
        [Export] private HFlowContainer definedPaletteFlowContainer;
        [Export] private ScrollContainer controlsScroll;
        [Export] private TextureRect sourceDisplay;
        [Export] private TextureRect processedDisplay;
        [ExportGroup("UI Interactables:")]
        [Export] private DisplayDialogue popUp;
        [Export] private FileDialog loadImageFileDialog;
        [ExportSubgroup("Buttons:")]
        [Export] private Button loadImageButton;
        [Export] private Button processButton;
        [Export] private Button squareDimensionButton8;
        [Export] private Button squareDimensionButton16;
        [Export] private Button squareDimensionButton32;
        [Export] private Button squareDimensionButton64;
        [Export] private Button squareDimensionButton128;
        [Export] private Button squareDimensionButton256;
        [Export] private Button paletteLimitButton8;
        [Export] private Button paletteLimitButton16;
        [Export] private Button paletteLimitButton32;
        [Export] private Button paletteLimitButton64;
        [Export] private Button paletteLimitButton128;
        [Export] private Button paletteLimitButton256;
        [ExportSubgroup("Sliders:")]
        [Export] private HSlider contrastSlider;
        [Export] private HSlider brightnessSlider;
        [Export] private HSlider ditheringStrengthSlider;
        [ExportSubgroup("Spin-Boxes:")]
        [Export] private SpinBox contrastSpinBox;
        [Export] private SpinBox brightnessSpinBox;
        [Export] private SpinBox widthSpinBox;
        [Export] private SpinBox heightSpinBox;
        [Export] private SpinBox ditheringStrenthSpinBox;
        [Export] private SpinBox paletteColorLimitSpinBox;
        [ExportSubgroup("Dropdowns:")]
        [Export] private OptionButton ditheringTypeDropdown;
        [Export] private OptionButton colorPaletteTypeDropdown;
        [Export] private OptionButton definedColorPaletteDropdown;
        [Export] private OptionButton customColorPaletteDropdown;
        [Export] private OptionButton extractionMethodDropdown;
        [ExportSubgroup("Checkboxes/Toggles:")]
        [Export] private CheckBox proportionalEditingCheckBox;
        [Export] private CheckButton showSourceCheck;
        private Image sourceImage;
        private Image processedImage;
        private ConfigFile configFile;
        private Settings settings;
        private Color[] palette;
        private ColorPalette CGAColorPalette;
        private ColorPalette EGAColorPalette;
        private ColorPalette NESColorPalette;
        private ColorPalette GameBoyColorPalette;
        private ColorPalette Commodore64ColorPalette;
        private ColorPalette ZXSpectrumColorPalette;
        private ColorPalette PICO8ColorPalette;
        private ColorPalette Sweetie16ColorPalette;
        private ColorPalette Endesga32ColorPalette;
        private ColorPalette DB32ColorPalette;
        private ColorPalette GrayscaleColorPalette;
        private ColorPalette OneBitColorPalette;
        private float contrast;
        private float brightness;
        private int width;
        private int height;
        private bool proportionalEditing;
        private bool isLandscape;
        private bool hasProcessed;

        static PixelConverter()
        {
            FILTERS_IMAGES = new string[] { "*.png", "*.jpg", "*.jprg" };
            FILTERS_COLORPALETTES = new string[] { "*.tres", "*.res" };
        }

        private void GenerateColorPaletteResources()
        {
            CGAColorPalette = VColorPalette.Create(VColor.PALETTE_CGA);
            EGAColorPalette = VColorPalette.Create(VColor.PALETTE_EGA);
            NESColorPalette = VColorPalette.Create(VColor.PALETTE_NES);
            GameBoyColorPalette = VColorPalette.Create(VColor.PALETTE_GAMEBOY);
            Commodore64ColorPalette = VColorPalette.Create(VColor.PALETTE_COMMODORE64);
            ZXSpectrumColorPalette = VColorPalette.Create(VColor.PALETTE_ZX_SPECTRUM);
            PICO8ColorPalette = VColorPalette.Create(VColor.PALETTE_PICO8);
            Sweetie16ColorPalette = VColorPalette.Create(VColor.PALETTE_SWEETIE16);
            Endesga32ColorPalette = VColorPalette.Create(VColor.PALETTE_ENDESGA32);
            DB32ColorPalette = VColorPalette.Create(VColor.PALETTE_DB32);
            GrayscaleColorPalette = VColorPalette.Create(VColor.PALETTE_GRAYSCALE_8);
            OneBitColorPalette = VColorPalette.Create(VColor.PALETTE_1BIT);
        }

        private void TryGenerateColorPaletteResource()
        {
            
        }

        public override void _Ready()
        {
            configFile = new ConfigFile();
            settings = new Settings();
            popUp.Visible = false;
            /*SetupFloatRange(contrastSlider);
            SetupFloatRange(brightnessSlider);
            SetupFloatRange(contrastSpinBox);
            SetupFloatRange(brightnessSpinBox);*/
            SetupFloatRange(widthSpinBox, 0.0f, Mathf.Inf);
            SetupFloatRange(heightSpinBox, 0.0f, Mathf.Inf);
            SetupFloatRange(ditheringStrengthSlider, 0.0f);
            SetupFloatRange(ditheringStrenthSpinBox, 0.0f);
            SetupIntRange(paletteColorLimitSpinBox);

            GetTree().Root.SizeChanged += OnViewportResized;
            loadImageButton.Pressed += OnLoadImagePressed;
            processButton.Pressed += OnProcessPressed;
            loadImageFileDialog.FileSelected += OnImageFileSelected;
            showSourceCheck.Toggled += OnPreviewToggled;
            /*contrastSlider.ValueChanged += OnContrastSliderValueChanged;
            brightnessSlider.ValueChanged += OnBrightnessSliderValueChanged;*/
            ditheringStrengthSlider.ValueChanged += OnDitheringSliderValueChanged;
            /*brightnessSpinBox.ValueChanged += OnBrightnessSpinBoxValueChanged;
            contrastSpinBox.ValueChanged += OnContrastSpinBoxValueChanged;*/
            widthSpinBox.ValueChanged += OnWidthSpinBoxValueChanged;
            heightSpinBox.ValueChanged += OnHeightSpinBoxValueChanged;
            ditheringStrenthSpinBox.ValueChanged += OnDitheringSpinBoxValueChanged;
            paletteColorLimitSpinBox.ValueChanged += OnPaletteLimitSpinBoxValueChanged;
            proportionalEditingCheckBox.Toggled += OnProportionalEditingCheckBoxToggled;
            ditheringTypeDropdown.ItemSelected += OnDitheringTypeOptionSelected;
            colorPaletteTypeDropdown.ItemSelected += OnColorPaletteTypeOptionSelected;
            definedColorPaletteDropdown.ItemSelected += OnDefinedPaletteOptionSelected;
            customColorPaletteDropdown.ItemSelected += OnCustomPaletteOptionSelected;
            extractionMethodDropdown.ItemSelected += OnExtractionMethodOptionSelected;
            squareDimensionButton8.Pressed += ()=> OnSquareDimensionButtonPressed(8);
            squareDimensionButton16.Pressed += ()=> OnSquareDimensionButtonPressed(16);
            squareDimensionButton32.Pressed += ()=> OnSquareDimensionButtonPressed(32);
            squareDimensionButton64.Pressed += ()=> OnSquareDimensionButtonPressed(64);
            squareDimensionButton128.Pressed += ()=> OnSquareDimensionButtonPressed(128);
            squareDimensionButton256.Pressed += ()=> OnSquareDimensionButtonPressed(256);
            paletteLimitButton8.Pressed += ()=> OnPaletteLimitButtonPressed(8);
            paletteLimitButton16.Pressed += ()=> OnPaletteLimitButtonPressed(16);
            paletteLimitButton32.Pressed += ()=> OnPaletteLimitButtonPressed(32);
            paletteLimitButton64.Pressed += ()=> OnPaletteLimitButtonPressed(64);
            paletteLimitButton128.Pressed += ()=> OnPaletteLimitButtonPressed(128);
            paletteLimitButton256.Pressed += ()=> OnPaletteLimitButtonPressed(256);

            loadImageFileDialog.Access = FileDialog.AccessEnum.Filesystem;
            loadImageFileDialog.FileMode = FileDialog.FileModeEnum.OpenFile; 
            UpdateLayout();
            GenerateColorPaletteResources();
        }

        private void UpdateLayout()
        {
            Vector2 size = GetViewportRect().Size;
            isLandscape = size.X > (size.Y * THRESHOLD_LANDSCAPE);

            if(isLandscape) RebuildLayout();
        }

        private void UpdateDisplays()
        {
            if(sourceImage != null) sourceDisplay.Texture = ImageTexture.CreateFromImage(sourceImage);
            if(processedImage != null) processedDisplay.Texture = ImageTexture.CreateFromImage(processedImage);
        }

        private void RebuildLayout()
        {
            if(baseLayout == null || appMargins == null || canvasPanel == null || controlsScroll == null) return;

            // FIX 1: Ensure appMargins expands to fill the window boundaries
            appMargins.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            appMargins.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            BoxContainer newLayout = isLandscape ? new HBoxContainer() : new VBoxContainer();
            GD.Print("[Voidless] Switched to " + (isLandscape ? "LANDSCAPE (PC) " : "PORTRAIT (Mobile) " + "layout."));

            newLayout.Name = "BaseLayout";
            newLayout.AddThemeConstantOverride("separation", 16);

            // Ensure the new layout also fills the margins
            newLayout.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            newLayout.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

            canvasPanel.Reparent(newLayout);
            controlsScroll.Reparent(newLayout);

            appMargins.AddChild(newLayout);
            baseLayout.QueueFree();

            ApplySizeFlags(canvasPanel, controlsScroll);
            ApplyPremiumFeatures();
        }

        private void ApplyPremiumFeatures()
        {
            switch(isLandscape)
            {
                case true:
                    sourceDisplay.Visible = true;
                    processedDisplay.Visible = true;
                    showSourceCheck.Disabled = true;
                    showSourceCheck.Text = "Preview Original (PC: Always Visible)";
                break;

                case false:
                    controlsScroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                    controlsScroll.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
                    controlsScroll.CustomMinimumSize = new Vector2(0, 0); 

                    showSourceCheck.Disabled = !hasProcessed;
                    showSourceCheck.Text = "Preview Original";
                    sourceDisplay.Visible = showSourceCheck.ButtonPressed;
                    processedDisplay.Visible = !showSourceCheck.ButtonPressed;
                break;
            }
        }

        private void ApplySizeFlags(Panel canvas, ScrollContainer scroll)
        {
            canvas.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
            canvas.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
            canvas.CustomMinimumSize = new Vector2(0, 0); // Allow it to shrink if needed

            switch(isLandscape)
            {
                case true:
                    // FIX 2: Use ShrinkBegin so it doesn't try to expand, and disable horizontal scrolling
                    scroll.SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin; 
                    scroll.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
                    scroll.CustomMinimumSize = new Vector2(260, 0);
                    
                    // Disable the horizontal scrollbar entirely so it never overlaps
                    scroll.HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled; 
                break;

                case false:
                    scroll.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
                    scroll.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
                    scroll.CustomMinimumSize = new Vector2(0, 0); 
                break;
            }
        }

        private void DisplayError(string message)
        {
            popUp.ShowDialog("Error!", message, null, popUp.HideDialog);
        }

        private void SetupFloatRange(Range range, float min = MIN_RANGEVALUE, float max = MAX_RANGEVALUE)
        {
            range.MinValue = min;
            range.MaxValue = max;
        }

        private void SetupIntRange(Range range, int min = 0, int max = 256)
        {
            range.MinValue = min;
            range.MaxValue = max;
        }

        private void CreateAutoExtractionPalette()
        {
            switch(settings.ColorExtractionMethod)
            {
                case ColorExtractionMethod.Octree:
                    palette = VColor.OctreePaletteExtraction(sourceImage, settings.MaxColors);
                break;

                case ColorExtractionMethod.MedianCut:
                    palette = VColor.MedianCutPaletteExtraction(sourceImage, settings.MaxColors);
                break;

                case ColorExtractionMethod.KMeans:
                    palette = VColor.KMeansPaletteExtraction(sourceImage, settings.MaxColors);
                break;

                case ColorExtractionMethod.Hybrid:
                    palette = VColor.HybridKMeansPaletteExtraction(
                        sourceImage,
                        settings.MaxColors,
                        VColor.OctreePaletteExtraction(sourceImage, settings.MaxColors)
                    );
                break;
            }
        }

        private void CreateSwatches(params Color[] palette)
        {
            definedPaletteFlowContainer.QueueFreeChildren();

            foreach (Color color in palette)
            {
                ColorRect swatch = new ColorRect();
                swatch.Color = color;
                swatch.CustomMinimumSize = new Vector2(DIMENSION_SWATCH, DIMENSION_SWATCH); 
                definedPaletteFlowContainer.AddChild(swatch);
            }
        }

#region Callbacks:
        private void OnViewportResized()
        {
            UpdateLayout();
        }
#endregion

#region UICallbacks:
/*==========================================================================
|       Button Callbacks:                                                  |
==========================================================================*/
        private void OnLoadImagePressed()
        {
            configFile.Load(PATH_SETTINGS);

            loadImageFileDialog.CurrentDir = configFile.GetValue(SECTION_PATH, KEY_LASTPATH, VARIANT_LASTPATH).As<string>();
            loadImageFileDialog.PopupCenteredRatio(0.8f);
        }

        private void OnProcessPressed()
        {
            if(sourceImage == null)
            {
                DisplayError("Please load an image first!");
                return;
            }

            processedImage = sourceImage.Duplicate() as Image;
            processedImage = processedImage.NearestNeighborScale(width, height);
            processedImage.ApplyModifications
            (
                img => VColor.Brightness(img, enhancementsControl.Brightness),
                img => VColor.Contrast(img, enhancementsControl.Contrast)
            );
            //processedImage = VImage.ApplyPalette(processedImage, VColor.PALETTE_GAMEBOY);
            float s = settings.DitheringStrength / MAX_RANGEVALUE;

            switch(settings.DitheringType)
            {
                case DitheringType.None:
                    processedImage.ApplyPalette(palette);
                break;

                case DitheringType.Ordered:
                    processedImage.ApplyOrderedDithering(s, palette);
                break;

                case DitheringType.Floyd_S:
                    processedImage.ApplyFloydSteinberg(s, palette);
                break;
            }
            processedDisplay.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            processedDisplay.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            hasProcessed = true;
            showSourceCheck.Disabled = false;
            UpdateDisplays();
        }

        private void OnImageFileSelected(string path)
        {
            GD.Print("Loading image from: " + path);

            sourceImage = new Image();
            Error error = sourceImage.Load(path);

            if(error != Error.Ok)
            {
                DisplayError("Falied to load image. Error code: " + error);
                return;
            }

            processedImage = null;
            hasProcessed = false;
            showSourceCheck.ButtonPressed = false;
            showSourceCheck.Disabled = true;
            UpdateDisplays();
            /*ImageTexture texture = ImageTexture.CreateFromImage(sourceImage);
            sourceDisplay.Texture = texture;*/

            GD.Print("Image loaded successfully with size: " + sourceImage.GetSize());
            configFile.SetValue(SECTION_PATH, KEY_LASTPATH, Path.GetDirectoryName(path)); // "Path", "LastPath", ...
            configFile.Save(PATH_SETTINGS);
        }

        private void OnSquareDimensionButtonPressed(int dimension)
        {
            widthSpinBox.SetValue(dimension);
            heightSpinBox.SetValue(dimension);
        }

        private void OnPaletteLimitButtonPressed(int limit)
        {
            paletteColorLimitSpinBox.SetValueNoSignal(limit);
            settings.MaxColors = limit;
            OnAutoExtractPaletteOptionSelected();
        }

/*==========================================================================
|       Toggle Callbacks:                                                  |
==========================================================================*/
        private void OnPreviewToggled(bool toggled)
        {
            if(isLandscape) return;

            sourceDisplay.Visible = !toggled;
            processedDisplay.Visible = toggled;
        }

        private void OnProportionalEditingCheckBoxToggled(bool toggled)
        {
            proportionalEditing = toggled;

            if(toggled)
            {
                float max = Mathf.Max(width, height);
                int iMax = (int)max;

                widthSpinBox.SetValueNoSignal(max);
                heightSpinBox.SetValueNoSignal(max);
                width = iMax;
                height = iMax;
            }
        }

/*==========================================================================
|       Slider Callbacks:                                                  |
==========================================================================*/
        private void OnContrastSliderValueChanged(double value)
        {
            contrastSpinBox.SetValueNoSignal(value);
            contrast = (float)value;
        }

        private void OnBrightnessSliderValueChanged(double value)
        {
            brightnessSpinBox.SetValueNoSignal(value);
            brightness = (float)value;
        }

        private void OnDitheringSliderValueChanged(double value)
        {
            ditheringStrenthSpinBox.SetValueNoSignal(value);
            settings.DitheringStrength = (float)value;
        }

/*==========================================================================
|       SpinBox Callbacks:                                                 |
==========================================================================*/
        private void OnBrightnessSpinBoxValueChanged(double value)
        {
            brightnessSlider.SetValueNoSignal(value);
            brightness = (float)value;
        }

        private void OnContrastSpinBoxValueChanged(double value)
        {
            contrastSlider.SetValueNoSignal(value);
            contrast = (float)value;
        }

        private void OnWidthSpinBoxValueChanged(double value)
        {
            width = (int)value;
            if(proportionalEditing) heightSpinBox.SetValue(value);
        }

        private void OnHeightSpinBoxValueChanged(double value)
        {
            height = (int)value;
            if(proportionalEditing) widthSpinBox.SetValue(value);
        }

        private void OnDitheringSpinBoxValueChanged(double value)
        {
            settings.DitheringStrength = (float)value;
            ditheringStrengthSlider.SetValueNoSignal(value);
        }

        private void OnPaletteLimitSpinBoxValueChanged(double value)
        {
            settings.MaxColors = (int)value;
            OnAutoExtractPaletteOptionSelected();
        }

/*==========================================================================
|       Option-Button Callbacks:                                           |
==========================================================================*/
        private void OnDitheringTypeOptionSelected(long selection)
        {
            settings.DitheringType = (DitheringType)selection;
        }

        private void OnColorPaletteTypeOptionSelected(long selection)
        {
            settings.PaletteType = (ColorPaletteType)selection;

            VCanvasItem.SetMultipleVisible(false, autoExtractPaletteContainer, definedPresetPaletteContainer, customPresetPaletteContainer);

            switch(settings.PaletteType)
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

        private void OnAutoExtractPaletteOptionSelected()
        {
            if(sourceImage == null) return;

            CreateAutoExtractionPalette();
            CreateSwatches(palette);
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
            ColorExtractionMethod extractionMethod = (ColorExtractionMethod)selection;
            settings.ColorExtractionMethod = extractionMethod;

            CreateAutoExtractionPalette();
            CreateSwatches(palette);
        }
#endregion
    }
}