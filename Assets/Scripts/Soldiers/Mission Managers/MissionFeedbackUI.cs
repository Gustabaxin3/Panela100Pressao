using UnityEngine;
using TMPro;
using System.Collections;

public class MissionFeedbackUI : MonoBehaviour {
    public static MissionFeedbackUI Instance { get; private set; }

    private TextMeshProUGUI _text;
    private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration = 1.5f;
    [SerializeField] private float _visibleDuration = 2f;

    private Coroutine _fadeCoroutine;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject); // evita múltiplos na cena
            return;
        }

        Instance = this;
    }
    private void Start() {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();

        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        if (_text == null) _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public static void ShowFeedback(string message) {
        Instance.Show(message);
    }

    private void Show(string message) {
        _text.text = message;

        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine() {
        // Fade-in
        float t = 0f;
        while (t < _fadeDuration) {
            t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 1f;
        yield return new WaitForSecondsRealtime(_visibleDuration);

        // Fade-out
        t = 0f;
        while (t < _fadeDuration) {
            t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / _fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = 0f;
    }
}
