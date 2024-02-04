using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace LMeter.ACT.DataStructures;

public class Encounter
{
    [JsonIgnore]
    public static string[] TextTags { get; } = typeof(Encounter).GetFields().Select(x => $"[{x.Name.ToLower()}]").ToArray();

    private static readonly Random _rand = new Random();
    private static readonly Dictionary<string, MemberInfo> _members = typeof(Encounter).GetMembers().ToDictionary((x) => x.Name.ToLower());

    public string GetFormattedString(string format, string numberFormat)
    {
        return TextTagFormatter.TextTagRegex.Replace(format, new TextTagFormatter(this, numberFormat, _members).Evaluate);
    }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;

    [JsonPropertyName("DURATION")]
    private string _duration { get; set; } = string.Empty;
        
    [JsonPropertyName("encdps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? Dps { get; set; } 

    [JsonPropertyName("damage")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? DamageTotal { get; set; } 
        
    [JsonPropertyName("enchps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? Hps { get; set; } 

    [JsonPropertyName("healed")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? HealingTotal { get; set; } 

    [JsonPropertyName("damagetaken")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? DamageTaken { get; set; } 

    [JsonPropertyName("deaths")]
    public string? Deaths { get; set; } 

    [JsonPropertyName("kills")]
    public string? Kills { get; set; } 

    public static Encounter GetTestData()
    {
        float damage = _rand.Next(212345 * 8);
        float healing = _rand.Next(41234 * 8);

        return new Encounter()
        {
            Duration = "00:30",
            Title = "Preview",
            Dps = new LazyFloat(damage / 30),
            Hps = new LazyFloat(healing / 30),
            Deaths = "0",
            DamageTotal = new LazyFloat(damage),
            HealingTotal = new LazyFloat(healing)
        };
    }
}