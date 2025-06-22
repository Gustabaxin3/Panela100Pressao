using UnityEngine;

public class BallCollectible : MonoBehaviour {
    public static event System.Action OnBallCollected;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cadet") || other.CompareTag("Sublieutenant")) {
            Debug.Log("Soldier collected a ball!");

            BallInventory.Instance.AddBall(this.gameObject);
        }
    }
}
