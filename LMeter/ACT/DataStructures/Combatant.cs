using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using LMeter.Enums;

namespace LMeter.ACT.DataStructures;

public class Combatant
{
    [JsonIgnore]
    public static string[] TextTags { get; } = typeof(Combatant).GetFields().Select(x => $"[{x.Name.ToLower()}]").ToArray();

    // TODO: move this to a global place so it can be shared between encounter and combatant
    private static readonly Random _rand = new Random();
    private static readonly Dictionary<string, MemberInfo> _members = typeof(Combatant).GetMembers().ToDictionary((x) => x.Name.ToLower());

    [JsonPropertyName("name")]
    public string OriginalName { get; set; } = string.Empty;
    
    public string? NameOverwrite { get; set; } = null;

    [JsonIgnore]
    public string Name => NameOverwrite ?? OriginalName;

    [JsonIgnore]
    public LazyString<string?>? Name_First;

    [JsonIgnore]
    public LazyString<string?>? Name_Last;

    [JsonIgnore]
    public string Rank = string.Empty;

    [JsonPropertyName("Job")]
    public Job Job { get; set; }

    [JsonIgnore]
    public LazyString<Job>? JobName;

    [JsonPropertyName("duration")]
    public string Duration { get; set; } = string.Empty;
        
    [JsonPropertyName("encdps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? EncDps { get; set; }

    [JsonPropertyName("dps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? Dps { get; set; }

    [JsonPropertyName("damage")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? DamageTotal { get; set; }

    [JsonPropertyName("damage%")]
    public string DamagePct { get; set; } = string.Empty;

    [JsonPropertyName("crithit%")]
    public string CritHitPct { get; set; } = string.Empty;

    [JsonPropertyName("DirectHitPct")]
    public string DirectHitPct { get; set; } = string.Empty;

    [JsonPropertyName("CritDirectHitPct")]
    public string CritDirectHitPct { get; set; } = string.Empty;
        
    [JsonPropertyName("enchps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? EncHps { get; set; }
        
    [JsonPropertyName("hps")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? Hps { get; set; }

    public LazyFloat? EffectiveHealing { get; set; }

    [JsonPropertyName("healed")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? HealingTotal { get; set; }

    [JsonPropertyName("healed%")]
    public string HealingPct  { get; set; }= string.Empty;

    [JsonPropertyName("overHeal")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? OverHeal { get; set; }

    [JsonPropertyName("OverHealPct")]
    public string OverHealPct  { get; set; }= string.Empty;

    [JsonPropertyName("damagetaken")]
    [JsonConverter(typeof(LazyFloatConverter))]
    public LazyFloat? DamageTaken { get; set; }

    [JsonPropertyName("deaths")]
    public string Deaths  { get; set; }= string.Empty;

    [JsonPropertyName("kills")]
    public string Kills  { get; set; }= string.Empty;

    [JsonPropertyName("maxhit")]
    public string MaxHit  { get; set; } = string.Empty;

    [JsonPropertyName("MAXHIT")]
    private string _maxHit  { get; set; } = string.Empty;

    public LazyString<string?> MaxHitName { get; set; }

    public LazyFloat? MaxHitValue { get; set; }

    public Combatant()
    {
        this.Name_First = new LazyString<string?>(() => this.Name, LazyStringConverters.FirstName);
        this.Name_Last = new LazyString<string?>(() => this.Name, LazyStringConverters.LastName);
        this.JobName = new LazyString<Job>(() => this.Job, LazyStringConverters.JobName);
        this.EffectiveHealing = new LazyFloat(() => (this.HealingTotal?.Value ?? 0) - (this.OverHeal?.Value ?? 0));
        this.MaxHitName = new LazyString<string?>(() => this.MaxHit, LazyStringConverters.MaxHitName);
        this.MaxHitValue = new LazyFloat(() => LazyStringConverters.MaxHitValue(this.MaxHit));
    }
    
    public string GetFormattedString(string format, string numberFormat)
    {
        return TextTagFormatter.TextTagRegex.Replace(format, new TextTagFormatter(this, numberFormat, _members).Evaluate);
    }

    public static Dictionary<string, Combatant> GetTestData()
    {
        Dictionary<string, Combatant> mockCombatants = new Dictionary<string, Combatant>();
        mockCombatants.Add("1", GetCombatant("GNB", "DRK", "WAR", "PLD"));
        mockCombatants.Add("2", GetCombatant("GNB", "DRK", "WAR", "PLD"));

        mockCombatants.Add("3", GetCombatant("WHM", "AST", "SCH", "SGE"));
        mockCombatants.Add("4", GetCombatant("WHM", "AST", "SCH", "SGE"));

        mockCombatants.Add("5", GetCombatant("SAM", "DRG", "MNK", "NIN", "RPR"));
        mockCombatants.Add("6", GetCombatant("SAM", "DRG", "MNK", "NIN", "RPR"));
        mockCombatants.Add("7", GetCombatant("BLM", "SMN", "RDM"));
        mockCombatants.Add("8", GetCombatant("DNC", "MCH", "BRD"));

        return mockCombatants;
    }

    private static Combatant GetCombatant(params string[] jobs)
    {
        int damage = _rand.Next(212345);
        int healing = _rand.Next(41234);

        return new Combatant()
        {
            OriginalName = "Firstname Lastname",
            Duration = "00:30",
            Job = Enum.Parse<Job>(jobs[_rand.Next(jobs.Length)]),
            DamageTotal = new LazyFloat(damage.ToString()),
            Dps = new LazyFloat((damage / 30).ToString()),
            EncDps = new LazyFloat((damage / 30).ToString()),
            HealingTotal = new LazyFloat(healing.ToString()),
            OverHeal = new LazyFloat(5000),
            Hps = new LazyFloat((healing / 30).ToString()),
            EncHps = new LazyFloat((healing / 30).ToString()),
            DamagePct = "100%",
            HealingPct = "100%",
            CritHitPct = "20%",
            DirectHitPct = "25%",
            CritDirectHitPct = "5%",
            DamageTaken = new LazyFloat((damage / 20).ToString()),
            Deaths = _rand.Next(2).ToString(),
            MaxHit = "Full Thrust-42069"
        };
    }
}