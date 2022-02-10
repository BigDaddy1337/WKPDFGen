﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WKPDFGen.Settings;

namespace WKPDFGen;

public interface IUserSettings
{
    // Just a marker
}
    
public record LibrarySetting(string Name, object Value, bool IsGlobal);

public interface IConfiguration
{
    List<LibrarySetting> LibrarySettings { get; }
}
    
public abstract class ConfigurationBase: IConfiguration
{
    protected ConfigurationBase(params IUserSettings[] userSettingCollections)
    {
        FillLibrarySettings(userSettingCollections);
    }
    
    public List<LibrarySetting> LibrarySettings { get; } = new();

    private void FillLibrarySettings(params IUserSettings[] userSettingCollections)
    {
        foreach (var userSettingCollection in userSettingCollections)
        {
            var isGlobalSettings = userSettingCollection is GlobalPdfSettings;

            var properties = userSettingCollection
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(userSettingCollection);
                if (propertyValue is null) continue;

                var attributes = property
                    .GetCustomAttributes()
                    .ToArray();

                if (attributes.Length > 0 && attributes[0] is WkHtmlSettingsAttribute attribute)
                {
                    LibrarySettings.Add(new LibrarySetting(attribute.Name, propertyValue, isGlobalSettings));
                }
                else if (propertyValue is IUserSettings childSetting)
                {
                    FillLibrarySettings(childSetting);
                }
            }
        }
    }
}
    
    
// ReSharper disable once InconsistentNaming
public class PDFConfiguration : ConfigurationBase
{
    public PDFConfiguration(PdfSettings pdfSettings, GlobalPdfSettings globalPdfSettings)
        : base(pdfSettings, globalPdfSettings)
    {
    }
}