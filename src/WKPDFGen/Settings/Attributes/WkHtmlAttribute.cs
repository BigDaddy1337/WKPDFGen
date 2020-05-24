using System;

namespace WKPDFGen.Settings.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WkHtmlSettingAttribute : Attribute
    {
        public string Name { get; }

        public WkHtmlSettingAttribute(string name)
        {
            Name = name;
        }
    }
}
