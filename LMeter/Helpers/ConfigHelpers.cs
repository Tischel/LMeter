using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Dalamud.Interface.Internal.Notifications;
using ImGuiNET;
using System.Text.Json;
using System.Text.RegularExpressions;
using LMeter.Config;
using Dalamud.Plugin.Services;
using LMeter.Converters;

namespace LMeter.Helpers
{
    public static class ConfigHelpers
    {
        private static readonly JsonSerializerOptions SerializerOptionsIndented = new()
        {
            WriteIndented = true,
            Converters =
            {
                new Vector2JsonConverter(),
                new Vector4JsonConverter()
            }
        };
        
        private static readonly JsonSerializerOptions SerializerOptionsFlat = new()
        {
            WriteIndented = false,
            Converters =
            {
                new Vector2JsonConverter(),
                new Vector4JsonConverter()
            }
        };

        public static void ExportToClipboard<T>(T toExport)
        {
            string? exportString = ConfigHelpers.GetExportString(toExport);

            if (exportString is not null)
            {
                ImGui.SetClipboardText(exportString);
                DrawHelpers.DrawNotification("Export string copied to clipboard.");
            }
            else
            {
                DrawHelpers.DrawNotification("Failed to Export!", NotificationType.Error);
            }
        }

        public static string? GetExportString<T>(T toExport)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(toExport, SerializerOptionsFlat);
                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (DeflateStream compressionStream = new DeflateStream(outputStream, CompressionLevel.Optimal))
                    {
                        using (StreamWriter writer = new StreamWriter(compressionStream, Encoding.UTF8))
                        {
                            writer.Write(jsonString);
                        }
                    }

                    return Convert.ToBase64String(outputStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }

            return null;
        }

        public static T? GetFromImportString<T>(string importString)
        {
            if (string.IsNullOrEmpty(importString)) return default;

            try
            {
                byte[] bytes = Convert.FromBase64String(importString);

                string decodedJsonString;
                using (MemoryStream inputStream = new MemoryStream(bytes))
                {
                    using (DeflateStream compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress))
                    {
                        using (StreamReader reader = new StreamReader(compressionStream, Encoding.UTF8))
                        {
                            decodedJsonString = reader.ReadToEnd();
                        }
                    }
                }
                
                T? importedObj = JsonSerializer.Deserialize<T>(decodedJsonString, SerializerOptionsFlat);
                return importedObj;
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }

            return default;
        }

        public static LMeterConfig LoadConfig(string path)
        {
            LMeterConfig? config = null;

            try
            {
                if (File.Exists(path))
                {
                    string jsonString = File.ReadAllText(path);
                    
                    // Temporary measure, should be removed after a while of having being ran in production.
                    // We have to remove the `$type` lines from the config otherwise we can't properly move to system.text.json
                    // On top of it being a massive potential security issue and it being an anti pattern
                    var typeDefPattern = @"(^.*""\$type"": .*\n)";
                    jsonString = Regex.Replace(jsonString, typeDefPattern, string.Empty, RegexOptions.Multiline | RegexOptions.Compiled);
                    
                    config = JsonSerializer.Deserialize<LMeterConfig>(jsonString, SerializerOptionsIndented);
                }
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());

                string backupPath = $"{path}.bak";
                if (File.Exists(path))
                {
                    try
                    {
                        File.Copy(path, backupPath);
                        Singletons.Get<IPluginLog>().Information($"Backed up LMeter config to '{backupPath}'.");
                    }
                    catch
                    {
                        Singletons.Get<IPluginLog>().Warning($"Unable to back up LMeter config.");
                    }
                }
            }

            return config ?? new LMeterConfig();
        }

        public static void SaveConfig()
        {
            ConfigHelpers.SaveConfig(Singletons.Get<LMeterConfig>());
        }

        public static void SaveConfig(LMeterConfig config)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(config, SerializerOptionsIndented);
                File.WriteAllText(Plugin.ConfigFilePath, jsonString);
            }
            catch (Exception ex)
            {
                Singletons.Get<IPluginLog>().Error(ex.ToString());
            }
        }
    }
}
