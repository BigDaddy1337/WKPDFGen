using System;

namespace WKPDFGen.Settings;

[AttributeUsage(AttributeTargets.Property)]
public class WkHtmlSettingsAttribute : Attribute
{
    public string Name { get; }

    public WkHtmlSettingsAttribute(string name)
    {
        Name = name;
    }
}