using AUDIO;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _backCreditsButton;
    [SerializeField] private Button _backOptionsButton;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup _optionsCanvasGroup;
    [SerializeField] private CanvasGroup _creditsCanvasGroup;

    [Header("Cameras")]
    [SerializeField] private CinemachineCamera _mainMenuCamera;
    [SerializeField] private CinemachineCamera _optionCamera;
    [SerializeField] private CinemachineCamera _creditsCamera;

    [Header("Camera Transition")]
    [SerializeField] private float _cameraTransitionWait = 2f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSettingData _audioSettings;

    private void Start() {
        AudioManager.Instance.PlayTrack("Audio/bgm/bgm_menu");

        Cursor.lockState = CursorLockMode.None;

        _optionsButton.onClick.AddListener(OnOptionsClicked);
        _creditsButton.onClick.AddListener(OnCreditsClicked);
        _playButton.onClick.AddListener(OnPlayClicked);
        _exitButton.onClick.AddListener(OnExitClicked);
        _backCreditsButton.onClick.AddListener(OnBackClicked);
        _backOptionsButton.onClick.AddListener(OnBackClicked);

        _audioSettings.Initialize();

        _audioSettings.masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        _audioSettings.musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _audioSettings.sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        _audioSettings.muteMasterButton.onClick.AddListener(ToggleMuteMaster);
        _audioSettings.muteMusicButton.onClick.AddListener(ToggleMuteMusic);
        _audioSettings.muteSFXButton.onClick.AddListener(ToggleMuteSFX);

        ShowMainMenuWithoutTransition();
    }
    private void OnMasterVolumeChanged(float value) => _audioSettings.OnMasterVolumeChanged(value);
    private void OnMusicVolumeChanged(float value) => _audioSettings.OnMusicVolumeChanged(value);
    private void OnSFXVolumeChanged(float value) => _audioSettings.OnSFXVolumeChanged(value);
    private void ToggleMuteMaster() => _audioSettings.ToggleMuteMaster();
    private void ToggleMuteMusic() => _audioSettings.ToggleMuteMusic();
    private void ToggleMuteSFX() => _audioSettings.ToggleMuteSFX();

    private void OnOptionsClicked() {
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao", spatialBlend: 0);
        StartCoroutine(ShowMenuWithCameraTransition(_optionCamera, _optionsCanvasGroup));
    }

    private void OnCreditsClicked() {
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao", spatialBlend: 0);
        StartCoroutine(ShowMenuWithCameraTransition(_creditsCamera, _creditsCanvasGroup));
    }
    private void OnBackClicked() {
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao", spatialBlend: 0);
        StartCoroutine(ShowMenuWithCameraTransition(_mainMenuCamera, _mainMenuCanvasGroup));
    }

    private void OnPlayClicked() {
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao", spatialBlend: 0);
        AudioManager.Instance.StopAllTracks();
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    private void OnExitClicked() {
        AudioManager.Instance.PlaySoundEffect("Audio/UI/Botao", spatialBlend: 0);
        Application.Quit();
    }

    private void ShowMainMenu() {
        StartCoroutine(ShowMenuWithCameraTransition(_mainMenuCamera, _mainMenuCanvasGroup));
    }
    private void ShowMainMenuWithoutTransition() {
        SetCanvasGroup(_mainMenuCanvasGroup, true);
        SetCanvasGroup(_optionsCanvasGroup, false);
        SetCanvasGroup(_creditsCanvasGroup, false);
        SetCameraPriority(_mainMenuCamera, 20);
        SetCameraPriority(_optionCamera, 10);
        SetCameraPriority(_creditsCamera, 10);
    }

    private IEnumerator ShowMenuWithCameraTransition(CinemachineCamera targetCamera, CanvasGroup targetCanvas) {
        SetCanvasGroup(_mainMenuCanvasGroup, false);
        SetCanvasGroup(_optionsCanvasGroup, false);
        SetCanvasGroup(_creditsCanvasGroup, false);

        SetCameraPriority(_mainMenuCamera, targetCamera == _mainMenuCamera ? 20 : 10);
        SetCameraPriority(_optionCamera, targetCamera == _optionCamera ? 20 : 10);
        SetCameraPriority(_creditsCamera, targetCamera == _creditsCamera ? 20 : 10);

        yield return new WaitForSeconds(_cameraTransitionWait);

        SetCanvasGroup(targetCanvas, true);
    }

    private void SetCanvasGroup(CanvasGroup group, bool visible) {
        group.alpha = visible ? 1 : 0;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }

    private void SetCameraPriority(CinemachineCamera cam, int priority) {
        if (cam != null)
            cam.Priority = priority;
    }
}
