using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

namespace AUDIO
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        public const string MASTER_VOLUME_PARAMETER_NAME = "masterVolume";
        public const string MUSIC_VOLUME_PARAMETER_NAME = "musicVolume";
        public const string SFX_VOLUME_PARAMETER_NAME = "soundFXVolume";

        public const float MUTED_VOLUME_LEVEL = -80f;
        public const float TRACK_TRANSITION_SPEED = 1f;
        private const string SFX_PARENT_NAME = "SFX";

        public static char[] SFX_NAME_FORMAT_CONTAINERS = new char[] { '[', ']' };
        private static string SFX_NAME_FORMAT = $"SFX - {SFX_NAME_FORMAT_CONTAINERS[0]}" + "{0}" + $"{SFX_NAME_FORMAT_CONTAINERS[1]}";

        [Header("Audio Channels")]
        public Dictionary<int, AudioChannel> channels = new Dictionary<int, AudioChannel>();

        [Header("Audio Mixers")]
        public AudioMixerGroup masterMixer;
        public AudioMixerGroup musicMixer;
        public AudioMixerGroup sfxMixer;

        [Header("Audio Settings")]
        public AnimationCurve audioFalloffCurve;

        private Transform sfxRoot;

        public AudioSource[] allSFX => sfxRoot.GetComponentsInChildren<AudioSource>();

        #region Unity Methods
        private void Awake()
        {
            if (Instance == null)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
                return;
            }

            sfxRoot = new GameObject(SFX_PARENT_NAME).transform;
            sfxRoot.SetParent(transform);
        }
        #endregion

        #region Sound Effects
        public AudioSource PlaySoundEffect(string filePath, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false)
        {
            AudioClip clip = Resources.Load<AudioClip>(filePath);

            if (clip == null)
            {
                Debug.LogError($"Não pode carregar o arquivo '{filePath}'. Veja se existe na pasta Resources!");
                return null;
            }

            return PlaySoundEffect(clip, mixer, volume, pitch, loop, filePath);
        }

        public AudioSource PlaySoundEffect(AudioClip clip, AudioMixerGroup mixer = null, float volume = 1, float pitch = 1, bool loop = false, string filePath = ""/*, Vector3? position = null, float spatialBlend = 0*/)
        {
            string fileName = clip.name;
            if (filePath != string.Empty) fileName = filePath;

            AudioSource effectSource = new GameObject(string.Format(SFX_NAME_FORMAT, fileName)).AddComponent<AudioSource>();
            effectSource.transform.SetParent(sfxRoot);

            /*usa a posição informada 
            effectSource.transform.position = position ?? sfxRoot.position;
            */
            
            effectSource.clip = clip;

            if (mixer == null) mixer = sfxMixer;

            effectSource.outputAudioMixerGroup = mixer;
            effectSource.volume = volume;
            effectSource.spatialBlend = 0; //precisa ser 1
            effectSource.pitch = pitch;
            effectSource.loop = loop;

            effectSource.Play();

            if (!loop) Destroy(effectSource.gameObject, (clip.length / pitch) + 1);

            return effectSource;
        }

        public void StopSoundEffect(AudioClip clip) => StopSoundEffect(clip.name);

        public void StopSoundEffect(string soundName)
        {
            soundName = soundName.ToLower();

            AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
            foreach (var source in sources)
            {
                if (source.clip.name.ToLower() == soundName)
                {
                    Destroy(source.gameObject);
                    return;
                }
            }
        }

        public bool IsPlayingSoundEffect(string soundName)
        {
            soundName = soundName.ToLower();

            AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
            foreach (var source in sources)
            {
                if (source.clip.name.ToLower() == soundName) return true;
            }

            return false;
        }
        #endregion

        #region Music Tracks
        public AudioTrack PlayTrack(string filePath, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f)
        {
            AudioClip clip = Resources.Load<AudioClip>(filePath);

            if (clip == null)
            {
                Debug.LogError($"Não pode carregar o arquivo '{filePath}'. Veja se existe na pasta Resources!");
                return null;
            }

            return PlayTrack(clip, channel, loop, startingVolume, volumeCap, pitch, filePath);
        }

        public AudioTrack PlayTrack(AudioClip clip, int channel = 0, bool loop = true, float startingVolume = 0f, float volumeCap = 1f, float pitch = 1f, string filePath = "")
        {
            AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: true);
            AudioTrack track = audioChannel.PlayTrack(clip, loop, startingVolume, volumeCap, pitch, filePath);
            return track;
        }

        public void StopTrack(int channel)
        {
            AudioChannel audioChannel = TryGetChannel(channel, createIfDoesNotExist: false);

            if (audioChannel == null) return;

            audioChannel.StopTrack();
        }

        public void StopTrack(string trackName)
        {
            trackName = trackName.ToLower();

            foreach (var channel in channels.Values)
            {
                if (channel.activeTrack != null && channel.activeTrack.name.ToLower() == trackName)
                {
                    channel.StopTrack();
                    return;
                }
            }
        }

        public void StopAllTracks()
        {
            foreach (AudioChannel channel in channels.Values) channel.StopTrack();
        }
        #endregion

        #region Utility Methods
        public void StopAllSoundEffects()
        {
            AudioSource[] sources = sfxRoot.GetComponentsInChildren<AudioSource>();
            foreach (var source in sources) Destroy(source.gameObject);
        }

        public AudioChannel TryGetChannel(int channelNumber, bool createIfDoesNotExist = false)
        {
            AudioChannel channel = null;

            if (channels.TryGetValue(channelNumber, out channel))
            {
                return channel;
            }
            else if (createIfDoesNotExist)
            {
                channel = new AudioChannel(channelNumber);
                channels.Add(channelNumber, channel);
                return channel;
            }

            return null;
        }

        public void SetMasterVolume(float volume, bool muted)
        {
            volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
            musicMixer.audioMixer.SetFloat(MASTER_VOLUME_PARAMETER_NAME, volume);
        }

        public void SetMusicVolume(float volume, bool muted)
        {
            volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
            musicMixer.audioMixer.SetFloat(MUSIC_VOLUME_PARAMETER_NAME, volume);
        }

        public void SetSFXVolume(float volume, bool muted)
        {
            volume = muted ? MUTED_VOLUME_LEVEL : audioFalloffCurve.Evaluate(volume);
            sfxMixer.audioMixer.SetFloat(SFX_VOLUME_PARAMETER_NAME, volume);
        }

        #endregion
    }
}