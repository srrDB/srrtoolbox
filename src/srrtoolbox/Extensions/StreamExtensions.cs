using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace srrtoolbox.Extensions
{
    public static class StreamExtensions
    {
        public static async Task CopyToAsyncWithProgress(this Stream source, Stream destination, IProgress<long> progress, CancellationToken cancellationToken = default(CancellationToken), int bufferSize = 0x1000)
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;
            long totalRead = 0;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                totalRead += bytesRead;
                progress.Report(totalRead);
            }
        }
    }
}
