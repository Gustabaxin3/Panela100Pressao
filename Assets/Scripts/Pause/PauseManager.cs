using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    private void Awake() {
        SetCanvasGroupState(_mainCanvasGroup, false);
        SetCanvasGroupState(_optionCanvasGroup, false);
        SetCanvasGroupState(_backgroundCanvasGroup, false);
    }
    private void Start() {
        _resumeButton.onClick.AddListener(Resume);
        _optionButton.onClick.AddListener(Options);
        _exitButton.onClick.AddListener(Exit);
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if (_mainCanvasGroup.alpha > 0f) {
                Resume();
                return;
            }
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
}