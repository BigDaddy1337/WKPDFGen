using System.Collections.Generic;
using WKPDFGen.Settings.Settings;

namespace WKPDFGen.Settings
{
    public interface IUserSettings {}
    
    public interface IUserConfig : IUserSettings
    {
        IEnumerable<PdfSettings> GetConfigs();
    }
    
    public class PdfConfig : IUserConfig
    {
        public List<PdfSettings> PdfSettings { get; set; } = new List<PdfSettings>();

        public GlobalPdfSettings GlobalPdfSettings { get; set; } = new GlobalPdfSettings();
        
        public IEnumerable<PdfSettings> GetConfigs()
        {
            return PdfSettings.ToArray();
        }   
    }
}
