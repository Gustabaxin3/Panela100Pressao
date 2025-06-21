using System.Collections;
using UnityEngine;
using TMPro;

public class RescuePoint : MonoBehaviour {
    [SerializeField] private ISoldierState _targetSoldier;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float visibleDuration = 2f;

    private Coroutine _fadeCoroutine;
    private bool _alreadyUnlocked = false;
    private void Awake() {
        canvasGroup.alpha = 0f; 
    }

    private void OnTriggerEnter(Collider other) {
        if (_alreadyUnlocked) return;

        if (other.TryGetComponent<ISoldierState>(out ISoldierState soldier)) {
            _alreadyUnlocked = true;

            SoldierUnlockEvents.Unlock(_targetSoldier);
            _text.text = $"Soldado {_targetSoldier.soldierType} desbloqueado!";
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = StartCoroutine(FadeMessage());
        }
    }

    private IEnumerator FadeMessage() {
        // Fade-in
        float timer = 0f;
        while (timer < fadeDuration) {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(visibleDuration);

        // Fade-out
        timer = 0f;
        while (timer < fadeDuration) {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }
}
