using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnFileImported(string filePath);

    public partial class ImportControl : ControlModule
    {
        public event OnFileImported OnFileImported;

        // UI References
        [Export] private Label fileInfo;
        [Export] private Button loadButton;
        [ExportCategory("Windows")]
        [Export] private FileDialog loadFileDialog;

        public override void _Ready()
        {
            loadButton.Pressed += OnLoadButtonPressed;
            loadFileDialog.FileSelected += OnLoadFileDialogFileSelected;
        }

        private void OnLoadButtonPressed()
        {
            // The UI's only job is to open the window
            loadFileDialog.PopupCentered();
        }

        private void OnLoadFileDialogFileSelected(string path)
        {
            if(OnFileImported != null) OnFileImported(path);
        }
    }
}