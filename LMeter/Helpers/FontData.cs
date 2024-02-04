namespace LMeter.Helpers;

public struct FontData
{
    public string Name;
    public int Size;
    public bool Chinese;
    public bool Korean;

    public FontData(string name, int size, bool chinese, bool korean)
    {
        Name = name;
        Size = size;
        Chinese = chinese;
        Korean = korean;
    }
}