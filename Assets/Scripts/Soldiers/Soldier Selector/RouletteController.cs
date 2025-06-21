using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RouletteController {
    private RectTransform _roulette;
    private List<Image> _portraits;
    private float _currentRotation = 0f;

    public RouletteController(RectTransform roulette, List<Image> portraits) {
        _roulette = roulette;
        _portraits = portraits;
    }

    public void RotateBy(float deltaAngle) {
        _currentRotation += deltaAngle;
        _roulette.localRotation = Quaternion.Euler(0, 0, _currentRotation);
        UpdatePortraitsRotation();
    }

    private void UpdatePortraitsRotation() {
        foreach (var portrait in _portraits)
            portrait.rectTransform.localRotation = Quaternion.Euler(0, 0, -_currentRotation);
    }

    public void SetRotation(float angle) {
        _currentRotation = angle;
        _roulette.localRotation = Quaternion.Euler(0, 0, _currentRotation);
        UpdatePortraitsRotation();
    }

    public IEnumerator Grow(float targetScale, float duration) {
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.one * targetScale;
        float elapsed = 0f;

        _roulette.localScale = startScale;
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            _roulette.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        _roulette.localScale = endScale;
    }
    public float GetCurrentRotation() => _currentRotation;
    public void ResetScale() => _roulette.localScale = Vector3.one;
    public void SetPosition(Vector2 anchoredPosition) => _roulette.anchoredPosition = anchoredPosition;
    public Vector2 GetPosition() => _roulette.anchoredPosition;
    
}
