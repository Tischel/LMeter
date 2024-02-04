using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LMeter.ACT.DataStructures
{
    public class ACTEvent
    {
        private bool _parsedActive;
        private bool _active;
        public DateTime Timestamp;

        [JsonPropertyName("type")]
        public string EventType { get; set; } = string.Empty;
        
        [JsonPropertyName("isActive")]
        public string IsActive { get; set; } = string.Empty;
        
        [JsonPropertyName("Encounter")]
        public Encounter? Encounter { get; set; }
        
        [JsonPropertyName("Combatant")]
        public Dictionary<string, Combatant>? Combatants { get; set; }

        public bool IsEncounterActive()
        {
            if (_parsedActive)
            {
                return _active;
            }
            
            bool.TryParse(this.IsActive, out _active);
            _parsedActive = true;
            return _active;
        }

        public static ACTEvent GetTestData()
        {
            return new ACTEvent()
            {
                Encounter = Encounter.GetTestData(),
                Combatants = Combatant.GetTestData()
            };
        }
    }
}