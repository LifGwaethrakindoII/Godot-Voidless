using Godot;
using System;

namespace Voidless
{
    public static class VFileDialog
    {
        public static void OpenInLoadMode(this FileDialog fileDialog, string title, params string[] filters)
        {
            if(fileDialog == null || filters.IsNullOrEmpty()) return;

            fileDialog.Title = title;
            fileDialog.Filters = filters;
            fileDialog.FileMode = FileDialog.FileModeEnum.OpenFile;
            fileDialog.Access = FileDialog.AccessEnum.Filesystem;
            fileDialog.PopupCenteredRatio(0.8f);
        }

        public static void OpenInSaveMode(this FileDialog fileDialog, string title, string defaultFileName, params string[] filters)
        {
            if(fileDialog == null || filters.IsNullOrEmpty()) return;

            fileDialog.Title = title;
            fileDialog.Filters = filters;
            fileDialog.FileMode = FileDialog.FileModeEnum.SaveFile;
            fileDialog.Access = FileDialog.AccessEnum.Filesystem;
            fileDialog.CurrentFile = defaultFileName;  
            fileDialog.PopupCenteredRatio(0.8f);
        }
    }
}