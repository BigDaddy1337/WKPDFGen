using System;
using System.IO;

namespace WKPDFGen.Tests
{
    public static class Helper
    {
        public static string GetLibFolder()
        {
            var solutionDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)
                .Parent.Parent.Parent.Parent.Parent.FullName;
            
            return Path.Combine(solutionDir, "examples", "AspNetCoreExample", "0.12.5");
        }
    }
}