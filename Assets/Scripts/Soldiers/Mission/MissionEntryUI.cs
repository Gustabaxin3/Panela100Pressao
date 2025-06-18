using TMPro;
using UnityEngine;

public class MissionEntryUI : MonoBehaviour {
    [field: SerializeField] public TextMeshProUGUI Label { get; private set; }

    [SerializeField] public CanvasGroup _canvasGroup { get; private set; }

    private string _missionText;
    public bool IsComplete { get; private set; } = false;

    private void Awake() {
        Label = GetComponentInChildren<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Setup(string missionText) {
        this._missionText = missionText;
        UpdateText();
    }

    public void MarkComplete() {
        IsComplete = true;
        UpdateText();
    }

    private void UpdateText() {
        Label.text = $"{_missionText}";
    }
}
