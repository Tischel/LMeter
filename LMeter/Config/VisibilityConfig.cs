using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using ImGuiNET;
using LMeter.Helpers;
using System.Linq;
using System.Collections.Generic;
using LMeter.ACT;
using LMeter.Enums;

namespace LMeter.Config
{

    public class VisibilityConfig : IConfigPage
    {
        public string Name => "Visibility";
        
        public IConfigPage GetDefault() => new VisibilityConfig();

        private string _customJobInput = string.Empty;
        private string _hideIfValueInput = string.Empty;
        private bool _alwaysHide;
        private bool _hideInCombat;
        private bool _hideOutsideCombat;
        private bool _hideOutsideDuty;
        private bool _hideWhilePerforming;
        private bool _hideInGoldenSaucer;
        private bool _hideIfNotConnected;
        private JobType _showForJobTypes = JobType.All;

        public bool AlwaysHide
        {
            get => _alwaysHide;
            set => _alwaysHide = value;
        }

        public bool HideInCombat
        {
            get => _hideInCombat;
            set => _hideInCombat = value;
        }

        public bool HideOutsideCombat
        {
            get => _hideOutsideCombat;
            set => _hideOutsideCombat = value;
        }

        public bool HideOutsideDuty
        {
            get => _hideOutsideDuty;
            set => _hideOutsideDuty = value;
        }

        public bool HideWhilePerforming
        {
            get => _hideWhilePerforming;
            set => _hideWhilePerforming = value;
        }

        public bool HideInGoldenSaucer
        {
            get => _hideInGoldenSaucer;
            set => _hideInGoldenSaucer = value;
        }

        public bool HideIfNotConnected
        {
            get => _hideIfNotConnected;
            set => _hideIfNotConnected = value;
        }

        public JobType ShowForJobTypes
        {
            get => _showForJobTypes;
            set => _showForJobTypes = value;
        }

        public string CustomJobString { get; set; } = string.Empty;
        public List<Job> CustomJobList { get; set; } = new List<Job>();

        public bool IsVisible()
        {
            if (this.AlwaysHide)
            {
                return false;
            }

            if (this.HideInCombat && CharacterState.IsInCombat())
            {
                return false;
            }

            if (this.HideOutsideCombat && !CharacterState.IsInCombat())
            {
                return false;
            }

            if (this.HideOutsideDuty && !CharacterState.IsInDuty())
            {
                return false;
            }

            if (this.HideWhilePerforming && CharacterState.IsPerforming())
            {
                return false;
            }

            if (this.HideInGoldenSaucer && CharacterState.IsInGoldenSaucer())
            {
                return false;
            }

            if (this.HideIfNotConnected && ACTClient.Status != ConnectionStatus.Connected)
            {
                return false;
            }

            return CharacterState.IsJobType(CharacterState.GetCharacterJob(), this.ShowForJobTypes, this.CustomJobList);
        }

        public void DrawConfig(Vector2 size, float padX, float padY)
        {
            if (ImGui.BeginChild($"##{this.Name}", new Vector2(size.X, size.Y), true))
            {
                ImGui.Checkbox("Always Hide", ref this._alwaysHide);
                ImGui.Checkbox("Hide In Combat", ref this._hideInCombat);
                ImGui.Checkbox("Hide Outside Combat", ref this._hideOutsideCombat);
                ImGui.Checkbox("Hide Outside Duty", ref this._hideOutsideDuty);
                ImGui.Checkbox("Hide While Performing", ref this._hideWhilePerforming);
                ImGui.Checkbox("Hide In Golden Saucer", ref this._hideInGoldenSaucer);
                ImGui.Checkbox("Hide While Not Connected to ACT", ref this._hideIfNotConnected);
                
                DrawHelpers.DrawSpacing(1);
                string[] jobTypeOptions = Enum.GetNames(typeof(JobType));
                ImGui.Combo("Show for Jobs", ref Unsafe.As<JobType, int>(ref this._showForJobTypes), jobTypeOptions, jobTypeOptions.Length);

                if (this.ShowForJobTypes == JobType.Custom)
                {
                    if (string.IsNullOrEmpty(_customJobInput))
                    {
                        _customJobInput = this.CustomJobString.ToUpper();
                    }

                    if (ImGui.InputTextWithHint("Custom Job List", "Comma Separated List (ex: WAR, SAM, BLM)", ref _customJobInput, 100, ImGuiInputTextFlags.EnterReturnsTrue))
                    {
                        IEnumerable<string> jobStrings = _customJobInput.Split(',').Select(j => j.Trim());
                        List<Job> jobList = new List<Job>();
                        foreach (string j in jobStrings)
                        {
                            if (Enum.TryParse(j, true, out Job parsed))
                            {
                                jobList.Add(parsed);
                            }
                            else
                            {
                                jobList.Clear();
                                _customJobInput = string.Empty;
                                break;
                            }
                        }

                        _customJobInput = _customJobInput.ToUpper();
                        this.CustomJobString = _customJobInput;
                        this.CustomJobList = jobList;
                    }
                }
            }
            
            ImGui.EndChild();
        }
    }
}