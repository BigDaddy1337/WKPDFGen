using System;
using Microsoft.Extensions.Logging;
using WKPDFGen.Exceptions;

namespace WKPDFGen.Converters;

public partial class WkHtmlToPdfConverter
{
    private void OnPhaseChanged(IntPtr converter)
    {
        var currentPhase = wkHtmlWrapper.GetCurrentPhase(converter);
        var phaseDescription = wkHtmlWrapper.GetPhaseDescription(converter, currentPhase);
        
        logger.LogDebug("[WkHTMLtoPDF] [PhaseChanged] Converting phase changed to {currentPhase} ({phaseDescription})", currentPhase, phaseDescription);
    }

    private void OnProgressChanged(IntPtr converter)
    {
        var progressDescription = wkHtmlWrapper.GetProgressString(converter);
        
        logger.LogDebug("[WkHTMLtoPDF] [ProgressChanged] {progressDescription}", progressDescription);
    }

    private void OnFinished(IntPtr converter, int success)
    {
        if (success == 1)
            logger.LogDebug("[WkHTMLtoPDF] [Finished] Object successfuly converted");
        else
            logger.LogDebug("[WkHTMLtoPDF] [Finished] Conversion failed");
    }

    private void OnError(IntPtr converter, string message)
    {
        throw new WkHtmlToPdfException(message);
    }

    private void OnWarning(IntPtr converter, string message)
    {
        logger.LogWarning("[WkHTMLtoPDF] [Warning] {message}", message);
    }
}