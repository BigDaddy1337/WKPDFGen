using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WKPDFGen.Converters;

public partial class WkHtmlToPdfConverter
{
    private record ProcessingItem(string Html,
                                  string? OutputPath,
                                  IConfiguration? Configuration,
                                  TaskCompletionSource<Stream> TaskCompletionSource,
                                  CancellationToken Token);

    private Thread? proccesingThread;
    
    private static readonly Mutex StartingThreadMutex = new();

    private readonly BlockingCollection<ProcessingItem> processingQueue = new();

    private void AddItemToProcessingQueue(ProcessingItem item)
    {
        StartProcessingThread();
        
        processingQueue.Add(item);
    }

    private void StartProcessingThread()
    {
        if (proccesingThread != null) return;

        StartingThreadMutex.WaitOne();

        if (proccesingThread != null) return;

        proccesingThread = new Thread(ProcessingThreadWorker)
        {
            IsBackground = true,
            Name = "WkHTMLtoPDF Processor"
        };

        proccesingThread.Start();

        logger.LogInformation("[WkHTMLtoPDF] Started background thread for wkhtmltopdf with name {ProccesingThreadName} and id {ProccesingThreadID}", proccesingThread.Name, proccesingThread.ManagedThreadId);

        StartingThreadMutex.ReleaseMutex();
    }

    private void ProcessingThreadWorker()
    {
        while (true)
        {
            var (html, path, configuration, taskCompletionSource, token) = processingQueue.Take();

            if (processingQueue.Count > 0)
                logger.LogInformation("[WkHTMLtoPDF] Started process item (path: {ProccesingItemPath}) from queue, current queue length {ProccesingThreadQueueLength}", path, processingQueue.Count);

            try
            {
                token.ThrowIfCancellationRequested();

                var result = Create(html, path, configuration);

                taskCompletionSource.SetResult(result);
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}