using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;
using static AudioManager;

namespace NitronicHUD
{
    public struct AudioClip : IDisposable
    {
        #region Fields and Constructors
        public static readonly AudioClip Empty = new AudioClip();

        public bool IsEmptyClip => Equals(Empty) || !COUNTDOWN_ANNOUNCER.AllowCustomMusic_;
        private AudioFileReader2 AudioFile;
        private SampleAggregator Aggregator;
        private FadeInOutSampleProvider FadeInOut;
        private WaveOutEvent OutputDevice;

        public AudioClip(string fileName)
        {
            if (File.Exists(fileName))
            {
                AudioFile = new AudioFileReader2(fileName);
                Aggregator = new SampleAggregator(AudioFile, 1024)
                {
                    PerformFFT = false
                };
                FadeInOut = new FadeInOutSampleProvider(Aggregator, false);
                OutputDevice = new WaveOutEvent()
                {
                    NumberOfBuffers = 10,
                    DesiredLatency = 85
                };
                OutputDevice.Init(FadeInOut);

            }
            else this = Empty;
        }

        public void Dispose()
        {
            OutputDevice?.Stop();
            OutputDevice?.Dispose();
            OutputDevice = null;

            Aggregator = null;
            FadeInOut = null;

            AudioFile?.Close();
            AudioFile?.Dispose();
            AudioFile = null;
        }
        #endregion

        #region Public Mehods
        public void Play()
        {
            if (!IsEmptyClip)
                OutputDevice.Play();
        }

        public void PlayFromStart()
        {
            if (!IsEmptyClip)
            {
                AudioFile.Position = 0;
                Play();
            }
        }

        public void Stop()
        {
            if(!IsEmptyClip)
                OutputDevice.Play();
        }

        public void SetVolume(float volume)
        {
            if (!IsEmptyClip)
                AudioFile.Volume = volume;
        }
        #endregion
    }
}