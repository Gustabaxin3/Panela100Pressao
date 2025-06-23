using AUDIO;
using UnityEngine;
public class Cadet : ISoldierState {
    [SerializeField] private Light _spotLight;
    private bool _isLightOn = false;

    protected override void Awake() {
        base.Awake();
        soldierType = SoldierType.Cadet;
        _spotLight.enabled = _isLightOn;
    }

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Cadet state entered.");
    }

    public override void OnUpdate() {
        base.OnUpdate();
        if (Input.GetKeyDown(KeyCode.E)) {
            ToggleLight();
        }
    }

    public override void OnExit() {
        base.OnExit();
        ToggleLight();
    }

    private void ToggleLight() {
        AudioManager.Instance.PlaySoundEffect("Audio/Soldados/CadeteLanterna", volume: 0.35f, position: transform.position, spatialBlend: 0);
        _isLightOn = !_isLightOn;
        if (_spotLight != null)
            _spotLight.enabled = _isLightOn;
    }
}