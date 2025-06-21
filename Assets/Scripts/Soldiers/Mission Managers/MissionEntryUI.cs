using System.Collections;
using UnityEngine;
using TMPro;

public class MissionEntryUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private CanvasGroup _canvasGroup;
    private Coroutine _fadeCoroutine;

    public void Initialize(Mission mission) {
        UpdateEntry(mission);
    }

    public void UpdateEntry(Mission mission) {
        _titleText.text = mission.GetTitle();
        bool completed = mission.IsCompleted;

        _titleText.color = completed ? Color.gray : Color.white;
        _titleText.fontStyle = completed ? FontStyles.Strikethrough : FontStyles.Normal;

        if (completed && _fadeCoroutine == null)
            _fadeCoroutine = StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy() {
        yield return new WaitForSeconds(2f);
        float duration = 0.5f;
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}