using System;

namespace WKPDFGen.Exceptions;

public class WkHtmlToPdfException: Exception
{
    public WkHtmlToPdfException(string message): base(message)
    {
        
    }
    
    public static WkHtmlToPdfException UnknownExceptionWhileConverting => new("Failed to create pdf for unknown reason, try again with logs in debug mode");

}