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
    [SerializeField] private int _rows = 3;
    [SerializeField] private int _cols = 3;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("UI")]
    [SerializeField] private TMP_Text _feedbackText;

    private ColorCell[] _cells;

    public bool _gameStarted = false;
    public bool _gameCompleted = false;

    [SerializeField] private float _fadeDuration = 0.5f;

    [SerializeField] private ColorMiniGameTrigger[] allMiniGames;
    public ColorMiniGameTrigger currentTrigger;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        if (_canvasGroup != null) {
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        allMiniGames = FindObjectsOfType<ColorMiniGameTrigger>();
    }

    public void StartGame(ColorMiniGameTrigger currentTrigger) {
        this.currentTrigger = currentTrigger;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        StartCoroutine(HandleStartingGame());

    }
    private IEnumerator HandleStartingGame() {
        StartCoroutine(FadeCanvas(true, _fadeDuration));

        yield return new WaitForSecondsRealtime(0.1f);

        ConfigureGridLayout();
        GenerateGrid();
        RandomizeColors();

        _feedbackText.text = "Deixe os quadrados com a mesma cor!";
    }

    private void ConfigureGridLayout() {
        GridLayoutGroup layout = _gridParent.GetComponent<GridLayoutGroup>();
        if (layout != null) {
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = _cols;
            layout.cellSize = new Vector2(100, 100);
            layout.spacing = new Vector2(5, 5);
            layout.padding = new RectOffset(5, 5, 5, 5);
        }
    }

    private void GenerateGrid() {
        foreach (Transform child in _gridParent) {
            Destroy(child.gameObject);
        }

        _cells = new ColorCell[_rows * _cols];

        for (int i = 0; i < _rows * _cols; i++) {
            GameObject go = Instantiate(_cellPrefab, _gridParent);
            ColorCell cell = go.GetComponent<ColorCell>();
            _cells[i] = cell;

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(cell.OnClick);
        }
    }

    private void RandomizeColors() {
        int totalCells = _cells.Length;
        int maxPerColor = totalCells / 2;
        int colorCount = 4;

        Dictionary<int, int> colorUsage = new Dictionary<int, int>();
        for (int i = 0; i < colorCount; i++) colorUsage[i] = 0;

        System.Random rand = new System.Random();

        foreach (var cell in _cells) {
            List<int> validIndices = new List<int>();
            for (int i = 0; i < colorCount; i++) {
                if (colorUsage[i] < maxPerColor) {
                    validIndices.Add(i);
                }
            }

            int selected = validIndices[rand.Next(validIndices.Count)];
            colorUsage[selected]++;
            cell.SetColorIndex(selected);
        }
    }

    public void CheckVictory() {
        if (_cells.Length == 0) return;

        int target = _cells[0].GetColorIndex();
        foreach (var cell in _cells) {
            if (cell.GetColorIndex() != target) {
                return;
            }
        }

        _feedbackText.text = "Você venceu!";
        _feedbackText.color = Color.green;

        StartCoroutine(HandleVictory());
    }

    private IEnumerator HandleVictory() {
        float blinkDuration = 1.5f;
        float elapsed = 0f;
        float blinkInterval = 0.2f;

        while (elapsed < blinkDuration) {
            _feedbackText.enabled = !_feedbackText.enabled;
            yield return new WaitForSecondsRealtime(blinkInterval);
            elapsed += blinkInterval;
        }

        _feedbackText.enabled = true;

        yield return StartCoroutine(FadeCanvas(false, 0.5f));

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        CompleteGame();
    }

    private IEnumerator FadeCanvas(bool fadeIn, float duration) {
        float startAlpha = fadeIn ? 0f : 1f;
        float endAlpha = fadeIn ? 1f : 0f;
        float t = 0f;

        if (_canvasGroup != null) {
            _canvasGroup.interactable = fadeIn;
            _canvasGroup.blocksRaycasts = fadeIn;

            while (t < duration) {
                t += Time.unscaledDeltaTime;
                float normalized = Mathf.Clamp01(t / duration);
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, normalized);
                yield return null;
            }

            _canvasGroup.alpha = endAlpha;
        }
    }

    public void ResetGrid() {
        foreach (var cell in _cells) {
            cell.ResetColor();
        }
        _feedbackText.text = "";
    }

    public void CompleteGame() {
        
        AudioManager.Instance.PlaySoundEffect("Audio/UI/MiniGame-Ganhou", spatialBlend: 0);
        
        _gameCompleted = true;
        OnGameCompleted?.Invoke();

        if (currentTrigger != null)
            currentTrigger.CompleteMinigame();

        int remaining = 0;
        foreach (var trigger in allMiniGames) {
            if (!trigger.IsCompleted) remaining++;
        }

        if (remaining == 1) {
            return;
        } else if (remaining > 1) {
            MissionFeedbackUI.ShowFeedback($"Faltam {remaining} máquinas para completar a missão.");
        }
    }

    public void CheckAllMinigamesCompleted() {
        bool allCompleted = true;
        foreach (var trigger in allMiniGames) {
            if (!trigger.IsCompleted) {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted) {
            MissionManager.Instance.CompleteMission(MissionID.HackearTodasAsMaquinas);
            MissionFeedbackUI.ShowFeedback("Missão 'Hackear Todas as Máquinas' foi completada!");
        }
    }
}
