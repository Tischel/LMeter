using System.Text.Json.Serialization;

namespace LMeter.Helpers;

public struct FontData
{
    public string Name { get; }
    public int Size { get; }
    public bool Chinese { get; }
    public bool Korean { get; }

    [JsonConstructor]
    public FontData(string name, int size, bool chinese, bool korean)
    {
        Name = name;
        Size = size;
        Chinese = chinese;
        Korean = korean;
    }
}