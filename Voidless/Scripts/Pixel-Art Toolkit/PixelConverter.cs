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
        [Export] private EnhancementsControl enhancementsControl;
        [Export] private DimensionsControl dimensionsControl;
        [Export] private DitheringControl ditheringControl;
        [Export] private ColorPaletteControl colorPaletteControl;
        [ExportCategory("UI Containers:")]
        [Export] private MarginContainer controlsPanel;
        [Export] private MarginContainer appMargins;
        [Export] private BoxContainer baseLayout;
        [Export] private Panel canvasPanel;
        [Export] private ScrollContainer controlsScroll;
        [Export] private TextureRect sourceDisplay;
        [Export] private TextureRect processedDisplay;
        [ExportGroup("UI Interactables:")]
        [Export] private DisplayDialogue popUp;
        [Export] private FileDialog loadImageFileDialog;
        [ExportGroup("Buttons:")]
        [Export] private Button loadImageButton;
        [Export] private Button processButton;
        [ExportGroup("Checkboxes/Toggles:")]
        [Export] private CheckButton showSourceCheck;
        private Image sourceImage;
        private Image processedImage;
        private ConfigFile configFile;
        private Settings settings;
        private bool isLandscape;
        private bool hasProcessed;

        public override void _Ready()
        {
            configFile = new ConfigFile();
            settings = new Settings();
            popUp.Visible = false;

            GetTree().Root.SizeChanged += OnViewportResized;
            loadImageButton.Pressed += OnLoadImagePressed;
            processButton.Pressed += OnProcessPressed;
            loadImageFileDialog.FileSelected += OnImageFileSelected;
            showSourceCheck.Toggled += OnPreviewToggled;

            loadImageFileDialog.Access = FileDialog.AccessEnum.Filesystem;
            loadImageFileDialog.FileMode = FileDialog.FileModeEnum.OpenFile; 
            UpdateLayout();
        }

        private void UpdateLayout()
        {
            Vector2 size = GetViewportRect().Size;
            isLandscape = size.X > (size.Y * Constants.THRESHOLD_LANDSCAPE);

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
            configFile.Load(Constants.PATH_SETTINGS);

            loadImageFileDialog.CurrentDir = configFile.GetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, Constants.VARIANT_LASTPATH).As<string>();
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
            colorPaletteControl.Image = sourceImage;
            UpdateDisplays();
            /*ImageTexture texture = ImageTexture.CreateFromImage(sourceImage);
            sourceDisplay.Texture = texture;*/

            GD.Print("Image loaded successfully with size: " + sourceImage.GetSize());
            configFile.SetValue(Constants.SECTION_PATH, Constants.KEY_LASTPATH, Path.GetDirectoryName(path)); // "Path", "LastPath", ...
            configFile.Save(Constants.PATH_SETTINGS);
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
#endregion
    }
}