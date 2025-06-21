using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AUDIO;
using System;

[Serializable]
public class AudioSettingData {
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Mute Buttons")]
    public Button muteMasterButton;
    public Button muteMusicButton;
    public Button muteSFXButton;

    private float _lastMasterVolume = 1f;
    private float _lastMusicVolume = 1f;
    private float _lastSFXVolume = 1f;

    private bool _isMasterMuted = false;
    private bool _isMusicMuted = false;
    private bool _isSFXMuted = false;

    public void Initialize() {
        masterSlider.value = _lastMasterVolume;
        musicSlider.value = _lastMusicVolume;
        sfxSlider.value = _lastSFXVolume;

        masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        UpdateMuteButtonTexts();
    }

    public void ToggleMuteMaster() {
        _isMasterMuted = !_isMasterMuted;

        if (_isMasterMuted) {
            // Salva volume atual antes de mutar
            _lastMasterVolume = masterSlider.value;
            masterSlider.value = 0f;
        } else {
            // Restaura volume anterior
            masterSlider.value = _lastMasterVolume;
        }

        AudioManager.Instance.SetMasterVolume(_lastMasterVolume, _isMasterMuted);
        UpdateMuteButtonTexts();
    }

    public void ToggleMuteMusic() {
        _isMusicMuted = !_isMusicMuted;

        if (_isMusicMuted) {
            _lastMusicVolume = musicSlider.value;
            musicSlider.value = 0f;
        } else {
            musicSlider.value = _lastMusicVolume;
        }

        AudioManager.Instance.SetMusicVolume(_lastMusicVolume, _isMusicMuted);
        UpdateMuteButtonTexts();
    }

    public void ToggleMuteSFX() {
        _isSFXMuted = !_isSFXMuted;

        if (_isSFXMuted) {
            _lastSFXVolume = sfxSlider.value;
            sfxSlider.value = 0f;
        } else {
            sfxSlider.value = _lastSFXVolume;
        }

        AudioManager.Instance.SetSFXVolume(_lastSFXVolume, _isSFXMuted);
        UpdateMuteButtonTexts();
    }

    public void OnMasterVolumeChanged(float value) {
        if (!_isMasterMuted) {
            _lastMasterVolume = value;
            AudioManager.Instance.SetMasterVolume(value, false);
        }
    }

    public void OnMusicVolumeChanged(float value) {
        if (!_isMusicMuted) {
            _lastMusicVolume = value;
            AudioManager.Instance.SetMusicVolume(value, false);
        }
    }

    public void OnSFXVolumeChanged(float value) {
        if (!_isSFXMuted) {
            _lastSFXVolume = value;
            AudioManager.Instance.SetSFXVolume(value, false);
        }
    }

    private void UpdateMuteButtonTexts() {
        muteMasterButton.GetComponentInChildren<TextMeshProUGUI>().text = _isMasterMuted ? "Desmutar Master" : "Mutar Master";
        muteMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = _isMusicMuted ? "Desmutar Música" : "Mutar Música";
        muteSFXButton.GetComponentInChildren<TextMeshProUGUI>().text = _isSFXMuted ? "Desmutar SFX" : "Mutar SFX";
    }
}
