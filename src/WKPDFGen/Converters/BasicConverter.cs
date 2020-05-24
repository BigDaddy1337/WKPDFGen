using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WKPDFGen.Library;
using WKPDFGen.Settings;
using WKPDFGen.Settings.Attributes;

namespace WKPDFGen.Converters
{
    public interface IConverter
    {
        /// <summary>
        ///  Converts document based on given settings
        /// </summary>
        /// <param name="document">Document to convert</param>
        /// <returns>Returns converted document in bytes</returns>
        byte[] Convert(IUserConfig document);

        event EventHandler<PhaseChangedArgs> PhaseChanged;

        event EventHandler<ProgressChangedArgs> ProgressChanged;

        event EventHandler<FinishedArgs> Finished;

        event EventHandler<ErrorArgs> Error;

        event EventHandler<WarningArgs> Warning;
    }
    
    public partial class BasicConverter : IConverter
    {
        private readonly IWkHtmlToXService wkHtmlToXService;

        public IUserConfig ProcessingDocument { get; private set; }

        public BasicConverter(IWkHtmlToXService wkHtmlToXService)
        {
            this.wkHtmlToXService = wkHtmlToXService;
        }

        public virtual byte[] Convert(IUserConfig document)
        {
            if (!document.GetConfigs().Any())
            {
                throw new ArgumentException("At least one object must be defined.");
            }

            ProcessingDocument = document;

            var result = new byte[0];
            
            wkHtmlToXService.Load();

            var converter = CreateConverter(document);

            // register events
            
            wkHtmlToXService.SetPhaseChangedCallback(converter, OnPhaseChanged);
            wkHtmlToXService.SetProgressChangedCallback(converter, OnProgressChanged);
            wkHtmlToXService.SetFinishedCallback(converter, OnFinished);
            wkHtmlToXService.SetWarningCallback(converter, OnWarning);
            wkHtmlToXService.SetErrorCallback(converter, OnError);

            var converted = wkHtmlToXService.DoConversion(converter);

            if (converted)
            {
                result = wkHtmlToXService.GetConversionResult(converter);
            }

            wkHtmlToXService.DestroyConverter(converter);

            return result;
        }

        private IntPtr CreateConverter(IUserConfig document)
        {
            var globalSettings = wkHtmlToXService.CreateGlobalSettings();

            ApplyConfig(globalSettings, document, true);

            var converter = wkHtmlToXService.CreateConverter(globalSettings);
            
            foreach (var userConfig in document.GetConfigs())
            {
                if (userConfig == null) continue;
                
                var objectSettings = wkHtmlToXService.CreateObjectSettings();

                ApplyConfig(objectSettings, userConfig, false);

                wkHtmlToXService.AddObject(converter, objectSettings, userConfig.GetHtmlContent());
            }

            return converter;
        }

        private void ApplyConfig(IntPtr config, IUserSettings settings, bool isGlobal)
        {
            if (settings == null) return;
            
            var props = settings
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var prop in props)
            {
                var attrs = prop
                    .GetCustomAttributes()
                    .ToArray();
                
                var propValue = prop.GetValue(settings);

                if (propValue == null)  continue;

                if (attrs.Length > 0 && attrs[0] is WkHtmlSettingAttribute attr)
                {
                    ApplyConcreteSetting(config, attr.Name, propValue, isGlobal);
                }
                else if (propValue is IUserSettings childSetting)
                {
                    ApplyConfig(config, childSetting, isGlobal);
                }
            }
        }

        private void ApplyConcreteSetting(IntPtr config, string name, object value, bool isGlobal)
        {
            var type = value.GetType();

            if (typeof(bool) == type)
            {
                wkHtmlToXService.SetSetting(config, name, (bool) value ? "true" : "false", isGlobal);
            }
            else if (typeof(double) == type)
            {
                wkHtmlToXService.SetSetting(config, name, ((double) value).ToString("0.##", CultureInfo.InvariantCulture), isGlobal);
            }
            else if (typeof(Dictionary<string, string>).IsAssignableFrom(type))
            {
                var dictionary = (Dictionary<string, string>) value;
                
                var index = 0;

                foreach (var (key, s) in dictionary)
                {
                    if (key == null || s == null) continue;

                    // https://github.com/wkhtmltopdf/wkhtmltopdf/blob/c754e38b074a75a51327df36c4a53f8962020510/src/lib/reflect.hh#L192
                    
                    wkHtmlToXService.SetSetting(config, name + ".append", null, isGlobal);
                    wkHtmlToXService.SetSetting(config, $"{name}[{index}]", key + "\n" + s, isGlobal);

                    index++;
                }
            }
            else
            {
                wkHtmlToXService.SetSetting(config, name, value.ToString(), isGlobal);
            }
        }
    }
}