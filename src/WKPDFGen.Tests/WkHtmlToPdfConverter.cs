using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using WKPDFGen.Converters;
using WKPDFGen.Library;

namespace WKPDFGen.Tests
{
    public class Tests
    {
        private static string GetAsset(string name) => File.ReadAllText(Path.Combine("Assets", name));

        private string html;
        
        private const int pdfBytesLength = 18854;

        private IOptions<WkPdfOptions> option;

        [SetUp]
        public void Setup()
        {
            html = GetAsset("Simple.html");
            
            var pathToLibFolder = Helper.GetLibFolder();
            
            option = Options.Create(new WkPdfOptions
            {
                LinuxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.so"),
                WindowsLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dll"),
                OsxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dylib")
            });
        }

        [Test]
        public void GetLibraryVersionTest()
        {            
            var loggerWrapperMock = new Mock<ILogger<WkHtmlWrapper>>();

            var service = new WkHtmlWrapper(option, loggerWrapperMock.Object);

            var version = service.GetLibraryVersion();
            
            Assert.AreEqual("0.12.6", version);
        }
        
        [Test]
        public async Task CreateTest()
        {
            var loggerWrapperMock = new Mock<ILogger<WkHtmlWrapper>>();
            var loggerMock = new Mock<ILogger<WkHtmlToPdfConverter>>();

            var service = new WkHtmlWrapper(option, loggerWrapperMock.Object);
            var converter = new WkHtmlToPdfConverter(service, loggerMock.Object);
            
            var stream = await converter.CreateAsync(html);
            
            Assert.IsNotNull(stream);
            Assert.AreEqual(pdfBytesLength, stream.Length);
            
            const string path = "CreateOnDiskAsyncTest.pdf";
            
            await converter.CreateOnDiskAsync(html, path);

            Assert.IsTrue(File.Exists(path));

            var fileBytes = await File.ReadAllBytesAsync(path);
            
            Assert.AreEqual(pdfBytesLength, fileBytes.Length);
            
            File.Delete(path);
        }
    }
}