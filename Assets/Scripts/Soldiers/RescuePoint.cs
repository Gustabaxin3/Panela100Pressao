using UnityEngine;

public class RescuePoint : MonoBehaviour {
    [SerializeField] private LayerMask soldierLayerMask;
    [SerializeField] private ISoldierState _targetSoldier;

    private void OnTriggerEnter(Collider other) {
        if (!IsInLayerMask(other.gameObject.layer, soldierLayerMask)) return;

        if (other.TryGetComponent<ISoldierState>(out ISoldierState soldier)) {
            SoldierUnlockEvents.Unlock(_targetSoldier);

            Debug.Log($"Soldado {_targetSoldier} resgatado por {other.name}");

            Destroy(gameObject);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask) => ((1 << layer) & mask) != 0;
}
