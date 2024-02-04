using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using LMeter.Helpers;
using System.Text.Json.Serialization;

namespace LMeter.Config
{
    public enum MeterDataType
    {
        Damage,
        Healing,
        DamageTaken
    }

    public class GeneralConfig : IConfigPage
    {
        private static readonly string[] _meterTypeOptions = Enum.GetNames(typeof(MeterDataType));
        
        private Vector2 _position = Vector2.Zero;
        private Vector2 _size = new Vector2(ImGui.GetMainViewport().Size.Y * 16 / 90, ImGui.GetMainViewport().Size.Y / 10);
        private bool _lock = false;
        private bool _clickThrough = false;
        private bool _showBorder = true;
        private int _borderThickness = 2;
        private bool _borderAroundBars = false;
        private MeterDataType _dataType = MeterDataType.Damage;
        private bool _returnToCurrent = true;
        private bool _preview = false;

        public string Name => "General";

        [JsonIgnore]
        public bool Preview
        {
            get => _preview;
            set => _preview = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Vector2 Size
        {
            get => _size;
            set => _size = value;
        }

        public bool Lock
        {
            get => _lock;
            set => _lock = value;
        }

        public bool ClickThrough
        {
            get => _clickThrough;
            set => _clickThrough = value;
        }

        public ConfigColor BackgroundColor { get; set; } = new ConfigColor(0, 0, 0, 0.5f);

        public bool ShowBorder
        {
            get => _showBorder;
            set => _showBorder = value;
        }

        public bool BorderAroundBars
        {
            get => _borderAroundBars;
            set => _borderAroundBars = value;
        }

        public ConfigColor BorderColor { get; set; } = new ConfigColor(30f / 255f, 30f / 255f, 30f / 255f, 230f / 255f);

        public int BorderThickness
        {
            get => _borderThickness;
            set => _borderThickness = value;
        }

        public MeterDataType DataType
        {
            get => _dataType;
            set => _dataType = value;
        }

        public bool ReturnToCurrent
        {
            get => _returnToCurrent;
            set => _returnToCurrent = value;
        }
        
        public IConfigPage GetDefault() => new GeneralConfig();

        public void DrawConfig(Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild($"##{this.Name}", new Vector2(size.X, size.Y), true))
            {
                Vector2 screenSize = ImGui.GetMainViewport().Size;
                ImGui.DragFloat2("Position", ref this._position, 1, -screenSize.X / 2, screenSize.X / 2);
                ImGui.DragFloat2("Size", ref this._size, 1, 0, screenSize.Y);
                ImGui.Checkbox("Lock", ref this._lock);
                ImGui.Checkbox("Click Through", ref this._clickThrough);
                ImGui.Checkbox("Preview", ref this._preview);

                ImGui.NewLine();

                Vector4 vector = this.BackgroundColor.Vector;
                ImGui.ColorEdit4("Background Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                this.BackgroundColor.Vector = vector;

                ImGui.Checkbox("Show Border", ref this._showBorder);
                if (this.ShowBorder)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.DragInt("Border Thickness", ref this._borderThickness, 1, 1, 20);

                    DrawHelpers.DrawNestIndicator(1);
                    vector = this.BorderColor.Vector;
                    ImGui.ColorEdit4("Border Color", ref vector, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
                    this.BorderColor.Vector = vector;

                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.Checkbox("Hide border around Header", ref this._borderAroundBars);
                }

                ImGui.NewLine();
                ImGui.Combo("Sort Type", ref Unsafe.As<MeterDataType, int>(ref this._dataType), _meterTypeOptions, _meterTypeOptions.Length);

                ImGui.Checkbox("Return to Current Data when entering combat", ref this._returnToCurrent);
            }

            ImGui.EndChild();
        }
    }
}
