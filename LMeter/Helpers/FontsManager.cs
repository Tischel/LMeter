using Dalamud.Interface;
using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Plugin;

namespace LMeter.Helpers
{
    public class FontsManager : IPluginDisposable
    {
        private IEnumerable<FontData> _fontData;
        private readonly Dictionary<string, IFontHandle> _imGuiFonts;
        private string[] _fontList;
        private readonly IFontAtlas _fontAtlas;

        public const string DalamudFontKey = "Dalamud Font";

        public static readonly List<string> DefaultFontKeys = ["Expressway_24", "Expressway_20", "Expressway_16"];
        public static string DefaultBigFontKey => DefaultFontKeys[0];
        public static string DefaultMediumFontKey => DefaultFontKeys[1];
        public static string DefaultSmallFontKey => DefaultFontKeys[2];

        public FontsManager(UiBuilder uiBuilder, IEnumerable<FontData> fonts)
        {
            _fontData = fonts;
            _fontList = [DalamudFontKey];
            _imGuiFonts = new Dictionary<string, IFontHandle>();

            _fontAtlas = uiBuilder.CreateFontAtlas(FontAtlasAutoRebuildMode.Async);
            BuildFonts();
        }

        private void BuildFonts()
        {
            var fontDir = GetUserFontPath();

            if (string.IsNullOrWhiteSpace(fontDir))
            {
                return;
            }

            ClearFontHandles();

            foreach (var font in _fontData)
            {
                var fontPath = $"{fontDir}{font.Name}.ttf";
                if (!File.Exists(fontPath))
                {
                    continue;
                }
                
                var handle = _fontAtlas.NewDelegateFontHandle(
                    e => e.OnPreBuild(
                        tk =>
                        {
                            var config = new SafeFontConfig { SizePx = font.Size };
                            config.MergeFont = tk.AddFontFromFile(fontPath, config);
                            tk.AddGameSymbol(config);
                            tk.AttachExtraGlyphsForDalamudLanguage(config);
                            tk.Font = config.MergeFont;
                        }));

                _imGuiFonts.Add(GetFontKey(font), handle);
            }

            List<string> fontList = [DalamudFontKey];
            fontList.AddRange(_imGuiFonts.Keys);
            _fontList = fontList.ToArray();

            // TODO: Can't make codebase async yet, as we have too tight couples between logic so we just discard the return value and fire forget
            if (_fontAtlas.BuildTask.IsCanceled || _fontAtlas.BuildTask.IsCompleted)
            {
                _ = _fontAtlas.BuildFontsAsync();
            }
        }

        private void ClearFontHandles()
        {
            foreach (var (_, value) in _imGuiFonts)
            {
                value.Dispose();
            }
            _imGuiFonts.Clear();
        }

        public static bool ValidateFont(IReadOnlyList<string> fontOptions, int fontId, string fontKey)
        {
            return fontId < fontOptions.Count && fontOptions[fontId].Equals(fontKey);
        }

        public static IFontHandle? GetFont(string fontKey)
        {
            var manager = Singletons.Get<FontsManager>();
            if (string.IsNullOrWhiteSpace(fontKey) || fontKey.Equals(DalamudFontKey) || !manager._imGuiFonts.ContainsKey(fontKey))
            {
                return null;
            }
            
            return manager._imGuiFonts[fontKey];
        }

        public void UpdateFonts(IEnumerable<FontData> fonts)
        {
            _fontData = fonts;
            BuildFonts();
        }

        public static string[] GetFontList()
        {
            return Singletons.Get<FontsManager>()._fontList;
        }

        public int GetFontIndex(string fontKey)
        {
            for (var i = 0; i < _fontList.Length; i++)
            {
                if (_fontList[i].Equals(fontKey))
                {
                    return i;
                }
            }

            return -1;
        }

        public static string GetFontKey(FontData font)
        {
            var key = $"{font.Name}_{font.Size}";
            key += (font.Chinese ? "_cnjp" : string.Empty);
            key += (font.Korean ? "_kr" : string.Empty);
            return key;
        }

        public static void CopyPluginFontsToUserPath()
        {
            var pluginFontPath = GetPluginFontPath();
            var userFontPath = GetUserFontPath();

            if (string.IsNullOrWhiteSpace(pluginFontPath) || string.IsNullOrWhiteSpace(userFontPath))
            {
                return;
            }

            if (!Directory.Exists(userFontPath))
            {
                try
                {
                    Directory.CreateDirectory(userFontPath);
                }
                catch (Exception ex)
                {
                    Singletons.Get<IPluginLog>().Warning($"Failed to create User Font Directory {ex.ToString()}");
                }
            }

            if (!Directory.Exists(userFontPath))
            {
                return;
            }

            string[] pluginFonts;
            try
            {
                pluginFonts = Directory.GetFiles(pluginFontPath, "*.ttf");
            }
            catch
            {
                pluginFonts = Array.Empty<string>();
            }

            foreach (var font in pluginFonts)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(font))
                    {
                        continue;
                    }
                    
                    var fileName = font.Replace(pluginFontPath, string.Empty);
                    var copyPath = Path.Combine(userFontPath, fileName);
                    if (!File.Exists(copyPath))
                    {
                        File.Copy(font, copyPath, false);
                    }
                }
                catch (Exception ex)
                {
                    Singletons.Get<IPluginLog>().Warning($"Failed to copy font {font} to User Font Directory: {ex.ToString()}");
                }
            }
        }

        private static string GetPluginFontPath()
        {
            var pluginInterface = Singletons.Get<DalamudPluginInterface>();
            var path = pluginInterface.AssemblyLocation.DirectoryName;

            if (path is not null)
            {
                return $@"{path}\Media\Fonts\";
            }

            return string.Empty;
        }

        public static string GetUserFontPath()
        {
            return $@"{Plugin.ConfigFileDir}\Fonts\";
        }

        public static string[] GetFontNamesFromPath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Array.Empty<string>();
            }

            string[] fonts;
            try
            {
                fonts = Directory.GetFiles(path, "*.ttf");
            }
            catch
            {
                fonts = Array.Empty<string>();
            }

            return fonts
                .Select(f => f
                    .Replace(path, string.Empty)
                    .Replace(".ttf", string.Empty, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }
            
            ClearFontHandles();
        }
    }
}
