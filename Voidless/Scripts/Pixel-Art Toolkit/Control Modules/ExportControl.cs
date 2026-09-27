using Godot;
using System;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnFileExported(string filePath);

    public partial class ExportControl : ControlModule
    {
        public event OnFileExported OnFileExported;

        [Export] private Button saveButton;   
        [ExportCategory("Windows")]
        [Export] private FileDialog saveFileDialog;

        public override void _Ready()
        {
            saveButton.Pressed += OnSaveButtonPressed;
            saveFileDialog.FileSelected += OnSaveFileDialogFileSelected;
        }

        private void OnSaveButtonPressed()
        {
            saveFileDialog.PopupCentered();
        }

        private void OnSaveFileDialogFileSelected(string path)
        {
            if(OnFileExported != null) OnFileExported(path);
        }
    }
}