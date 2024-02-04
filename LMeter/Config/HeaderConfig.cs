using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using LMeter.ACT;
using LMeter.ACT.DataStructures;
using LMeter.Helpers;
using LMeter.Enums;

namespace LMeter.Config
{
    public class HeaderConfig : IConfigPage
    {
        private static readonly string[] _anchorOptions = Enum.GetNames(typeof(DrawAnchor));
        private bool _showHeader = true;
        private int _headerHeight = 25;
        private bool _showEncounterDuration = true;
        private Vector2 _durationOffset = new Vector2(0, 0);
        private int _durationFontId;
        private DrawAnchor _durationAlign = DrawAnchor.Left;
        private bool _durationShowOutline = true;
        private bool _showEncounterName = true;
        private Vector2 _nameOffset = new Vector2(0, 0);
        private int _nameFontId;
        private DrawAnchor _nameAlign = DrawAnchor.Left;
        private bool _nameShowOutline = true;
        private bool _showRaidStats = true;
        private string _raidStatsFormat = "[dps]rdps [hps]rhps Deaths: [deaths]";
        private bool _thousandsSeparators = true;
        private Vector2 _statsOffset = new Vector2(0, 0);
        private int _statsFontId;
        private DrawAnchor _statsAlign = DrawAnchor.Right;
        private bool _statsShowOutline = true;

        public string Name => "Header";

        public bool ShowHeader
        {
            get => _showHeader;
            set => _showHeader = value;
        }

        public int HeaderHeight
        {
            get => _headerHeight;
            set => _headerHeight = value;
        }

        public ConfigColor BackgroundColor { get; set; } = new ConfigColor(30f / 255f, 30f / 255f, 30f / 255f, 230 / 255f);

        public bool ShowEncounterDuration
        {
            get => _showEncounterDuration;
            set => _showEncounterDuration = value;
        }

        public ConfigColor DurationColor { get; set; } = new ConfigColor(0f / 255f, 190f / 255f, 225f / 255f, 1f);

        public bool DurationShowOutline
        {
            get => _durationShowOutline;
            set => _durationShowOutline = value;
        }

        public ConfigColor DurationOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);

        public DrawAnchor DurationAlign
        {
            get => _durationAlign;
            set => _durationAlign = value;
        }

        public Vector2 DurationOffset
        {
            get => _durationOffset;
            set => _durationOffset = value;
        }

        public int DurationFontId
        {
            get => _durationFontId;
            set => _durationFontId = value;
        }

        public string DurationFontKey { get; set; } = FontsManager.DalamudFontKey;

        public bool ShowEncounterName
        {
            get => _showEncounterName;
            set => _showEncounterName = value;
        }

        public ConfigColor NameColor { get; set; } = new ConfigColor(1, 1, 1, 1);

        public bool NameShowOutline
        {
            get => _nameShowOutline;
            set => _nameShowOutline = value;
        }

        public ConfigColor NameOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);

        public DrawAnchor NameAlign
        {
            get => _nameAlign;
            set => _nameAlign = value;
        }

        public Vector2 NameOffset
        {
            get => _nameOffset;
            set => _nameOffset = value;
        }

        public int NameFontId
        {
            get => _nameFontId;
            set => _nameFontId = value;
        }

        public string NameFontKey { get; set; } = FontsManager.DalamudFontKey;

        public bool ShowRaidStats
        {
            get => _showRaidStats;
            set => _showRaidStats = value;
        }

        public ConfigColor RaidStatsColor { get; set; } = new ConfigColor(0.5f, 0.5f, 0.5f, 1f);

        public bool StatsShowOutline
        {
            get => _statsShowOutline;
            set => _statsShowOutline = value;
        }

        public ConfigColor StatsOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);

        public DrawAnchor StatsAlign
        {
            get => _statsAlign;
            set => _statsAlign = value;
        }

        public Vector2 StatsOffset
        {
            get => _statsOffset;
            set => _statsOffset = value;
        }

        public int StatsFontId
        {
            get => _statsFontId;
            set => _statsFontId = value;
        }

        public string StatsFontKey { get; set; } = FontsManager.DalamudFontKey;

        public string RaidStatsFormat
        {
            get => _raidStatsFormat;
            set => _raidStatsFormat = value;
        }

        public bool ThousandsSeparators
        {
            get => _thousandsSeparators;
            set => _thousandsSeparators = value;
        }

        public IConfigPage GetDefault()
        {
            HeaderConfig defaultConfig = new HeaderConfig();
            defaultConfig.DurationFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.DurationFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);
            defaultConfig.NameFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.NameFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);
            defaultConfig.StatsFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.StatsFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);
            return defaultConfig;
        }

        public (Vector2, Vector2) DrawHeader(Vector2 pos, Vector2 size, Encounter? encounter, ImDrawListPtr drawList)
        {
            if (!this.ShowHeader)
            {
                return (pos, size);
            }
            
            Vector2 headerSize = new Vector2(size.X, this.HeaderHeight);
            drawList.AddRectFilled(pos, pos + headerSize, this.BackgroundColor.Base);
            
            Vector2 durationPos = pos;
            Vector2 durationSize = Vector2.Zero;
            if (this.ShowEncounterDuration)
            {
                using (FontsManager.PushFont(this.DurationFontKey))
                {
                    string duration = encounter is null ? $" LMeter v{Plugin.Version}" : $" {encounter.Duration}";
                    durationSize = ImGui.CalcTextSize(duration);
                    durationPos = Utils.GetAnchoredPosition(durationPos, -headerSize, DrawAnchor.Left);
                    durationPos = Utils.GetAnchoredPosition(durationPos, durationSize, this.DurationAlign) + this.DurationOffset;
                    DrawHelpers.DrawText(drawList, duration, durationPos, this.DurationColor.Base, this.DurationShowOutline, this.DurationOutlineColor.Base);
                }
            }

            Vector2 raidStatsSize = Vector2.Zero;
            if (this.ShowRaidStats && encounter is not null)
            {
                string text = encounter.GetFormattedString($" {this.RaidStatsFormat} ", this.ThousandsSeparators ? "N" : "F");

                if (!string.IsNullOrEmpty(text))
                {
                    using (FontsManager.PushFont(this.StatsFontKey))
                    {
                        raidStatsSize = ImGui.CalcTextSize(text);
                        Vector2 statsPos = Utils.GetAnchoredPosition(pos + this.StatsOffset, -headerSize, DrawAnchor.Right);
                        statsPos = Utils.GetAnchoredPosition(statsPos, raidStatsSize, this.StatsAlign);
                        DrawHelpers.DrawText(drawList, text, statsPos, this.RaidStatsColor.Base, this.StatsShowOutline, this.StatsOutlineColor.Base);
                    }
                }
            }

            if (this.ShowEncounterName && encounter is not null && !string.IsNullOrEmpty(encounter.Title))
            {
                using (FontsManager.PushFont(this.NameFontKey))
                {
                    string name = $" {encounter.Title}";
                    Vector2 nameSize = ImGui.CalcTextSize(name);

                    if (durationSize.X + raidStatsSize.X + nameSize.X > size.X)
                    {
                        float elipsesWidth = ImGui.CalcTextSize("... ").X;
                        do
                        {
                            name = name.AsSpan(0, name.Length - 1).ToString();
                            nameSize = ImGui.CalcTextSize(name);
                        }
                        while (durationSize.X + raidStatsSize.X + nameSize.X + elipsesWidth > size.X && name.Length > 1);
                        name += "... ";
                    }

                    Vector2 namePos = Utils.GetAnchoredPosition(pos.AddX(durationSize.X), -headerSize, DrawAnchor.Left);
                    namePos = Utils.GetAnchoredPosition(namePos, nameSize, this.NameAlign) + this.NameOffset;
                    DrawHelpers.DrawText(drawList, name, namePos, this.NameColor.Base, this.NameShowOutline, this.NameOutlineColor.Base);
                }
            }
            
            return (pos.AddY(this.HeaderHeight), size.AddY(-this.HeaderHeight));
        }

        public void DrawConfig(Vector2 size, float padX, float padY)
        {
            string[] fontOptions = FontsManager.GetFontList();
            if (fontOptions.Length == 0)
            {
                return;
            }

            if (ImGui.BeginChild($"##{this.Name}", new Vector2(size.X, size.Y), true))
            {
                ImGui.Checkbox("Show Header", ref this._showHeader);
                if (this.ShowHeader)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragInt("Header Height", ref this._headerHeight, 1, 0, 100);

                    DrawHelpers.DrawNestIndicator(1);
                    Vector4 vector = this.BackgroundColor.Vector;
                    ImGui.ColorEdit4("Background Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BackgroundColor.Vector = vector;

                    ImGui.NewLine();
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Show Encounter Duration", ref this._showEncounterDuration);
                    if (this.ShowEncounterDuration)
                    {
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.DragFloat2("Position Offset##Duration", ref this._durationOffset);

                        if (!FontsManager.ValidateFont(fontOptions, this.DurationFontId, this.DurationFontKey))
                        {
                            this.DurationFontId = 0;
                            for (int i = 0; i < fontOptions.Length; i++)
                            {
                                if (this.DurationFontKey.Equals(fontOptions[i]))
                                {
                                    this.DurationFontId = i;
                                }
                            }
                        }
                        
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Font##Duration", ref this._durationFontId, fontOptions, fontOptions.Length);
                        this.DurationFontKey = fontOptions[this.DurationFontId];

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Text Align##Duration", ref Unsafe.As<DrawAnchor, int>(ref this._durationAlign), _anchorOptions, _anchorOptions.Length);

                        DrawHelpers.DrawNestIndicator(2);
                        vector = this.DurationColor.Vector;
                        ImGui.ColorEdit4("Text Color##Duration", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        this.DurationColor.Vector = vector;

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Checkbox("Show Outline##Duration", ref this._durationShowOutline);
                        if (this.DurationShowOutline)
                        {
                            DrawHelpers.DrawNestIndicator(3);
                            vector = this.DurationOutlineColor.Vector;
                            ImGui.ColorEdit4("Outline Color##Duration", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                            this.DurationOutlineColor.Vector = vector;
                        }
                    }

                    ImGui.NewLine();
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Show Encounter Name", ref this._showEncounterName);
                    if (this.ShowEncounterName)
                    {
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.DragFloat2("Position Offset##Name", ref this._nameOffset);

                        if (!FontsManager.ValidateFont(fontOptions, this.NameFontId, this.NameFontKey))
                        {
                            this.NameFontId = 0;
                            for (int i = 0; i < fontOptions.Length; i++)
                            {
                                if (this.NameFontKey.Equals(fontOptions[i]))
                                {
                                    this.NameFontId = i;
                                }
                            }
                        }
                        
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Font##Name", ref this._nameFontId, fontOptions, fontOptions.Length);
                        this.NameFontKey = fontOptions[this.NameFontId];

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Text Align##Name", ref Unsafe.As<DrawAnchor, int>(ref this._nameAlign), _anchorOptions, _anchorOptions.Length);

                        DrawHelpers.DrawNestIndicator(2);
                        vector = this.NameColor.Vector;
                        ImGui.ColorEdit4("Text Color##Name", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        this.NameColor.Vector = vector;

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Checkbox("Show Outline##Name", ref this._nameShowOutline);
                        if (this.NameShowOutline)
                        {
                            DrawHelpers.DrawNestIndicator(3);
                            vector = this.NameOutlineColor.Vector;
                            ImGui.ColorEdit4("Outline Color##Name", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                            this.NameOutlineColor.Vector = vector;
                        }
                    }

                    ImGui.NewLine();
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Show Raid Stats", ref this._showRaidStats);
                    if (this.ShowRaidStats)
                    {
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.InputText("Raid Stats Format", ref this._raidStatsFormat, 128);
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(Utils.GetTagsTooltip(Encounter.TextTags));
                        }
                        
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Checkbox("Use Thousands Separators for Numbers", ref this._thousandsSeparators);

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.DragFloat2("Position Offset##Stats", ref this._statsOffset);

                        if (!FontsManager.ValidateFont(fontOptions, this.StatsFontId, this.StatsFontKey))
                        {
                            this.StatsFontId = 0;
                            for (int i = 0; i < fontOptions.Length; i++)
                            {
                                if (this.StatsFontKey.Equals(fontOptions[i]))
                                {
                                    this.StatsFontId = i;
                                }
                            }
                        }
                        
                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Font##Stats", ref this._statsFontId, fontOptions, fontOptions.Length);
                        this.StatsFontKey = fontOptions[this.StatsFontId];

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Combo("Text Align##Stats", ref Unsafe.As<DrawAnchor, int>(ref this._statsAlign), _anchorOptions, _anchorOptions.Length);

                        DrawHelpers.DrawNestIndicator(2);
                        vector = this.RaidStatsColor.Vector;
                        ImGui.ColorEdit4("Text Color##Stats", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        this.RaidStatsColor.Vector = vector;

                        DrawHelpers.DrawNestIndicator(2);
                        ImGui.Checkbox("Show Outline##Stats", ref this._statsShowOutline);
                        if (this.StatsShowOutline)
                        {
                            DrawHelpers.DrawNestIndicator(3);
                            vector = this.StatsOutlineColor.Vector;
                            ImGui.ColorEdit4("Outline Color##Stats", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                            this.StatsOutlineColor.Vector = vector;
                        }
                    }
                }
            }

            ImGui.EndChild();
        }
    }
}
