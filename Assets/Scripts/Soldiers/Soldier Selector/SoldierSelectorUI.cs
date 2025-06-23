using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AUDIO;

public class SoldierSelectorUI : MonoBehaviour {
    private SoldierManager _soldierManager;

    [SerializeField] private SoldierSelectorData _data;

    private RouletteController _rouletteController;

    private Coroutine _growCoroutine;
    private bool _isRouletteActive = false;
    private bool _isDragging = false;

    private float _lastMouseAngle;

    private Vector2 _originalRouletteAnchoredPos;

    private SoldierType _lastSoldierType;

    [SerializeField] private CanvasGroup _selectorCanvasGroup;

    private void Start() {
        _soldierManager = GetComponent<SoldierManager>();

        _data.Initialize();

        _rouletteController = new RouletteController(_data.choiceRoulette, _data.buttonPortraits);
        _originalRouletteAnchoredPos = _rouletteController.GetPosition();

        HandleAllChoicesInteractivity();
        AlignRouletteToCurrentSoldier();
        _lastSoldierType = _soldierManager.GetCurrentSoldierType();
    }

    private void OnEnable() => SoldierUnlockEvents.OnSoldierUnlocked += OnSoldierUnlocked;
    private void OnDisable() => SoldierUnlockEvents.OnSoldierUnlocked -= OnSoldierUnlocked;


    private void OnSoldierUnlocked(ISoldierState soldier) {
        HandleAllChoicesInteractivity();
    }

    private void Update() {
        HandleInput();
        CheckSoldierTypeChanged();
    }

    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            string[] soundsMenuTroca =
            {
                "Audio/UI/MenuTrocaPersonagem01",
                "Audio/UI/MenuTrocaPersonagem02",
                "Audio/UI/MenuTrocaPersonagem03"
            };

            int numSorteado = UnityEngine.Random.Range(0, soundsMenuTroca.Length);
            AudioManager.Instance.PlaySoundEffect(soundsMenuTroca[numSorteado], position: transform.position, spatialBlend: 0);

            OpenSelector();
        }
        
        else if (_isRouletteActive && Input.GetMouseButtonDown(0))
            StartDragging();
        else if (_isRouletteActive && Input.GetMouseButtonUp(0))
            StopDragging();

        if (_isRouletteActive && _isDragging) RotateWithMouse();

        if (Input.GetKeyUp(KeyCode.Tab)) CloseSelector();
    }

    private void OpenSelector() {
        Time.timeScale = 0f;
        _data.canvasGroupPanel.alpha = 1f;
        _data._keyboardHintText.GetComponent<CanvasGroup>().alpha = 0f;
        MoveRouletteToScreenCenter();
        if (_growCoroutine != null) StopCoroutine(_growCoroutine);
        _growCoroutine = StartCoroutine(_rouletteController.Grow(_data.choiceRouletteScale, _data.rouletteGrowDuration));

        Cursor.lockState = CursorLockMode.None;
        _isRouletteActive = true;

        InteractionHintUI.Instance.ShowHint("Rotacione a roleta com o mouse e selecione um soldado", Color.green);

        AlignRouletteToCurrentSoldier();
    }

    private void StartDragging() {
        _isDragging = true;
        _lastMouseAngle = GetMouseAngle();
    }

    private void StopDragging() {
        _isDragging = false;
    }

    private void RotateWithMouse() {
        float mouseAngle = GetMouseAngle();
        float deltaAngle = Mathf.DeltaAngle(_lastMouseAngle, mouseAngle);
        _rouletteController.RotateBy(deltaAngle);
        _lastMouseAngle = mouseAngle;
    }

    private void CloseSelector() {
        _isRouletteActive = false;
        _data._keyboardHintText.GetComponent<CanvasGroup>().alpha = 1f;
        SelectCurrentSoldier();
        ResetRouletteUI();
        AlignRouletteToCurrentSoldier();
        Cursor.lockState = CursorLockMode.Locked;
        InteractionHintUI.Instance.HideHint();
    }

    private void ResetRouletteUI() {
        Time.timeScale = 1f;
        _selectorCanvasGroup.alpha = 0f;
        _data.canvasGroupPanel.alpha = 0f;
        if (_growCoroutine != null) StopCoroutine(_growCoroutine);
        _rouletteController.ResetScale();
        _isRouletteActive = false;
        _rouletteController.SetPosition(_originalRouletteAnchoredPos);
    }

    private void MoveRouletteToScreenCenter() {
        var canvas = _data.choiceRoulette.GetComponentInParent<Canvas>();
        if (canvas != null) {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenCenter,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out localPoint
            );
            _rouletteController.SetPosition(localPoint);
        }
        _selectorCanvasGroup.alpha = 1f;
    }

    private float GetMouseAngle() {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, _data.choiceRoulette.position);
        Vector2 mouse = Input.mousePosition;
        Vector2 dir = mouse - center;
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private void HandleAllChoicesInteractivity() {
        _data.captainButton.interactable = _soldierManager.IsCaptainUnlocked;
        _data.sublieutenantButton.interactable = _soldierManager.IsSublieutenantUnlocked;
        _data.sargeantButton.interactable = _soldierManager.IsSargeantUnlocked;
        _data.cadetButton.interactable = _soldierManager.IsCadetUnlocked;
    }


    private void SelectCurrentSoldier() {
        float minAngle = float.MaxValue;
        int selectedIdx = 0;
        for (int i = 0; i < _data.buttons.Count; i++) {
            Vector3 btnWorldPos = _data.buttons[i].transform.position;
            Vector2 btnDir = (btnWorldPos - _data.choiceRoulette.position).normalized;
            float angle = Vector2.Angle(Vector2.up, btnDir);
            if (angle < minAngle && _data.buttons[i].interactable) {
                minAngle = angle;
                selectedIdx = i;
            }
        }

        switch (_data.types[selectedIdx]) {
            case SoldierType.Captain: SelectCaptain(); break;
            case SoldierType.Sublieutenant: SelectSublieutenant(); break;
            case SoldierType.Sargeant: SelectSargeant(); break;
            case SoldierType.Cadet: SelectCadet(); break;
        }
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

        if (_data.soldierAngles.TryGetValue(current, out float angle)) {
            _rouletteController.SetRotation(angle);
            return;
        }
        Debug.LogWarning($"[SoldierSelectorUI] Sem ângulo definido para o tipo {current}");
        _rouletteController.SetRotation(0f);
    }

    private void CheckSoldierTypeChanged() {
        var currentType = _soldierManager.GetCurrentSoldierType();
        if (currentType != _lastSoldierType) {
            AlignRouletteToCurrentSoldier();
            _lastSoldierType = currentType;
        }
    }
}
