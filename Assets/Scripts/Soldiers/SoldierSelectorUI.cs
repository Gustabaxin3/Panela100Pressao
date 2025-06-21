using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierSelectorUI : MonoBehaviour {
    private SoldierManager _soldierManager;

    [Header("Buttons")]
    [SerializeField] private Button _captainButton;
    [SerializeField] private Button _sublieutenantButton;
    [SerializeField] private Button _sargeantButton;
    [SerializeField] private Button _cadetButton;

    [Header("Roulette")]
    [SerializeField] private RectTransform _choiceRoulette;
    [SerializeField] private CanvasGroup _canvasGroupPanel;
    [SerializeField] private float _choiceRouletteScale = 1.5f;
    [SerializeField] private float _rouletteGrowDuration = 0.25f;

    [SerializeField] private Image _selectionImage;
    private List<Image> _buttonPortraits; 

    private Coroutine _growCoroutine;
    private bool _isRouletteActive = false;
    private float _lastMouseAngle;
    private float _currentRotation;

    private Vector2 _originalRouletteAnchoredPos;

    private List<Button> _buttons;
    private List<SoldierType> _types;

    private bool _isDragging = false;

    private readonly float[] _buttonAngles = { 0f, 90f, 180f, 270f }; 

    private void Start() {
        _soldierManager = GetComponent<SoldierManager>();
        _buttons = new List<Button> { _captainButton, _sublieutenantButton, _sargeantButton, _cadetButton };
        _types = new List<SoldierType> { SoldierType.Captain, SoldierType.Sublieutenant, SoldierType.Sargeant, SoldierType.Cadet };
        HandleAllChoicesInteractivity();

        _buttonPortraits = new List<Image> {
            _captainButton.GetComponent<Image>(),
            _sublieutenantButton.GetComponent<Image>(),
            _sargeantButton.GetComponent<Image>(),
            _cadetButton.GetComponent<Image>()
        };

        _originalRouletteAnchoredPos = _choiceRoulette.anchoredPosition;
    }

    private void OnEnable() {
        SoldierUnlockEvents.OnSoldierUnlocked += OnSoldierUnlocked;
    }

    private void OnDisable() {
        SoldierUnlockEvents.OnSoldierUnlocked -= OnSoldierUnlocked;
    }

    private void OnSoldierUnlocked(ISoldierState soldier) {
        HandleAllChoicesInteractivity();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            _canvasGroupPanel.alpha = 1f;
            MoveRouletteToScreenCenter();
            if (_growCoroutine != null)
                StopCoroutine(_growCoroutine);
            _growCoroutine = StartCoroutine(GrowRoulette());
            Cursor.lockState = CursorLockMode.None;
            _isRouletteActive = true;
            AlignRouletteToCurrentSoldier();
            InteractionHintUI.Instance.ShowHint("Rotacione a roleta com o mouse e selecione um soldado", Color.green);
        }

        if (_isRouletteActive && Input.GetMouseButtonDown(0)) {
            _isDragging = true;
            _lastMouseAngle = GetMouseAngle();
        }

        if (_isRouletteActive && Input.GetMouseButtonUp(0)) {
            _isDragging = false;
        }

        if (_isRouletteActive && _isDragging) {
            HandleRouletteRotation();
        }

        if (Input.GetKeyUp(KeyCode.Tab)) {
            _isRouletteActive = false;
            SelectCurrentSoldier();
            ResetRouletteUI();
            Cursor.lockState = CursorLockMode.Locked;
            InteractionHintUI.Instance.HideHint();
        }
        KeepSelectionImageFixed();
    }
    private void KeepSelectionImageFixed() {
        if (_selectionImage != null) {
            _selectionImage.rectTransform.rotation = Quaternion.identity;
        }
    }

    private void MoveRouletteToScreenCenter() {
        Canvas canvas = _choiceRoulette.GetComponentInParent<Canvas>();
        if (canvas != null) {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenCenter,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );
            _choiceRoulette.anchoredPosition = localPoint;
        }
    }

    private void RestoreRoulettePosition() {
        _choiceRoulette.anchoredPosition = _originalRouletteAnchoredPos;
    }

    private void HandleRouletteRotation() {
        float mouseAngle = GetMouseAngle();
        float deltaAngle = Mathf.DeltaAngle(_lastMouseAngle, mouseAngle);
        _currentRotation += deltaAngle;
        _choiceRoulette.localRotation = Quaternion.Euler(0, 0, _currentRotation);
        _lastMouseAngle = mouseAngle;

        UpdatePortraitsRotation();
    }

    private void UpdatePortraitsRotation() {
        foreach (var portrait in _buttonPortraits) {
            portrait.rectTransform.localRotation = Quaternion.Euler(0, 0, -_currentRotation);
        }
    }

    private float GetMouseAngle() {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, _choiceRoulette.position);
        Vector2 mouse = Input.mousePosition;
        Vector2 dir = mouse - center;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void SelectCurrentSoldier() {
        float minAngle = float.MaxValue;
        int selectedIdx = 0;
        for (int i = 0; i < _buttons.Count; i++) {
            Vector3 btnWorldPos = _buttons[i].transform.position;
            Vector2 btnDir = (btnWorldPos - _choiceRoulette.position).normalized;
            float angle = Vector2.Angle(Vector2.up, btnDir);
            if (angle < minAngle && _buttons[i].interactable) {
                minAngle = angle;
                selectedIdx = i;
            }
        }

        if (_selectionImage != null) {
            _selectionImage.gameObject.SetActive(true);
        }

        switch (_types[selectedIdx]) {
            case SoldierType.Captain: SelectCaptain(); break;
            case SoldierType.Sublieutenant: SelectSublieutenant(); break;
            case SoldierType.Sargeant: SelectSargeant(); break;
            case SoldierType.Cadet: SelectCadet(); break;
        }
    }

    private IEnumerator GrowRoulette() {
        Time.timeScale = 0f;
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 targetScale = Vector3.one * _choiceRouletteScale;
        _choiceRoulette.localScale = startScale;
        while (elapsed < _rouletteGrowDuration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / _rouletteGrowDuration);
            _choiceRoulette.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        _choiceRoulette.localScale = targetScale;
    }

    private void ResetRouletteUI() {
        Time.timeScale = 1f;
        _canvasGroupPanel.alpha = 0f;
        if (_growCoroutine != null)
            StopCoroutine(_growCoroutine);
        _choiceRoulette.localScale = Vector3.one;
        _choiceRoulette.localRotation = Quaternion.identity;
        _currentRotation = 0f;
        _isRouletteActive = false;
        RestoreRoulettePosition();
        UpdatePortraitsRotation();
    }

    private void HandleAllChoicesInteractivity() {
        _captainButton.interactable = _soldierManager.IsCaptainUnlocked;
        _sublieutenantButton.interactable = _soldierManager.IsSublieutenantUnlocked;
        _sargeantButton.interactable = _soldierManager.IsSargeantUnlocked;
        _cadetButton.interactable = _soldierManager.IsCadetUnlocked;
    }

    private void SelectSoldier(System.Action selectMethod, SoldierType type) {
        selectMethod();
        InteractionHintUI.Instance.HideHint();
        ResetRouletteUI();
    }

    public void SelectCaptain() => SelectSoldier(_soldierManager.SelectCaptain, SoldierType.Captain);
    public void SelectSublieutenant() => SelectSoldier(_soldierManager.SelectSublieutenant, SoldierType.Sublieutenant);
    public void SelectSargeant() => SelectSoldier(_soldierManager.SelectSargeant, SoldierType.Sargeant);
    public void SelectCadet() => SelectSoldier(_soldierManager.SelectCadet, SoldierType.Cadet);

    private void AlignRouletteToCurrentSoldier() {
        SoldierType current = _soldierManager.GetCurrentSoldierType();
        int idx = _types.IndexOf(current);
        if (idx < 0) idx = 0;
        _currentRotation = -_buttonAngles[idx];
        _choiceRoulette.localRotation = Quaternion.Euler(0, 0, _currentRotation);
        UpdatePortraitsRotation();
    }
}