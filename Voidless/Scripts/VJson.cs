using System.Text.Json;
using System;

namespace Voidless
{
    public static class VJson
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true // Formats the string with clean line breaks and spacing
        };

        public static T FromJsonString<T>(string jsonString, T def = default)
        {
            // 1. Return a fresh instance if the string is empty to avoid crashes
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return def;
            }

            try
            {
                // 2. Deserialize the string using .NET's System.Text.Json engine
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (System.Exception e)
            {
                Godot.GD.PrintErr($"Error parsing object from JSON string: {e.Message}");
                return def;
            }
        }

        /// <summary>
        /// Converts any custom C# data type directly into a JSON string.
        /// </summary>
        public static string ToJsonString<T>(T data)
        {
            try
            {
                return JsonSerializer.Serialize<T>(data, _options);
            }
            catch (System.Exception e)
            {
                Godot.GD.PrintErr($"Error converting object to JSON string: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Converts a raw JSON string back into your explicit custom C# object.
        /// </summary>
        /*public static T FromJsonString<T>(string jsonString) where T : new()
        {
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return new T();
            }

            try
            {
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (System.Exception e)
            {
                Godot.GD.PrintErr($"Error parsing object from JSON string: {e.Message}");
                return new T();
            }
        }

        public static void SaveToJSON<T>(T data, string path)
        {
            try
            {
                // 1. Serialize object directly to JSON string
                string jsonString = JsonSerializer.Serialize<T>(data, _options);

                // 2. Open Godot file stream and save
                using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                if (file == null)
                {
                    GD.PrintErr(string.Concat("Failed to open path for writing: ", path));
                    return false;
                }

                file.StoreString(jsonString);
                return true;
            }
            catch (System.Exception e)
            {
                GD.PrintErr(string.Concat("Error saving generic JSON: ", e.Message));
                return false;
            }
        }

        public static T LoadJSON<T>(string path, T def = default) where T : new()
        {
            if (!FileAccess.FileExists(path))
            {
                GD.Print($"File not found: {path}. Returning default instance.");
                return def; // Return a clean object instead of null if preferred
            }

            try
            {
                // 1. Read file text
                using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
                string jsonString = file.GetAsText();

                // 2. Deserialize directly to the requested type T
                return JsonSerializer.Deserialize<T>(jsonString, _options);
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"Error loading generic JSON: {e.Message}");
                return def;
            }
        }*/
    }
}