using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour {
    public static MissionManager Instance { get; private set; }
    private List<Mission> _missions = new List<Mission>();
    private List<IMissionObserver> _observers = new List<IMissionObserver>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() {
        AddMission(MissionID.SairDoLabirinto);
    }
    public void AddMission(MissionID id) {
        if (_missions.Exists(m => m.ID == id)) return;
        var mission = new Mission(id);
        _missions.Add(mission);
        NotifyMissionUpdated(mission);
    }

    public void CompleteMission(MissionID id) {
        var mission = _missions.Find(m => m.ID == id);
        mission?.Complete();
    }

    public IReadOnlyList<Mission> Missions => _missions;

    public void RegisterObserver(IMissionObserver observer) {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        // envia todas as missões atuais
        foreach (var mission in _missions)
            observer.OnMissionUpdated(mission);
    }

    public void UnregisterObserver(IMissionObserver observer) {
        if (_observers.Contains(observer))
            _observers.Remove(observer);
    }

    public void NotifyMissionUpdated(Mission mission) {
        foreach (var obs in _observers)
            obs.OnMissionUpdated(mission);
    }
}
