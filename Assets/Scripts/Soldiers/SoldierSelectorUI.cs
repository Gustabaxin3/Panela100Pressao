using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoldierSelectorUI : MonoBehaviour {
    private SoldierManager _soldierManager;

    [Header("Buttons")]
    [SerializeField] private Button _captainButton;
    [SerializeField] private Button _sublieutenantButton;
    [SerializeField] private Button _sargeantButton;
    [SerializeField] private Button _cadetButton;

    [Header("Canvas Groups")]
    private CanvasGroup _captainCanvasGroup;
    private CanvasGroup _sublieutenantCanvasGroup;
    private CanvasGroup _sargeantCanvasGroup;
    private CanvasGroup _cadetCanvasGroup;

    [SerializeField] private CanvasGroup _canvasGroupPanel;

    [Header("Choice Roulette")]
    [SerializeField] private GameObject _choiceRoulette;

    [Header("Choice Roulette Scale")]
    [SerializeField] private float _choiceRouletteScale = 2f;

    private void Start() {
        _soldierManager = GetComponent<SoldierManager>();

        _captainCanvasGroup = _captainButton.GetComponentInChildren<CanvasGroup>();
        _sublieutenantCanvasGroup = _sublieutenantButton.GetComponentInChildren<CanvasGroup>();
        _sargeantCanvasGroup = _sargeantButton.GetComponentInChildren<CanvasGroup>();
        _cadetCanvasGroup = _cadetButton.GetComponentInChildren<CanvasGroup>();

        HandleAllChoicesCanvasGroup(hide: true);
    }

    private bool _isRouletteActive = false;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Time.timeScale = 0.2f;

            _canvasGroupPanel.alpha = 1f;

            Cursor.lockState = CursorLockMode.None;

            _choiceRoulette.transform.localScale = Vector3.one * _choiceRouletteScale;

            _isRouletteActive = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            ResetRouletteUI();
        }
    }
    private void ResetRouletteUI() {
        Time.timeScale = 1f;
        _canvasGroupPanel.alpha = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        _choiceRoulette.transform.localScale = Vector3.one;
        _isRouletteActive = false;
        HandleAllChoicesCanvasGroup(hide: true);
    }
    private void HandleChoiceProperties(CanvasGroup canvasGroup, Button button, bool isSelecting) {
        canvasGroup.alpha = isSelecting ? 1f : 0f;
        canvasGroup.interactable = isSelecting;
        canvasGroup.blocksRaycasts = isSelecting;
    }
    private void HandleAllChoicesCanvasGroup(bool hide) {
        HandleChoiceProperties(_cadetCanvasGroup, _cadetButton, !hide);
        HandleChoiceProperties(_sargeantCanvasGroup, _sargeantButton, !hide);
        HandleChoiceProperties(_sublieutenantCanvasGroup, _sublieutenantButton, !hide);
        HandleChoiceProperties(_captainCanvasGroup, _captainButton, !hide);
    }
    public void HandleHover(SoldierType type) {
        CanvasGroup[] canvasGroups = { _captainCanvasGroup, _sublieutenantCanvasGroup, _sargeantCanvasGroup, _cadetCanvasGroup };
        Button[] buttons = { _captainButton, _sublieutenantButton, _sargeantButton, _cadetButton };

        int selectedIndex = (int)type;
        for (int i = 0; i < canvasGroups.Length; i++) {
            bool isSelected = i == selectedIndex;
            HandleChoiceProperties(canvasGroups[i], buttons[i], isSelected);
        }
    }
    public void SelectCaptain() {
        ResetRouletteUI();
        _soldierManager.SelectCaptain();
    }
    public void SelectSublieutenant() {
        ResetRouletteUI();
        _soldierManager.SelectSublieutenant();
    }
    public void SelectSargeant() {
        ResetRouletteUI();
        _soldierManager.SelectSargeant();
    }
    public void SelectCadet() {
        ResetRouletteUI();
        _soldierManager.SelectCadet();
    }
}
