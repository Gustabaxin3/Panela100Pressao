using UnityEngine;

public class BallCollectible : MonoBehaviour {
    public static event System.Action OnBallCollected;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out ISoldierState soldier)) {
            SoldierType type = soldier.soldierType;

            if (type == SoldierType.Cadet || type == SoldierType.Sublieutenant) {
                BallInventory.Instance.AddBall();
                OnBallCollected?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
