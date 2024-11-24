namespace FES_data_generator.Model;

public record Constraint
{
    public Constraint(bool required, int singleParam)
    {
        Required = required;
        SingleParam = singleParam;
    }
    public Constraint(bool required, int[] arrayParam)
    {
        Required = required;
        ArrayParam = arrayParam;
    }
    public Constraint(bool required, Dictionary<int, int> dictParam)
    {
        Required = required;
        DictParam = dictParam;
    }
    public Constraint(bool required, Dictionary<int, int[]> arrayDictParam)
    {
        Required = required;
        ArrayDictParam = arrayDictParam;
    }
    public bool Required { get; init; }
    public int? SingleParam { get; init; }
    public int[]? ArrayParam { get; init; }
    public Dictionary<int, int>? DictParam { get; init; }
    public Dictionary<int, int[]>? ArrayDictParam { get; init; }

    public override string ToString()
    {
        if (SingleParam.HasValue)
        {
            return SingleParam.Value.ToString();
        }
        if (ArrayParam is not null)
        {
            return $"[{string.Join(", ", ArrayParam)}]";
        }
        if (DictParam is not null)
        {
            return $"[|{string.Join(", ", DictParam.Keys)}|{string.Join(", ", DictParam.Values)}|]";
        }
        if (ArrayDictParam is not null)
        {
            return $"[{string.Join(", ", ArrayDictParam.Values.Select(p => string.Join("..",p)))}]";
        }
        return string.Empty;
    }
}
