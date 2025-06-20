using System.Collections;
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
    private CanvasGroup _captainSelectionCanvasGroup;
    private CanvasGroup _sublieutenantSelectionCanvasGroup;
    private CanvasGroup _sargeantSelectionCanvasGroup;
    private CanvasGroup _cadetSelectionCanvasGroup;

    [SerializeField] private CanvasGroup _canvasGroupPanel;

    [Header("Choice Roulette")]
    [SerializeField] private GameObject _choiceRoulette;

    [Header("Choice Roulette Scale")]
    [SerializeField] private float _choiceRouletteScale = 2f;
    [SerializeField] private float _rouletteGrowDuration = 0.25f;

    private Coroutine _growCoroutine;
    private bool _isRouletteActive = false;

    private void Start() {
        _soldierManager = GetComponent<SoldierManager>();

        _captainSelectionCanvasGroup = _captainButton.GetComponentInChildren<CanvasGroup>();
        _sublieutenantSelectionCanvasGroup = _sublieutenantButton.GetComponentInChildren<CanvasGroup>();
        _sargeantSelectionCanvasGroup = _sargeantButton.GetComponentInChildren<CanvasGroup>();
        _cadetSelectionCanvasGroup = _cadetButton.GetComponentInChildren<CanvasGroup>();

        HandleAllChoicesCanvasGroup(hide: true);

        ShowSelectedCanvasGroup(_soldierManager.GetCurrentSoldierType());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            _canvasGroupPanel.alpha = 1f;
            Cursor.lockState = CursorLockMode.None;

            if (_growCoroutine != null)
                StopCoroutine(_growCoroutine);
            _growCoroutine = StartCoroutine(GrowRoulette());

            _isRouletteActive = true;
        }
        if (Input.GetKeyUp(KeyCode.Tab)) {
            ResetRouletteUI();
        }
    }

    private IEnumerator GrowRoulette() {
        Time.timeScale = 0.35f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * _choiceRouletteScale;
        _choiceRoulette.transform.localScale = startScale;
        while (elapsed < _rouletteGrowDuration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _rouletteGrowDuration);
            _choiceRoulette.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        _choiceRoulette.transform.localScale = targetScale;
    }

    private void ResetRouletteUI() {
        Time.timeScale = 1f;
        _canvasGroupPanel.alpha = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        if (_growCoroutine != null)
            StopCoroutine(_growCoroutine);
        _choiceRoulette.transform.localScale = Vector3.one;
        _isRouletteActive = false;
    }

    private void HandleChoiceProperties(CanvasGroup canvasGroup, Button button, bool isSelecting) {
        canvasGroup.alpha = isSelecting ? 1f : 0f;
        canvasGroup.interactable = isSelecting;
        canvasGroup.blocksRaycasts = isSelecting;
    }

    private void HandleAllChoicesCanvasGroup(bool hide) {
        HandleChoiceProperties(_cadetSelectionCanvasGroup, _cadetButton, !hide);
        HandleChoiceProperties(_sargeantSelectionCanvasGroup, _sargeantButton, !hide);
        HandleChoiceProperties(_sublieutenantSelectionCanvasGroup, _sublieutenantButton, !hide);
        HandleChoiceProperties(_captainSelectionCanvasGroup, _captainButton, !hide);
    }

    public void HandleHover(SoldierType type) {
        ShowSelectedCanvasGroup(type);
    }

    private void ShowSelectedCanvasGroup(SoldierType selectedType) {
        CanvasGroup[] canvasGroups = {
            _captainSelectionCanvasGroup,
            _sublieutenantSelectionCanvasGroup,
            _sargeantSelectionCanvasGroup,
            _cadetSelectionCanvasGroup
        };

        Button[] buttons = {
            _captainButton,
            _sublieutenantButton,
            _sargeantButton,
            _cadetButton
        };

        for (int i = 0; i < canvasGroups.Length; i++) {
            bool isSelected = ((int)selectedType == i);
            HandleChoiceProperties(canvasGroups[i], buttons[i], isSelected);
        }
    }


    private void SelectSoldier(System.Action selectMethod, SoldierType type) {
        selectMethod();
        ShowSelectedCanvasGroup(type);
        ResetRouletteUI();
    }

    public void SelectCaptain() => SelectSoldier(_soldierManager.SelectCaptain, SoldierType.Captain);

    public void SelectSublieutenant() => SelectSoldier(_soldierManager.SelectSublieutenant, SoldierType.Sublieutenant);

    public void SelectSargeant() => SelectSoldier(_soldierManager.SelectSargeant, SoldierType.Sargeant);

    public void SelectCadet() => SelectSoldier(_soldierManager.SelectCadet, SoldierType.Cadet);

}
