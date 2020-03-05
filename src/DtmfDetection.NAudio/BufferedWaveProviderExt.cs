﻿namespace DtmfDetection.NAudio {
    using System;
    using System.Threading;
    using global::NAudio.Wave;

    /// <summary>Provides an extension method that waits until a `BufferedWaveProvider` has read enough data.</summary>
    public static class BufferedWaveProviderExt {
        /// <summary>Blocks the thread for as long as the `BufferedWaveProvider` minimally should need to buffer at least `count` sample frames. The wait time is estimated from the difference of the number of already buffered bytes to the number of requested bytes.</summary>
        /// <param name="source">The buffered source of input data.</param>
        /// <param name="count">The requested number of samples frames. Used to calucalte the number of requested bytes.</param>
        /// <returns>Returns `false` when the estimated wait time was sufficient to fill the buffer, or `false` when more waiting is needed.</returns>
        public static bool WaitForSamples(this BufferedWaveProvider source, int count) {
            var bytesPerSample = source.WaveFormat.BitsPerSample / 8 * source.WaveFormat.Channels;
            var bytesPerSampleBlock = bytesPerSample * count;
            var missingBytes = bytesPerSampleBlock - source.BufferedBytes;

            if (missingBytes > 0) {
                var missingSamples = (double)missingBytes / bytesPerSample;
                var waitTime = (int)Math.Round(missingSamples / source.WaveFormat.SampleRate * 1000);
                Thread.Sleep(Math.Max(waitTime, 1));
            }

            return source.BufferedBytes < missingBytes;
        }
    }
}
