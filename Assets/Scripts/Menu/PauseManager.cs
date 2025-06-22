using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using AUDIO;

public class PauseManager : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private CanvasGroup _optionCanvasGroup;
    [SerializeField] private CanvasGroup _backgroundCanvasGroup;

    [Header("Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _exitButton;

    [Header("Audio Settings")]
    [SerializeField] private AudioSettingData _audioSettings;

    private bool pausouNoEsc = false;

    private void Awake()
    {
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
    }

    private void Start()
    {

        _audioSettings.Initialize();

        _resumeButton.onClick.AddListener(Resume);
        _optionButton.onClick.AddListener(Options);
        _exitButton.onClick.AddListener(Exit);

        _audioSettings.Initialize();

        _audioSettings.masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _audioSettings.musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _audioSettings.sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        _audioSettings.muteMasterButton.onClick.AddListener(ToggleMuteMaster);
        _audioSettings.muteMusicButton.onClick.AddListener(ToggleMuteMusic);
        _audioSettings.muteSFXButton.onClick.AddListener(ToggleMuteSFX);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_mainCanvasGroup.alpha > 0f)
            {
                ResumeEsc();
            }

            else
            {
                PauseEsc();
            }

        }
    }

    private void SetCanvasGroupState(CanvasGroup targetGroup, bool isActive)
    {
        targetGroup.alpha = isActive ? 1f : 0f;
        targetGroup.interactable = isActive;
        targetGroup.blocksRaycasts = isActive;
    }

    private void PauseEsc()
    {
        pausouNoEsc = true;
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Pause", spatialBlend: 0);

        Cursor.lockState = CursorLockMode.None;
        SetCanvasGroupState(_mainCanvasGroup, true);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, true);
        Time.timeScale = 0f;
    }

    private void ResumeEsc()
    {
        ResumeInternal(playDespauseSound: true);
        
        /*
        pausouNoEsc = false;
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Despause", spatialBlend: 0);

        Cursor.lockState = CursorLockMode.Locked;
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
        Time.timeScale = 1f;
        */
    }

    public void Pause()
    {
        pausouNoEsc = false;

        Cursor.lockState = CursorLockMode.None;
        SetCanvasGroupState(_mainCanvasGroup, true);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        ResumeInternal(playDespauseSound: true);

        /*
        pausouNoEsc = false;
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Despause", spatialBlend: 0);

        Cursor.lockState = CursorLockMode.Locked;
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
        Time.timeScale = 1f;
        */
    }

    private void ResumeInternal(bool playDespauseSound){
        pausouNoEsc = false;
        if (playDespauseSound)
            AudioManager.Instance.PlaySoundEffect("Audio/UI/Despause", spatialBlend: 0);

        Cursor.lockState = CursorLockMode.Locked;
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
        if(!ColorGridManager.Instance._gameStarted) {
            Time.timeScale = 1f;
        }

    }

    public void Options()
    {
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, true);
        SetCanvasGroupState(_backgroundCanvasGroup, true);
        Time.timeScale = 0f;
    }

    public void Exit()
    {
        Application.Quit();
    }
    private void OnMasterVolumeChanged(float value) => _audioSettings.OnMasterVolumeChanged(value);
    private void OnMusicVolumeChanged(float value) => _audioSettings.OnMusicVolumeChanged(value);
    private void OnSFXVolumeChanged(float value) => _audioSettings.OnSFXVolumeChanged(value);
    private void ToggleMuteMaster() => _audioSettings.ToggleMuteMaster();
    private void ToggleMuteMusic() => _audioSettings.ToggleMuteMusic();
    private void ToggleMuteSFX() => _audioSettings.ToggleMuteSFX();
}
