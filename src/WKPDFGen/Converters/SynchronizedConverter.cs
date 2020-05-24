using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using WKPDFGen.Library;
using WKPDFGen.Settings;

namespace WKPDFGen.Converters
{
    public class SynchronizedConverter : BasicConverter
    {
        private Thread conversionThread;

        private readonly BlockingCollection<Task> conversionsQueue = new BlockingCollection<Task>();
        
        private readonly object startLock = new object();

        public SynchronizedConverter(IWkHtmlToXService wkHtmlToXService) : base(wkHtmlToXService)
        {
        }

        public override byte[] Convert(IUserConfig document)
        {
            StartThreadIfNotStarted();

            var task = new Task<byte[]>(() => base.Convert(document));

            lock (task)
            {
                // add task to blocking collection
                
                conversionsQueue.Add(task);
                
                // wait for task to be processed by conversion thread 
                
                Monitor.Wait(task);
            }

            // throw exception that happened during conversion

            if (task.Exception != null)
            {
                throw task.Exception;
            }

            return task.Result;
        }
        
        private void StartThreadIfNotStarted()
        {
            lock (startLock)
            {
                if (conversionThread != null) return;
                
                conversionThread = new Thread(ProcessConverionFromQueue)
                {
                    IsBackground = true,
                    Name = "wkhtmltopdf worker thread"
                };
                
                conversionThread.Start();
            }
        }
        
        private void ProcessConverionFromQueue()
        {
            while (true)
            {
                var task = conversionsQueue.Take();

                lock (task)
                {
                    task.RunSynchronously();

                    // notify caller thread that task is completed
                    Monitor.Pulse(task);
                }
            }
        }
    }
}
