using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

public class MissionManager : MonoBehaviour {
    [Header("Referências de Missão")]
    [SerializeField] private MissionEntryUI _sublieutenantEntry;
    [SerializeField] private MissionEntryUI _sargeantEntry;
    [SerializeField] private MissionEntryUI _cadetEntry;

    [SerializedDictionary("Tipo", "Texto")]
    public SerializedDictionary<string, TextMeshProUGUI> _missionTexts;

    private Dictionary<string, MissionEntryUI> _missionEntries;

    private void Awake() {
        _missionEntries = new Dictionary<string, MissionEntryUI> {
            { "Sublieutenant", _sublieutenantEntry },
            { "Sargeant", _sargeantEntry },
            { "Cadet", _cadetEntry }
        };

        _missionTexts = new SerializedDictionary<string, TextMeshProUGUI> {
            { "Sublieutenant", _sublieutenantEntry.Label },
            { "Sargeant", _sargeantEntry.Label },
            { "Cadet", _cadetEntry.Label }
        };

        SoldierUnlockEvents.OnSoldierUnlocked += HandleSoldierRescue;
    }

    private void OnDestroy() {
        SoldierUnlockEvents.OnSoldierUnlocked -= HandleSoldierRescue;
    }

    private void Start() {
        _sublieutenantEntry.Label.text = "Resgatar o Subtenente";
        _sargeantEntry.Label.text = "Resgatar o Sargento";
        _cadetEntry.Label.text = "Resgatar o Cadete";
    }

    private void HandleSoldierRescue(ISoldierState soldier) {
        string soldierType = soldier.GetType().Name;

        if (_missionTexts.TryGetValue(soldierType, out TextMeshProUGUI label)) {
            label.text = $"{label.text.Replace("Resgatar", "Resgatado")}";
            label.color = Color.green;
        }

        if (_missionEntries.TryGetValue(soldierType, out MissionEntryUI entry)) {
            StartCoroutine(FadeOutAndDisable(entry._canvasGroup, 2f, 0.5f));
        }
    }

    private IEnumerator FadeOutAndDisable(CanvasGroup canvasGroup, float delay, float duration) {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.gameObject.SetActive(false);
        canvasGroup.alpha = 1f;
    }
}
