using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using AUDIO;
using TMPro;

public class PauseManager : MonoBehaviour {
    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private CanvasGroup _optionCanvasGroup;
    [SerializeField] private CanvasGroup _backgroundCanvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

    [Header("Sliders")]
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;

    [Header("Mute Buttons")]
    [SerializeField] private Button _muteMasterButton;
    [SerializeField] private Button _muteMusicButton;
    [SerializeField] private Button _muteSFXButton;

    private float _lastMasterVolume = 1f;
    private float _lastMusicVolume = 1f;
    private float _lastSFXVolume = 1f;

    private bool _isMasterMuted = false;
    private bool _isMusicMuted = false;
    private bool _isSFXMuted = false;

    private void Awake() {
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
    }

    private void Start() {
        _masterSlider.value = _lastMasterVolume;
        _musicSlider.value = _lastMusicVolume;
        _sfxSlider.value = _lastSFXVolume;

        _masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        UpdateMuteButtonTexts();
    }


    private void Update() {
        if (Keyboard.current.escapeKey.wasPressedThisFrame) {
            if (_mainCanvasGroup.alpha > 0f)
                Resume();
            else
                Pause();
        }
    }

    private void SetCanvasGroupState(CanvasGroup targetGroup, bool isActive) {
        targetGroup.alpha = isActive ? 1f : 0f;
        targetGroup.interactable = isActive;
        targetGroup.blocksRaycasts = isActive;
    }

    public void Pause() {
        Cursor.lockState = CursorLockMode.None;
        SetCanvasGroupState(_mainCanvasGroup, true);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, true);
        Time.timeScale = 0f;
    }

    public void Resume() {
        Cursor.lockState = CursorLockMode.Locked;
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
        Time.timeScale = 1f;
    }

    public void Options() {
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, true);
        SetCanvasGroupState(_backgroundCanvasGroup, true);
        Time.timeScale = 0f;
    }

    public void Exit() {
        Application.Quit();
    }

    public void OnMasterVolumeChanged(float value) {
        _lastMasterVolume = value;
        AudioManager.Instance.SetMasterVolume(value, _isMasterMuted);
    }

    public void OnMusicVolumeChanged(float value) {
        _lastMusicVolume = value;
        AudioManager.Instance.SetMusicVolume(value, _isMusicMuted);
    }

    public void OnSFXVolumeChanged(float value) {
        _lastSFXVolume = value;
        AudioManager.Instance.SetSFXVolume(value, _isSFXMuted);
    }

    public void ToggleMuteMaster() {
        _isMasterMuted = !_isMasterMuted;
        AudioManager.Instance.SetMasterVolume(_lastMasterVolume, _isMasterMuted);
        _masterSlider.value = _isMasterMuted ? 0f : _lastMasterVolume;
        UpdateMuteButtonTexts();
    }

    public void ToggleMuteMusic() {
        _isMusicMuted = !_isMusicMuted;
        AudioManager.Instance.SetMusicVolume(_lastMusicVolume, _isMusicMuted);
        _musicSlider.value = _isMusicMuted ? 0f : _lastMusicVolume;
        UpdateMuteButtonTexts();
    }

    public void ToggleMuteSFX() {
        _isSFXMuted = !_isSFXMuted;
        AudioManager.Instance.SetSFXVolume(_lastSFXVolume, _isSFXMuted);
        _sfxSlider.value = _isSFXMuted ? 0f : _lastSFXVolume;
        UpdateMuteButtonTexts();
    }

    private void UpdateMuteButtonTexts() {
        _muteMasterButton.GetComponentInChildren<TextMeshProUGUI>().text = _isMasterMuted ? "Desmutar Master" : "Mutar Master";

        _muteMusicButton.GetComponentInChildren<TextMeshProUGUI>().text = _isMusicMuted ? "Desmutar Música" : "Mutar Música";

        _muteSFXButton.GetComponentInChildren<TextMeshProUGUI>().text = _isSFXMuted ? "Desmutar SFX" : "Mutar SFX";
    }
}
