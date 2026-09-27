using Godot;
using System;
using Voidless;
using Voidless.UI;

namespace Voidless.PixelArtToolkit
{
    public enum FileEvent { Save, Preview, Load }

    public delegate void OnFileEvent(FileEvent fileEvent);

    public partial class FileControl : ControlModule
    {
        public event OnFileEvent OnFileEvent;

        [Export] private Label fileInfo;
        [Export] private CheckBox previewCheckBox;
        [Export] private Button loadButton;
        [Export] private Button previewButton;
        [Export] private Button saveButton;
        [ExportCategory("Windows")]
        [Export] private FileDialog fileDialog;
        [Export] private DisplayDialogue popUp;

        public Label FileInfo { get { return fileInfo; } }
        
        public override void _Ready()
        {
            previewCheckBox.Toggled += OnPreviewCheckBoxToggled;
            loadButton.Pressed += ()=> InvokeFileEvent(FileEvent.Load);
            previewButton.Pressed += ()=> InvokeFileEvent(FileEvent.Preview);
            saveButton.Pressed += ()=> InvokeFileEvent(FileEvent.Save);
        }

        private void InvokeFileEvent(FileEvent fileEvent) { if(OnFileEvent != null) OnFileEvent(fileEvent); }

        private void OnPreviewCheckBoxToggled(bool toggled)
        {

        }
    }
}