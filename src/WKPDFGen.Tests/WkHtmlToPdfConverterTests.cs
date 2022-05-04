using System.IO;
using System.Linq;
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
        
        private static WkHtmlWrapper wkHtmlWrapper;
        private static WkHtmlToPdfConverter converter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            html = GetAsset("Simple.html");
            
            var pathToLibFolder = Helper.GetLibFolder();
            
            option = Options.Create(new WkPdfOptions
            {
                LinuxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.so"),
                WindowsLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dll"),
                OsxLibPath = Path.Combine(pathToLibFolder, "libwkhtmltox.dylib")
            });
            
            var loggerWrapperMock = new Mock<ILogger<WkHtmlWrapper>>();
            wkHtmlWrapper = new WkHtmlWrapper(option, loggerWrapperMock.Object);
            
            var loggerMock = new Mock<ILogger<WkHtmlToPdfConverter>>();
            converter = new WkHtmlToPdfConverter(wkHtmlWrapper, loggerMock.Object);
        }

        [Test]
        public void GetLibraryVersionTest()
        {
            var version = wkHtmlWrapper.GetLibraryVersion();
            
            Assert.AreEqual("0.12.6", version);
        }
        
        [Test]
        public async Task CreateTest()
        {
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
        
        [Test]
        public async Task CreateSeveralTimesNotCorruptedFileTest()
        {
            var firstStream = await converter.CreateAsync(html);
            
            Assert.IsNotNull(firstStream);

            var expectedStreamLength = firstStream.Length;

            await Task.WhenAll(Enumerable.Range(0, 15)
                                         .Select(async index =>
                                         {
                                             var stream = await converter.CreateAsync(html);
                                             
                                             Assert.IsNotNull(stream);
                                             
                                             Assert.AreEqual(expectedStreamLength, stream.Length, $"Stream {index} has corrupted file");
                                         }));
        }
        
        [Test]
        public async Task CreateonDiskSeveralTimesNotCorruptedFileTest()
        {
            const string path = "CreateOnDiskAsyncTest.pdf";
            
            await converter.CreateOnDiskAsync(html, path);

            Assert.IsTrue(File.Exists(path));

            var firstfileBytes = await File.ReadAllBytesAsync(path);

            var expectedFileBytesCount = firstfileBytes.Length;
            
            File.Delete(path);

            await Task.WhenAll(Enumerable.Range(0, 15)
                                         .Select(async index =>
                                         {
                                             await converter.CreateOnDiskAsync(html, path);
                                             
                                             Assert.IsTrue(File.Exists(path));

                                             var fileBytes = await File.ReadAllBytesAsync(path);

                                             Assert.AreEqual(expectedFileBytesCount, fileBytes.Length, $"Bytes {index} has corrupted file");
                                         }));
        }
    }
}