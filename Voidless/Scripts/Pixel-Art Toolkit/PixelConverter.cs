using Godot;
using System;
using System.IO;
using Voidless.UI;
using System.Reflection;

using Range = Godot.Range;

namespace Voidless.PixelArtToolkit
{
    public partial class PixelConverter : Control
    {
        [ExportCategory("Controls:")]
        [Export] private ImageViewerControl imageViewerControl;
        [Export] private ImportControl importControl;
        [Export] private EnhancementsControl enhancementsControl;
        [Export] private DimensionsControl dimensionsControl;
        [Export] private ScalingControl scalingControl;
        [Export] private DitheringControl ditheringControl;
        [Export] private ColorPaletteControl colorPaletteControl;
        [Export] private ProcessControl processControl;
        [Export] private ExportControl exportControl;
        [ExportCategory("UI Containers:")]
        [Export] private MarginContainer appMargins;
        [Export] private BoxContainer baseLayout;
        [Export] private Panel canvasPanel;
        [Export] private ScrollContainer controlsScroll;
        [ExportGroup("UI Interactables:")]
        [Export] private DisplayDialogue popUp;
        [Export] private FileDialog loadImageFileDialog;
        [ExportGroup("Checkboxes/Toggles:")]
        [Export] private CheckButton showSourceCheck;
        private ConfigFile configFile;
        private PixelConversionSettings settings;
        private bool isLandscape;

        public override void _Ready()
        {
            loadImageFileDialog.Visible = false;
            popUp.Visible = false;
            controlsScroll.ClipContents = true;
            SaveSystem.Initialize();

            GetTree().Root.SizeChanged += OnViewportResized;
            importControl.OnFileImported += OnFileImported;
            dimensionsControl.OnChanged += OnDimensionsControlChanged;
            processControl.OnPreviewToggled += OnPreviewToggled;
            processControl.OnProcessRequested += OnProcessRequested;
            exportControl.OnFileExported += OnFileExported;

            LoadConfigFile();
            SetupControls();
            UpdateLayout();
            EvaluateImageDisplays();
        }

        private void SetupControls()
        {
            string currentDir = configFile.GetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, Constants.VARIANT_LASTPATH).As<string>();
            
            importControl.SetValues(Constants.TITLE_LOADIMAGE, currentDir, Constants.FILTERS_IMAGES);
            enhancementsControl.SetValues(settings.DarkOutline, settings.Contrast, settings.Brightness);
            dimensionsControl.SetValues(settings.ProportionalEditing, settings.Width, settings.Height);
            scalingControl.SetValues(settings.ScaleFactor, settings.Width, settings.Height);
            ditheringControl.SetValues(settings.DitheringType, settings.DitheringStrength);
            colorPaletteControl.SetValues(settings.ColorExtractionMethod, settings.PaletteType, "Import Color-Palette", currentDir, Constants.FILTERS_COLORPALETTES, true);
            processControl.SetValues(false);
            exportControl.SetValues(Constants.TITLE_SAVEIMAGE, currentDir, "newImage.png", Constants.FILTERS_IMAGES);
        }

        private void UpdateSettings()
        {

        }

        private void DisplayPopUp(string title, string message)
        {
            popUp.ShowDialog(title, message, null, popUp.HideDialog);
        }

        private void UpdateDisplays(Image sourceImage, Image processedImage = null)
        {
            imageViewerControl.SetValues(sourceImage, processedImage);
            EvaluateImageDisplays();
        }

        private void EvaluateImageDisplays()
        {
            bool sourceDisplayLoaded = imageViewerControl.HasSource;

            processControl.Visible = sourceDisplayLoaded;
            exportControl.Visible = (sourceDisplayLoaded && imageViewerControl.HasProcessed);
            VCanvasItem.SetMultipleVisible
            (
                sourceDisplayLoaded,
                enhancementsControl,
                dimensionsControl,
                scalingControl,
                ditheringControl,
                colorPaletteControl
            );
        }

        private void LoadConfigFile()
        {
            configFile = new ConfigFile();
            settings = PixelConversionSettings.Default();

            // Feed the ImportControl:
            Error loadError = configFile.Load(Constants.PATH_SETTINGS);
            string lastPath = configFile.GetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, string.Empty).As<string>();

            if(loadError != Error.Ok || string.IsNullOrEmpty(lastPath))
            {
                lastPath = OS.GetSystemDir(OS.SystemDir.Documents);
                configFile.SetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, lastPath);
                configFile.Save(Constants.PATH_SETTINGS);
            }

            settings.LastFilePath = lastPath;
        }

#region Callbacks:
        private void OnViewportResized()
        {
            UpdateLayout();
        }
#endregion

#region UICallbacks:
/*==========================================================================
|       Control Module Callbacks:                                          |
==========================================================================*/
        private void OnProcessRequested()
        {
            if(!imageViewerControl.HasSource)
            {
                DisplayPopUp("Error", "Please load an image first!");
                return;
            }

            Image sourceImage = imageViewerControl.SourceImage;
            Image processedImage = imageViewerControl.ProcessedImage;

            processedImage = imageViewerControl.GetDuplicateSourceImage();
            processedImage = processedImage.NearestNeighborScale(dimensionsControl.Width, dimensionsControl.Height);
            processedImage.ApplyModifications
            (
                img => VColor.Brightness(img, enhancementsControl.Brightness),
                img => VColor.Contrast(img, enhancementsControl.Contrast)
            );
            //processedImage = VImage.ApplyPalette(processedImage, VColor.PALETTE_GAMEBOY);
            float s = ditheringControl.Strength;

            Color[] palette = colorPaletteControl.Palette;

            switch(ditheringControl.DitheringType)
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
            
            imageViewerControl.UpdateProcessedDisplay();
            UpdateDisplays(sourceImage, processedImage);
        }

        private void OnFileImported(string path)
        {
            GD.Print("Loading image from: " + path);

            Image sourceImage = new Image();
            Error error = sourceImage.Load(path);

            if(error != Error.Ok)
            {
                DisplayPopUp("Error", "Falied to load image. Error code: " + error);
                return;
            }

            colorPaletteControl.Image = sourceImage;
            UpdateDisplays(sourceImage);

            string newDir = Path.GetDirectoryName(path);

            GD.Print("Image loaded successfully with size: " + sourceImage.GetSize());
            configFile.SetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, newDir); // "Path", "LastPath", ...
            configFile.Save(Constants.PATH_SETTINGS);

            importControl.CurrentDir = newDir;
            exportControl.CurrentDir = newDir;
        }

        private void OnFileExported(string path)
        {
            if(!imageViewerControl.HasProcessed)
            {
                DisplayPopUp("Error", "No Processed image (What!?), won't be able to export image.");
                return;
            }

            Vector2I size = scalingControl.ScaledDimensions;
            Image duplicate = imageViewerControl.GetDuplicateProcessedImage();
            duplicate = duplicate.NearestNeighborScale(size.X, size.Y);
            
            bool result = duplicate.SaveImageToDisk(path);

            if(result)
            {
                string newDir = Path.GetDirectoryName(path);

                configFile.SetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, newDir);
                configFile.Save(Constants.PATH_SETTINGS);

                importControl.CurrentDir = newDir;
                exportControl.CurrentDir = newDir;
            }

            DisplayPopUp(result ? "Success" : "Failure", "Saved at path: " + path + ". Successful? " + result);
        }

        private void OnDimensionsControlChanged()
        {
            scalingControl.SetValues(scalingControl.ScaleFactor, dimensionsControl.Width, dimensionsControl.Height);
        }

/*==========================================================================
|       Toggle Callbacks:                                                  |
==========================================================================*/
        private void OnPreviewToggled(bool toggled)
        {
            
        }
#endregion

#region DeprecatedLayoutFunctions:
        private void UpdateLayout()
        {
            Vector2 size = GetViewportRect().Size;
            isLandscape = size.X > (size.Y * Constants.THRESHOLD_LANDSCAPE);

            //if(isLandscape) RebuildLayout();
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
        }

        private void ApplySizeFlags(Panel canvas, ScrollContainer scroll)
        {
            if(canvas == null || scroll == null) return;

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
#endregion

    }
}