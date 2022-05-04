using System;

namespace WKPDFGen.Settings;

[AttributeUsage(AttributeTargets.Property)]
public class WkHtmlSettingsAttribute : Attribute
{
    public string Name { get; }
    
    public bool IsGlobal { get; }
    
    public WkHtmlSettingsAttribute(string name, bool isGlobal = false)
    {
        Name = name;
        IsGlobal = isGlobal;
    }
}