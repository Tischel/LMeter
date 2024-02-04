using System;
using System.Numerics;
using Dalamud.Interface;
using ImGuiNET;
using LMeter.ACT;
using LMeter.Helpers;
using System.Text.Json.Serialization;

namespace LMeter.Config
{
    public class ACTConfig : IConfigPage
    {
        private const string _defaultSocketAddress = "ws://127.0.0.1:10501/ws";

        private DateTime? _lastCombatTime = null;
        private DateTime? _lastReconnectAttempt = null;
        private string _actSocketAddress = _defaultSocketAddress;
        private int _encounterHistorySize = 15;
        private bool _autoReconnect;
        private int _reconnectDelay = 30;
        private bool _clearAct;
        private bool _autoEnd;
        private int _autoEndDelay = 3;

        public string Name => "ACT";

        public string ActSocketAddress
        {
            get => _actSocketAddress;
            set => _actSocketAddress = value;
        }

        public int EncounterHistorySize
        {
            get => _encounterHistorySize;
            set => _encounterHistorySize = value;
        }

        public bool AutoReconnect
        {
            get => _autoReconnect;
            set => _autoReconnect = value;
        }

        public int ReconnectDelay
        {
            get => _reconnectDelay;
            set => _reconnectDelay = value;
        }

        public bool ClearAct
        {
            get => _clearAct;
            set => _clearAct = value;
        }

        public bool AutoEnd
        {
            get => _autoEnd;
            set => _autoEnd = value;
        }

        public int AutoEndDelay
        {
            get => _autoEndDelay;
            set => _autoEndDelay = value;
        }

        public IConfigPage GetDefault() => new ACTConfig();

        public void DrawConfig(Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild($"##{this.Name}", new Vector2(size.X, size.Y), true))
            {
                Vector2 buttonSize = new Vector2(40, 0);
                ImGui.Text($"ACT Status: {ACTClient.Status}");
                ImGui.InputTextWithHint("ACT Websocket Address", $"Default: '{_defaultSocketAddress}'", ref this._actSocketAddress, 64);
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Sync, () => ACTClient.RetryConnection(this.ActSocketAddress), "Reconnect", buttonSize);

                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 1f);
                ImGui.Text("Retry ACT Connection");

                ImGui.NewLine();
                ImGui.PushItemWidth(30);
                ImGui.InputInt("Number of Encounters to save", ref this._encounterHistorySize, 0, 0);
                ImGui.PopItemWidth();

                ImGui.NewLine();
                ImGui.Checkbox("Automatically attempt to reconnect if connection fails", ref this._autoReconnect);
                if (this.AutoReconnect)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.PushItemWidth(30);
                    ImGui.InputInt("Seconds between reconnect attempts", ref this._reconnectDelay, 0, 0);
                    ImGui.PopItemWidth();
                }


                ImGui.NewLine();
                ImGui.Checkbox("Clear ACT when clearing LMeter", ref this._clearAct);
                ImGui.Checkbox("Force ACT to end encounter after combat", ref this._autoEnd);
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetTooltip("It is recommended to disable ACT Command Sounds if you use this feature.\n" +
                                     "The option can be found in ACT under Options -> Sound Settings.");
                }
                
                if (this.AutoEnd)
                {
                    DrawHelpers.DrawNestIndicator(1);
                    ImGui.PushItemWidth(30);
                    ImGui.InputInt("Seconds delay after combat", ref this._autoEndDelay, 0, 0);
                    ImGui.PopItemWidth();
                }

                ImGui.NewLine();
                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Stop, () => ACTClient.EndEncounter(), null, buttonSize);
                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 1f);
                ImGui.Text("Force End Combat");

                DrawHelpers.DrawButton(string.Empty, FontAwesomeIcon.Trash, () => Singletons.Get<PluginManager>().Clear(), null, buttonSize);
                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 1f);
                ImGui.Text("Clear LMeter");
            }
            
            ImGui.EndChild();
        }

        public void TryReconnect()
        {
            if (this._lastReconnectAttempt.HasValue &&
                (ACTClient.Status == ConnectionStatus.NotConnected ||
                ACTClient.Status == ConnectionStatus.ConnectionFailed))
            {
                if (this.AutoReconnect &&
                    this._lastReconnectAttempt < DateTime.UtcNow - TimeSpan.FromSeconds(this.ReconnectDelay))
                {
                    ACTClient.RetryConnection(this.ActSocketAddress);
                    this._lastReconnectAttempt = DateTime.UtcNow;
                }
            }
            else
            {
                this._lastReconnectAttempt = DateTime.UtcNow;
            }
        }

        public void TryEndEncounter()
        {
            if (ACTClient.Status == ConnectionStatus.Connected)
            {
                if (this.AutoEnd &&
                    CharacterState.IsInCombat())
                {
                    this._lastCombatTime = DateTime.UtcNow;
                }
                else if (this._lastCombatTime is not null && 
                         this._lastCombatTime < DateTime.UtcNow - TimeSpan.FromSeconds(this.AutoEndDelay))
                {
                    ACTClient.EndEncounter();
                    this._lastCombatTime = null;
                }
            }
        }
    }
}
