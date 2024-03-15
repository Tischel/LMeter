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
    public class BarConfig : IConfigPage
    {
        private static readonly string[] _anchorOptions = Enum.GetNames(typeof(DrawAnchor));
        private static readonly string[] _jobIconStyleOptions = new string[] { "Style 1", "Style 2" };
        private int _barCount = 8;
        private int _barGaps = 1;
        private bool _showJobIcon = true;
        private Vector2 _jobIconOffset = new Vector2(0, 0);
        private int _jobIconStyle;
        private bool _useJobColor = true;
        private bool _thousandsSeparators = true;
        private bool _showRankText;
        private string _rankTextFormat = "[rank].";
        private DrawAnchor _rankTextAlign = DrawAnchor.Right;
        private Vector2 _rankTextOffset = new Vector2(0, 0);
        private int _rankTextFontId;
        private bool _rankTextJobColor;
        private bool _rankTextShowOutline = true;
        private bool _useCharacterName;
        private bool _alwaysShowSelf;
        private string _leftTextFormat = "[name]";
        private Vector2 _leftTextOffset = new Vector2(0, 0);
        private int _barNameFontId;
        private bool _leftTextJobColor;
        private bool _barNameShowOutline = true;
        private string _rightTextFormat = "[damagetotal:k.1]  ([encdps:k.1], [damagepct])";
        private Vector2 _rightTextOffset = new Vector2(0, 0);
        private int _barDataFontId;
        private bool _rightTextJobColor;
        private bool _barDataShowOutline = true;

        public string Name => "Bars";

        public int BarCount
        {
            get => _barCount;
            set => _barCount = value;
        }

        public int BarGaps
        {
            get => _barGaps;
            set => _barGaps = value;
        }

        public bool ShowJobIcon
        {
            get => _showJobIcon;
            set => _showJobIcon = value;
        }

        public int JobIconStyle
        {
            get => _jobIconStyle;
            set => _jobIconStyle = value;
        }

        public Vector2 JobIconOffset
        {
            get => _jobIconOffset;
            set => _jobIconOffset = value;
        }

        public bool ThousandsSeparators
        {
            get => _thousandsSeparators;
            set => _thousandsSeparators = value;
        }

        public bool UseJobColor
        {
            get => _useJobColor;
            set => _useJobColor = value;
        }

        public ConfigColor BarColor { get; set; } = new ConfigColor(.3f, .3f, .3f, 1f);

        public bool ShowRankText
        {
            get => _showRankText;
            set => _showRankText = value;
        }

        public string RankTextFormat
        {
            get => _rankTextFormat;
            set => _rankTextFormat = value;
        }

        public DrawAnchor RankTextAlign
        {
            get => _rankTextAlign;
            set => _rankTextAlign = value;
        }

        public Vector2 RankTextOffset
        {
            get => _rankTextOffset;
            set => _rankTextOffset = value;
        }

        public bool RankTextJobColor
        {
            get => _rankTextJobColor;
            set => _rankTextJobColor = value;
        }

        public ConfigColor RankTextColor { get; set; } = new ConfigColor(1, 1, 1, 1);

        public bool RankTextShowOutline
        {
            get => _rankTextShowOutline;
            set => _rankTextShowOutline = value;
        }

        public ConfigColor RankTextOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);
        public string RankTextFontKey { get; set; } = FontsManager.DalamudFontKey;

        public int RankTextFontId
        {
            get => _rankTextFontId;
            set => _rankTextFontId = value;
        }

        public bool AlwaysShowSelf
        {
            get => _alwaysShowSelf;
            set => _alwaysShowSelf = value;
        }

        public string LeftTextFormat
        {
            get => _leftTextFormat;
            set => _leftTextFormat = value;
        }

        public Vector2 LeftTextOffset
        {
            get => _leftTextOffset;
            set => _leftTextOffset = value;
        }

        public bool LeftTextJobColor
        {
            get => _leftTextJobColor;
            set => _leftTextJobColor = value;
        }

        public ConfigColor BarNameColor { get; set; } = new ConfigColor(1, 1, 1, 1);

        public bool BarNameShowOutline
        {
            get => _barNameShowOutline;
            set => _barNameShowOutline = value;
        }

        public ConfigColor BarNameOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);
        public string BarNameFontKey { get; set; } = FontsManager.DalamudFontKey;

        public int BarNameFontId
        {
            get => _barNameFontId;
            set => _barNameFontId = value;
        }

        public bool UseCharacterName
        {
            get => _useCharacterName;
            set => _useCharacterName = value;
        }

        public string RightTextFormat
        {
            get => _rightTextFormat;
            set => _rightTextFormat = value;
        }

        public Vector2 RightTextOffset
        {
            get => _rightTextOffset;
            set => _rightTextOffset = value;
        }

        public bool RightTextJobColor
        {
            get => _rightTextJobColor;
            set => _rightTextJobColor = value;
        }

        public ConfigColor BarDataColor { get; set; } = new ConfigColor(1, 1, 1, 1);

        public bool BarDataShowOutline
        {
            get => _barDataShowOutline;
            set => _barDataShowOutline = value;
        }

        public ConfigColor BarDataOutlineColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);
        public string BarDataFontKey { get; set; } = FontsManager.DalamudFontKey;

        public int BarDataFontId
        {
            get => _barDataFontId;
            set => _barDataFontId = value;
        }

        public IConfigPage GetDefault()
        {
            BarConfig defaultConfig = new BarConfig();
            defaultConfig.BarNameFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.BarNameFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);

            defaultConfig.BarDataFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.BarDataFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);

            defaultConfig.RankTextFontKey = FontsManager.DefaultSmallFontKey;
            defaultConfig.RankTextFontId = Singletons.Get<FontsManager>().GetFontIndex(FontsManager.DefaultSmallFontKey);
            
            return defaultConfig;
        }

        public Vector2 DrawBar(
            ImDrawListPtr drawList,
            Vector2 localPos,
            Vector2 size,
            Combatant combatant,
            ConfigColor jobColor,
            ConfigColor barColor,
            float top,
            float current)
        {
            float barHeight = (size.Y - (this.BarCount - 1) * this.BarGaps) / this.BarCount;
            Vector2 barSize = new Vector2(size.X, barHeight);
            Vector2 barFillSize = new Vector2(size.X * (current / top), barHeight);
            drawList.AddRectFilled(localPos, localPos + barFillSize, this.UseJobColor ? jobColor.Base : barColor.Base);

            float textOffset = 5f;
            if (this.ShowJobIcon && combatant.Job != Job.UKN)
            {
                uint jobIconId = 62000u + (uint)combatant.Job + 100u * (uint)this.JobIconStyle;
                Vector2 jobIconSize = Vector2.One * barHeight;
                DrawHelpers.DrawIcon(jobIconId, localPos + this.JobIconOffset, jobIconSize, drawList);
                textOffset = barHeight;
            }

            if (this.ShowRankText)
            {
                string rankText = combatant.GetFormattedString($"{this.RankTextFormat}", this.ThousandsSeparators ? "N" : "F");
                using(FontsManager.GetFont(RankTextFontKey)?.Push())
                {
                    textOffset += ImGui.CalcTextSize("00.").X;
                    Vector2 rankTextSize = ImGui.CalcTextSize(rankText);
                    Vector2 rankTextPos = Utils.GetAnchoredPosition(localPos, -barSize, DrawAnchor.Left);
                    rankTextPos = Utils.GetAnchoredPosition(rankTextPos, rankTextSize, this.RankTextAlign) + this.RankTextOffset;
                    DrawHelpers.DrawText(
                        drawList,
                        rankText,
                        rankTextPos.AddX(textOffset),
                        this.RankTextJobColor ? jobColor.Base : this.RankTextColor.Base,
                        this.RankTextShowOutline,
                        this.RankTextOutlineColor.Base);
                }
            }

            using (FontsManager.GetFont(BarNameFontKey)?.Push())
            {
                string leftText = combatant.Name;
                if (!leftText.Contains("YOU"))
                {
                    leftText = combatant.GetFormattedString($" {this.LeftTextFormat} ", this.ThousandsSeparators ? "N" : "F");
                }

                Vector2 nameTextSize = ImGui.CalcTextSize(leftText);
                Vector2 namePos = Utils.GetAnchoredPosition(localPos, -barSize, DrawAnchor.Left);
                namePos = Utils.GetAnchoredPosition(namePos, nameTextSize, DrawAnchor.Left) + this.LeftTextOffset;
                DrawHelpers.DrawText(
                    drawList,
                    leftText,
                    namePos.AddX(textOffset),
                    this.LeftTextJobColor ? jobColor.Base : this.BarNameColor.Base,
                    this.BarNameShowOutline,
                    this.BarNameOutlineColor.Base);
            }

            using (FontsManager.GetFont(BarDataFontKey)?.Push())
            {
                string rightText = combatant.GetFormattedString($" {this.RightTextFormat} ", this.ThousandsSeparators ? "N" : "F");
                Vector2 dataTextSize = ImGui.CalcTextSize(rightText);
                Vector2 dataPos = Utils.GetAnchoredPosition(localPos, -barSize, DrawAnchor.Right);
                dataPos = Utils.GetAnchoredPosition(dataPos, dataTextSize, DrawAnchor.Right) + this.RightTextOffset;
                DrawHelpers.DrawText(
                    drawList,
                    rightText,
                    dataPos,
                    this.RightTextJobColor ? jobColor.Base : this.BarDataColor.Base,
                    this.BarDataShowOutline,
                    this.BarDataOutlineColor.Base);
            }

            return localPos.AddY(barHeight + this.BarGaps);
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
                ImGui.DragInt("Num Bars to Display", ref this._barCount, 1, 1, 48);
                ImGui.DragInt("Bar Gap Size", ref this._barGaps, 1, 0, 20);

                ImGui.Checkbox("Show Job Icon", ref this._showJobIcon);
                if (this.ShowJobIcon)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragFloat2("Job Icon Offset", ref this._jobIconOffset);

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Combo("Job Icon Style", ref this._jobIconStyle, _jobIconStyleOptions, _jobIconStyleOptions.Length);
                }

                ImGui.Checkbox("Use Job Colors for Bars", ref this._useJobColor);
                Vector4 vector = Vector4.Zero;
                if (!this.UseJobColor)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BarColor.Vector;
                    ImGui.ColorEdit4("Bar Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BarColor.Vector = vector;
                }
                
                ImGui.Checkbox("Use Thousands Separators for Numbers", ref this._thousandsSeparators);

                ImGui.NewLine();
                ImGui.Checkbox("Show Rank Text", ref this._showRankText);
                if (this.ShowRankText)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.InputText("Rank Text Format", ref this._rankTextFormat, 128);

                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip(Utils.GetTagsTooltip(Combatant.TextTags));
                    }
                    
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Combo("Rank Text Align", ref Unsafe.As<DrawAnchor, int>(ref this._rankTextAlign), _anchorOptions, _anchorOptions.Length);

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragFloat2("Rank Text Offset", ref this._rankTextOffset);

                    if (!FontsManager.ValidateFont(fontOptions, this.RankTextFontId, this.RankTextFontKey))
                    {
                        this.RankTextFontId = 0;
                        for (int i = 0; i < fontOptions.Length; i++)
                        {
                            if (this.RankTextFontKey.Equals(fontOptions[i]))
                            {
                                this.RankTextFontId = i;
                            }
                        }
                    }
                    
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Combo("Font##Rank", ref this._rankTextFontId, fontOptions, fontOptions.Length);
                    this.RankTextFontKey = fontOptions[this.RankTextFontId];
                    
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Use Job Color##RankTextJobColor", ref this._rankTextJobColor);
                    if (!this.RankTextJobColor)
                    {
                        DrawHelpers.DrawNestIndicator(2);
                        vector = this.RankTextColor.Vector;
                        ImGui.ColorEdit4("Text Color##Rank", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        this.RankTextColor.Vector = vector;
                    }

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Show Outline##Rank", ref this._rankTextShowOutline);
                    if (this.RankTextShowOutline)
                    {
                        DrawHelpers.DrawNestIndicator(2);
                        vector = this.RankTextOutlineColor.Vector;
                        ImGui.ColorEdit4("Outline Color##Rank", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                        this.RankTextOutlineColor.Vector = vector;
                    }
                }

                ImGui.NewLine();
                ImGui.Checkbox("Use your name instead of 'YOU'", ref this._useCharacterName);
                ImGui.Checkbox("Always show your own bar", ref this._alwaysShowSelf);
                ImGui.InputText("Left Text Format", ref this._leftTextFormat, 128);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(Utils.GetTagsTooltip(Combatant.TextTags));
                }

                ImGui.DragFloat2("Left Text Offset", ref this._leftTextOffset);

                if (!FontsManager.ValidateFont(fontOptions, this.BarNameFontId, this.BarNameFontKey))
                {
                    this.BarNameFontId = 0;
                    for (int i = 0; i < fontOptions.Length; i++)
                    {
                        if (this.BarNameFontKey.Equals(fontOptions[i]))
                        {
                            this.BarNameFontId = i;
                        }
                    }
                }
                
                ImGui.Combo("Font##Name", ref this._barNameFontId, fontOptions, fontOptions.Length);
                this.BarNameFontKey = fontOptions[this.BarNameFontId];
                
                
                ImGui.Checkbox("Use Job Color##LeftTextJobColor", ref this._leftTextJobColor);
                if (!this.LeftTextJobColor)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BarNameColor.Vector;
                    ImGui.ColorEdit4("Text Color##Name", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BarNameColor.Vector = vector;
                }

                ImGui.Checkbox("Show Outline##Name", ref this._barNameShowOutline);
                if (this.BarNameShowOutline)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BarNameOutlineColor.Vector;
                    ImGui.ColorEdit4("Outline Color##Name", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BarNameOutlineColor.Vector = vector;
                }

                ImGui.NewLine();
                ImGui.InputText("Right Text Format", ref this._rightTextFormat, 128);

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip(Utils.GetTagsTooltip(Combatant.TextTags));
                }

                ImGui.DragFloat2("Right Text Offset", ref this._rightTextOffset);

                if (!FontsManager.ValidateFont(fontOptions, this.BarDataFontId, this.BarDataFontKey))
                {
                    this.BarDataFontId = 0;
                    for (int i = 0; i < fontOptions.Length; i++)
                    {
                        if (this.BarDataFontKey.Equals(fontOptions[i]))
                        {
                            this.BarDataFontId = i;
                        }
                    }
                }
                
                ImGui.Combo("Font##Data", ref this._barDataFontId, fontOptions, fontOptions.Length);
                this.BarDataFontKey = fontOptions[this.BarDataFontId];
                
                ImGui.Checkbox("Use Job Color##RightTextJobColor", ref this._rightTextJobColor);
                if (!this.RightTextJobColor)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BarDataColor.Vector;
                    ImGui.ColorEdit4("Text Color##Data", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BarDataColor.Vector = vector;
                }

                ImGui.Checkbox("Show Outline##Data", ref this._barDataShowOutline);
                if (this.BarDataShowOutline)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BarDataOutlineColor.Vector;
                    ImGui.ColorEdit4("Outline Color##Data", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BarDataOutlineColor.Vector = vector;
                }
            }

            ImGui.EndChild();
        }
    }
}
