using AUDIO;
using Unity.VisualScripting;
using UnityEngine;

public class Sublieutenant : ISoldierState
{
    [SerializeField] private float detectRadius = 1.5f;
    private PushableObject currentPushable;
    [SerializeField] private Animator _animator;
    private bool _isPushing = false;

    protected override void Awake()
    {
        base.Awake();
        soldierType = SoldierType.Sublieutenant;
    }
    public override void OnEnter(SoldierManager soldierManager)
    {
        base.OnEnter(soldierManager);
        Debug.Log("Sublieutenant state entered.");
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentPushable != null && currentPushable.IsBeingPushed) {
                
                _animator.SetBool("Push", false);
                currentPushable.StopPush();
                _isPushing = false;
                _soldierMovement.SpeedOverrides.Remove(PushSpeedOverride);
                _soldierMovement.IsPushing = _isPushing;
                currentPushable = null;
            } else {
                Collider[] hits = Physics.OverlapSphere(_transform.position, detectRadius);
                foreach (var hit in hits) {
                    if (hit.TryGetComponent(out PushableObject pushable) && !pushable.IsBeingPushed) {
                        pushable.StartPush(_transform);
                        currentPushable = pushable;
                        _isPushing = true;
                        _animator.SetBool("Push", true);
                        _soldierMovement.SpeedOverrides.Add(PushSpeedOverride);
                        _soldierMovement.IsPushing = _isPushing;
                        break;
                    }
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E)) {
            InteractionHintUI.Instance.HideHint();
        }
    }

    public override void OnExit()
    {
        if (currentPushable != null)
        {
            currentPushable.StopPush();
            currentPushable = null;
        }
        base.OnExit();
    }

    private float PushSpeedOverride() => 2f; 
}
