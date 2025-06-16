using Unity.Cinemachine;
using UnityEngine;

public class SoldierManager : MonoBehaviour {
    private ISoldierState _currentSoldier;

    public Captain captain;
    public Sublieutenant sublieutenant;
    public Sargeant sargeant;
    public Cadet cadet;

    private void Start() {
        ChangeState(captain);
    }

    private void Update() {
        _currentSoldier?.OnUpdate();
        ChooseSoldier();
    }
    private void FixedUpdate() {
        _currentSoldier?.FixedUpdate();
    }
    private void LateUpdate() {
        _currentSoldier?.LateUpdate();
    }
    private void ChangeState(ISoldierState newState) {
        _currentSoldier?.OnExit();
        _currentSoldier = newState;
        _currentSoldier.OnEnter(this);
    }

    private void ChooseSoldier() {
        switch (true) {
            case bool _ when Input.GetKeyDown(KeyCode.Alpha1): ChangeState(captain); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha2): ChangeState(sublieutenant); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha3): ChangeState(sargeant); break;
            case bool _ when Input.GetKeyDown(KeyCode.Alpha4): ChangeState(cadet); break;
        }
    }
}
