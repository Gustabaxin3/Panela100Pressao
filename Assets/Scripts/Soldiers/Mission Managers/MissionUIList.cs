using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class MissionUIList : MonoBehaviour, IMissionObserver {
    [SerializeField] private GameObject _entryPrefab;
    [SerializeField] private Transform _contentParent;
    [SerializedDictionary("ID", "Entry")]
    private SerializedDictionary<MissionID, MissionEntryUI> _entries = new SerializedDictionary<MissionID, MissionEntryUI>();

    private void Start() {
        MissionManager.Instance.RegisterObserver(this);
    }

    private void OnDisable() {
        if (MissionManager.Instance != null)
            MissionManager.Instance.UnregisterObserver(this);
    }

    public void OnMissionUpdated(Mission mission) {
        if (!_entries.ContainsKey(mission.ID)) {
            var go = Instantiate(_entryPrefab, _contentParent);
            var entry = go.GetComponent<MissionEntryUI>();
            entry.Initialize(mission, () => MissionManager.Instance.TryAddNextMissionAfter(mission.ID));
            _entries.Add(mission.ID, entry);
            return;
        }
        _entries[mission.ID].UpdateEntry(mission);
    }
}