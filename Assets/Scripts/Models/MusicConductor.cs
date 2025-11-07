using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System;

namespace RaveSurvival
{
    public class MusicConductor : MonoBehaviour
    {
        public static MusicConductor Instance;

        [Header("Track")]
        public AudioClip track;
        public bool playOnStart = true;

        [Header("Analysis")]
        public AudioMixerGroup analysisMixerGroup;  
        public AudioAnalyzer analyzer;

        private AudioSource analysisSource;

        [SerializeField]
        private List<Speaker> speakers = new();
        private bool isPlaying = false;
        //private int masterSamples = 0;

        // Expose timing to others
        public double DspStartTime { get; private set; }
        public event Action<double> OnSongStarted;
        public event Action OnSongStopped;


        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Build analysis source
            analysisSource = gameObject.AddComponent<AudioSource>();
            analysisSource.outputAudioMixerGroup = analysisMixerGroup;
            //analysisSource.loop = true;
            analysisSource.playOnAwake = false;
            // 2D sound
            analysisSource.spatialBlend = 0f; 
            analysisSource.volume = 1f;
        }

        public void Register(Speaker s)
        {
            if (!speakers.Contains(s))
            {
                speakers.Add(s);
            }
            if (track != null && playOnStart)
            {
                StartTrack();
            }
        }

        public void Unregister(Speaker s) => speakers.Remove(s);

        public void StartTrack()
        {
            if (track == null || speakers.Count == 0)
            {
                return;
            }
        
            // Get DSP timing info and buffer size so we can align start to buffer boundary
            int dspBufferSize, dspNumBuffers;
            AudioSettings.GetDSPBufferSize(out dspBufferSize, out dspNumBuffers);
            int sampleRate = AudioSettings.outputSampleRate;
        
            // Give the audio system a bit more lead time to schedule sources reliably
            double leadTime = 0.12; // seconds (increase if you still see jitter)
            double desiredStart = AudioSettings.dspTime + leadTime;
        
            // Align to next DSP buffer boundary to avoid sample misalignment jitter
            double secondsPerBuffer = (double)dspBufferSize / sampleRate;
            double remainder = desiredStart % secondsPerBuffer;
            if (remainder > 0.0)
            {
                desiredStart += secondsPerBuffer - remainder;
            }
        
            DspStartTime = desiredStart;
        
            // Ensure each source is stopped / reset and scheduled at the exact same DSP time
            foreach (Speaker s in speakers)
            {
                AudioSource src = s.source;
                src.Stop();
                src.clip = track;
                src.timeSamples = 0;
                // Optional: ensure loop/playOnAwake states consistent across sources
                // src.loop = true;
                src.PlayScheduled(DspStartTime);
                DebugManager.Instance.Print(src.gameObject.name, DebugManager.DebugLevel.Paul);
            }
        
            analysisSource.Stop();
            analysisSource.clip = track;
            analysisSource.timeSamples = 0;
            analysisSource.PlayScheduled(DspStartTime);
        
            isPlaying = true;
        
            // Notify listeners the song is scheduled to start at DspStartTime
            OnSongStarted?.Invoke(DspStartTime);
        }

        public AudioSource GetAnalysisSource() => analysisSource;

        public void StopTrack()
        {
            if (!isPlaying)
            {
                return;
            }
            foreach (Speaker s in speakers)
            {
                s.source.Stop();
            }

            isPlaying = false;
        }

        public void SetTrack(AudioClip audioClip)
        {
            track = audioClip;
        }
    }
}