using System.Collections;
using UnityEngine;
using TMPro;

public class InteractionHintUI : MonoBehaviour {
    public static InteractionHintUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _hintText.text = "";
        _canvasGroup.alpha = 0f;
    }

    public void ShowHint(string message) {
        _hintText.text = message;
        FadeTo(1f);
    }

    public void HideHint() {
        FadeTo(0f);
    }

    private void FadeTo(float targetAlpha) {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(_canvasGroup, _canvasGroup.alpha, targetAlpha, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
