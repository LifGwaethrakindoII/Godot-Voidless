global using FileAccess = Godot.FileDialog.AccessEnum;
global using FileMode = Godot.FileDialog.FileModeEnum;

using Godot;
using System;

namespace Voidless.UI
{
    public static class VFileDialog
    {
        public static void OpenInLoadMode(this FileDialog fileDialog, string title, string currentDir, float ratio = 0.8f, params string[] filters)
        {
            if(fileDialog == null || filters.IsNullOrEmpty()) return;

            fileDialog.Title = title;
            fileDialog.Access = FileAccess.Filesystem;
            fileDialog.CurrentDir = currentDir;
            fileDialog.Filters = filters;
            fileDialog.FileMode = FileMode.OpenFile;
            fileDialog.PopupCenteredRatio(ratio);
        }

        public static void OpenInSaveMode(this FileDialog fileDialog, string title, string currentDir, string defaultFileName, float ratio = 0.8f, params string[] filters)
        {
            if(fileDialog == null || filters.IsNullOrEmpty()) return;

            fileDialog.Title = title;
            fileDialog.CurrentDir = currentDir;
            fileDialog.Filters = filters;
            fileDialog.FileMode = FileMode.SaveFile;
            fileDialog.Access = FileAccess.Filesystem;
            fileDialog.CurrentFile = defaultFileName;  
            fileDialog.PopupCenteredRatio(ratio);
        }
    }
}