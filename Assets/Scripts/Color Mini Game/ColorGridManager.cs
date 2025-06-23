using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using AUDIO;

public class ColorGridManager : MonoBehaviour {
    public static ColorGridManager Instance { get; private set; }
    public event System.Action OnGameCompleted;

    [Header("Grid Setup")]
    [SerializeField] private Transform _gridParent;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("UI")]
    [SerializeField] private TMP_Text _feedbackText;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Header("Minigame Triggers")]
    [SerializeField] private ColorMiniGameTrigger[] allMiniGames;
    public ColorMiniGameTrigger currentTrigger { get; private set; }

    [Header("Desbloqueio")]
    [SerializeField] private ISoldierState soldierToUnlock;

    private readonly Vector2Int[] _levels = new Vector2Int[] {
        new Vector2Int(1, 4),  // 1ª máquina
        new Vector2Int(2, 4),  // 2ª máquina
        new Vector2Int(3, 3),  // 3ª máquina
        new Vector2Int(4, 4),  // 4ª máquina
        new Vector2Int(5, 5),  // 5ª máquina
        new Vector2Int(6, 5),  // 6ª máquina
    };
    private int _currentLevelIndex = 0;

    // estado interno
    private ColorCell[] _cells;
    public bool _gameStarted = false;
    private bool _gameCompleted = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        if (_canvasGroup != null) {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        allMiniGames = FindObjectsOfType<ColorMiniGameTrigger>();
        soldierSelectorUI = FindObjectOfType<SoldierSelectorUI>();

    }

    public void StartGame(ColorMiniGameTrigger trigger) {
        if (_currentLevelIndex >= _levels.Length) {
            Debug.LogWarning("Todas as máquinas já foram jogadas!");
            return;
        }

        currentTrigger = trigger;
        _gameStarted = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        var size = _levels[_currentLevelIndex];
        int rows = size.x;
        int cols = size.y;

        StartCoroutine(HandleStartingGame(rows, cols));
    }

    private IEnumerator HandleStartingGame(int rows, int cols) {
        StartCoroutine(FadeCanvas(true, _fadeDuration));
        yield return new WaitForSecondsRealtime(0.1f);

        ConfigureGridLayout(rows, cols);
        GenerateGrid(rows, cols);
        RandomizeColors();

        _feedbackText.text = "Deixe os quadrados com a mesma cor!";
        _feedbackText.color = Color.white;
    }

    private void ConfigureGridLayout(int rows, int cols) {
        var layout = _gridParent.GetComponent<GridLayoutGroup>();
        if (layout == null) return;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = cols;
    }

    private void GenerateGrid(int rows, int cols) {
        foreach (Transform c in _gridParent) {
            Destroy(c.gameObject);
        }

        _cells = new ColorCell[rows * cols];
        for (int i = 0; i < rows * cols; i++) {
            var go = Instantiate(_cellPrefab, _gridParent);
            var cell = go.GetComponent<ColorCell>();
            _cells[i] = cell;
            var btn = go.GetComponent<Button>();
            btn.onClick.AddListener(cell.OnClick);
        }
    }

    private void RandomizeColors() {
        int total = _cells.Length;
        int maxPerColor = total / 2;
        int colorCount = 4;

        var usage = new Dictionary<int, int>();
        for (int i = 0; i < colorCount; i++) usage[i] = 0;

        var rand = new System.Random();
        foreach (var cell in _cells) {
            var valid = new List<int>();
            for (int i = 0; i < colorCount; i++)
                if (usage[i] < maxPerColor) valid.Add(i);

            int pick = valid[rand.Next(valid.Count)];
            usage[pick]++;
            cell.SetColorIndex(pick);
        }
    }

    public void CheckVictory() {
        if (_cells == null || _cells.Length == 0) return;

        int target = _cells[0].GetColorIndex();
        foreach (var cell in _cells)
            if (cell.GetColorIndex() != target)
                return;

        foreach (var cell in _cells) {
            var btn = cell.GetComponent<Button>();
            if (btn != null) btn.interactable = false;
        }

        AudioManager.Instance.PlaySoundEffect("Audio/UI/MiniGame-Ganhou", spatialBlend: 0);

        Cursor.lockState = CursorLockMode.Locked;

        _feedbackText.text = "Você venceu!";
        _feedbackText.color = Color.green;

        StartCoroutine(HandleVictory());
    }

    private IEnumerator HandleVictory() {
        float blinkDuration = 1.5f;
        float interval = 0.2f;
        float elapsed = 0f;

        while (elapsed < blinkDuration) {
            _feedbackText.enabled = !_feedbackText.enabled;
            yield return new WaitForSecondsRealtime(interval);
            elapsed += interval;
        }

        _feedbackText.enabled = true;

        yield return StartCoroutine(FadeCanvas(false, _fadeDuration));
        Time.timeScale = 1f;

        CompleteGame();
    }

    private IEnumerator FadeCanvas(bool fadeIn, float duration) {
        if (_canvasGroup == null) yield break;

        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;
        _canvasGroup.interactable = fadeIn;
        _canvasGroup.blocksRaycasts = fadeIn;

        float t = 0f;
        while (t < duration) {
            t += Time.unscaledDeltaTime;
            float f = Mathf.Clamp01(t / duration);
            _canvasGroup.alpha = Mathf.Lerp(start, end, f);
            yield return null;
        }

        _canvasGroup.alpha = end;
    }

    private void CompleteGame() {
        _gameCompleted = true;
        OnGameCompleted?.Invoke();

        if (currentTrigger != null) {
            currentTrigger.CompleteMinigame();
            currentTrigger.DisableInteraction();
        }

        _currentLevelIndex++;

        int remaining = 0;
        foreach (var trig in allMiniGames)
            if (!trig.IsCompleted) remaining++;

        if (remaining > 1) {
            MissionFeedbackUI.ShowFeedback($"Faltam {remaining} máquinas para completar a missão.");
        }

    }


    public void CheckAllMinigamesCompleted() {
        bool all = true;
        foreach (var trig in allMiniGames)
            if (!trig.IsCompleted) { all = false; break; }

        if (all) {
            MissionManager.Instance.CompleteMission(MissionID.HackearTodasAsMaquinas);
            MissionFeedbackUI.ShowFeedback("Missão 'Hackear Todas as Máquinas' foi completada!");

            SoldierUnlockEvents.Unlock(soldierToUnlock);
            soldierSelectorUI.HandleAllChoicesInteractivity();

        }
    }
}
