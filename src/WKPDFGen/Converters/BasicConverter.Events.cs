using System;
using WKPDFGen.Settings;

namespace WKPDFGen.Converters
{
    public class ErrorArgs : EventArgs
    {
        public IUserConfig Document { get; set; }

        public string Message { get; set; }
    }
        
    public class PhaseChangedArgs : EventArgs
    {
        public IUserConfig Document { get; set; }

        public int PhaseCount { get; set; }

        public int CurrentPhase { get; set; }

        public string Description { get; set; }
    }
    
    public class FinishedArgs : EventArgs
    {
        public IUserConfig Document { get; set; }

        public bool Success { get; set; }
    }
    
    public class ProgressChangedArgs : EventArgs
    {
        public IUserConfig Document { get; set; }

        public string Description { get; set; }
    }
    
    public class WarningArgs : EventArgs
    {
        public IUserConfig Document { get; set; }

        public string Message { get; set; }
    }
    
    public partial class BasicConverter
    {
        public event EventHandler<PhaseChangedArgs> PhaseChanged;

        public event EventHandler<ProgressChangedArgs> ProgressChanged;

        public event EventHandler<FinishedArgs> Finished;

        public event EventHandler<ErrorArgs> Error;

        public event EventHandler<WarningArgs> Warning;
        
        private void OnPhaseChanged(IntPtr converter)
        {
            int currentPhase = wkHtmlToXService.GetCurrentPhase(converter);
            var eventArgs = new PhaseChangedArgs()
            {
                Document = ProcessingDocument,
                PhaseCount = wkHtmlToXService.GetPhaseCount(converter),
                CurrentPhase = currentPhase,
                Description = wkHtmlToXService.GetPhaseDescription(converter, currentPhase)
            };

            PhaseChanged?.Invoke(this, eventArgs);
        }

        private void OnProgressChanged(IntPtr converter)
        {
            var eventArgs = new ProgressChangedArgs()
            {
                Document = ProcessingDocument,
                Description = wkHtmlToXService.GetProgressString(converter)
            };

            ProgressChanged?.Invoke(this, eventArgs);
        }

        private void OnFinished(IntPtr converter, int success)
        {
            var eventArgs = new FinishedArgs
            {
                Document = ProcessingDocument,
                Success = success == 1
            };

            Finished?.Invoke(this, eventArgs);
        }

        private void OnError(IntPtr converter, string message)
        {
            var eventArgs = new ErrorArgs
            {
                Document = ProcessingDocument,
                Message = message
            };

            Error?.Invoke(this, eventArgs);
        }

        private void OnWarning(IntPtr converter, string message)
        {
            var eventArgs = new WarningArgs
            {
                Document = ProcessingDocument,
                Message = message
            };

            Warning?.Invoke(this, eventArgs);
        }
    }
}