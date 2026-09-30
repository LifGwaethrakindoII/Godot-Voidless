using Godot;
using System;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public delegate void OnFileExported(string filePath);

    public partial class ExportControl : ControlModule
    {
        public event OnFileExported OnFileExported;

        [Export] private Button saveButton;   
        [ExportCategory("Windows")]
        [Export] private FileDialog saveFileDialog;
        private string title;
        private string currentDir;
        private string defaultFileName;
        private string[] filters;

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

        public string DefaultFileName
        {
            get { return defaultFileName; }
            set { defaultFileName = value; }
        }

        public string[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public override void _Ready()
        {
            saveButton.Pressed += OnSaveButtonPressed;
            saveFileDialog.FileSelected += OnSaveFileDialogFileSelected;
        }

        public void SetValues(string title, string currentDir, string defaultFileName, params string[] filters)
        {
            Title = title;
            CurrentDir = currentDir;
            DefaultFileName = defaultFileName;
            Filters = filters;
            saveFileDialog.Title = title;
            saveFileDialog.CurrentDir = currentDir;
            saveFileDialog.CurrentFile = defaultFileName;
            saveFileDialog.Filters = filters;
        }

        private void OnSaveButtonPressed()
        {
            saveFileDialog.OpenInSaveMode(Title, CurrentDir, DefaultFileName, App.RATIO_FILEDIALOG, Filters);
        }

        private void OnSaveFileDialogFileSelected(string path)
        {
            if(saveFileDialog.FileMode != FileMode.SaveFile) return;
            if(OnFileExported != null) OnFileExported(path);
        }
    }
}