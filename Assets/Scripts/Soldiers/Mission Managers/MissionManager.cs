using System.Collections.Generic;
using UnityEngine;
using AUDIO;

public class MissionManager : MonoBehaviour {
    public static MissionManager Instance { get; private set; }
    private List<Mission> _missions = new List<Mission>();
    private List<IMissionObserver> _observers = new List<IMissionObserver>();

    [SerializeField]
    private List<MissionID> _missionOrder = new List<MissionID> {
        MissionID.SairDoLabirinto,
        MissionID.ResgatarSubtenente,
        MissionID.Trampolim,
        MissionID.ResgatarSargento,
        MissionID.HackearTodasAsMaquinas,
    };

    private int _currentMissionIndex = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start() {
        if (_missionOrder.Count > 0)
            AddMission(_missionOrder[0]);
    }
    public void AddMission(MissionID id) {
        if (_missions.Exists(m => m.ID == id)) return;
        var mission = new Mission(id);
        _missions.Add(mission);
        NotifyMissionUpdated(mission);
    }

    public void CompleteMission(MissionID id) {

        AudioManager.Instance.PlaySoundEffect("Audio/UI/MissaoConcluida", spatialBlend: 0);
        
        var mission = _missions.Find(m => m.ID == id);
        if (mission != null && !mission.IsCompleted) {
            mission.Complete();
            int index = _missionOrder.IndexOf(id);
            if (index != -1 && index + 1 < _missionOrder.Count) {
                AddMission(_missionOrder[index + 1]);
                _currentMissionIndex = index + 1;
            }
        }
    }

    public IReadOnlyList<Mission> Missions => _missions;

    public void RegisterObserver(IMissionObserver observer) {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
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
    public void TryAddNextMissionAfter(MissionID completedMissionID) {
        int index = _missionOrder.IndexOf(completedMissionID);
        if (index != -1 && index + 1 < _missionOrder.Count) {
            AddMission(_missionOrder[index + 1]);
            _currentMissionIndex = index + 1;
        }
    }
}
