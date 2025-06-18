using UnityEngine;

public class Sublieutenant : ISoldierState {
    [SerializeField] private float pushForce = 5f;

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Sublieutenant state entered.");
    }
    public override void OnUpdate() {
    }
    public override void OnExit() {
        base.OnExit();
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.TryGetComponent<PushableObject>(out var pushableObject)) {
            Vector3 direction = collision.transform.position - _transform.position;

            pushableObject.Push(direction, pushForce);

            SoundManager.PlaySound(SoundType.SOLD_EMPURRA);
        }
    }
}