using AUDIO;
using UnityEngine;

public class Sublieutenant : ISoldierState {
    [SerializeField] private float detectRadius = 1.5f;
    private PushableObject currentPushable;

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Sublieutenant state entered.");
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentPushable != null && currentPushable.IsBeingPushed) {
                currentPushable.StopPush();
                currentPushable = null;
                AudioManager.Instance.StopSoundEffect("SoldadoEmpurra01");
                return;
            }

            Collider[] hits = Physics.OverlapSphere(_transform.position, detectRadius);
            foreach (var hit in hits) {
                if (hit.TryGetComponent(out PushableObject pushable)) {
                    pushable.StartPush(_transform);
                    currentPushable = pushable;

                    AudioManager.Instance.PlaySoundEffect("Audio/Empurra/SoldadoEmpurra01", loop: true, position: transform.position, spatialBlend: 0); ;
                    break;
                }
            }
        }
    }

    public override void OnExit() {
        if (currentPushable != null) {
            currentPushable.StopPush();
            currentPushable = null;

            //AudioManager.Instance.StopSoundEffect("SoldadoEmpurra01");
        }
        base.OnExit();
    }
}
