using Godot;
using System;
using Voidless.UI;

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
        private string title;
        private string currentDir;
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

        public string[] Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public override void _Ready()
        {
            loadButton.Pressed += OnLoadButtonPressed;
            loadFileDialog.FileSelected += OnLoadFileDialogFileSelected;

            fileInfo.Text = string.Empty;
        }

        public void SetValues(string title, string currentDir, params string[] filters)
        {
            Title = title;
            CurrentDir = currentDir;
            Filters = filters;
            loadFileDialog.Title = title;
            loadFileDialog.CurrentDir = currentDir;
            loadFileDialog.Filters = filters;
        }

        private void OnLoadButtonPressed()
        {
            // Add this line to see exactly what Godot is receiving
            GD.Print("[ImportControl] Opening FileDialog in directory: '" + CurrentDir + "'");
            loadFileDialog.OpenInLoadMode(Title, CurrentDir, App.RATIO_FILEDIALOG, Filters);
        }

        private void OnLoadFileDialogFileSelected(string path)
        {
            if(loadFileDialog.FileMode != FileMode.OpenFile) return;
            fileInfo.Text = System.IO.Path.GetFileName(path);
            if(OnFileImported != null) OnFileImported(path);
        }
    }
}