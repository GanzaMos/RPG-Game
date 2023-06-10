using System;
using RPG.Attributes;

public static class Utilities
{
    public static T InstError<T>()
    {
        string className = typeof(T).Name;
        throw new Exception($"Missing {className} component");
    }
}
