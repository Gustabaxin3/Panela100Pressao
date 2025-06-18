using UnityEngine;

public class Rope : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Captain")) {
            var ropeClimb = other.GetComponent<RopeClimb>();
            if (ropeClimb != null)
                ropeClimb.EnterRope(transform);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Captain")) {
            var ropeClimb = other.GetComponent<RopeClimb>();
            if (ropeClimb != null)
                ropeClimb.ExitRope();
        }
    }
}
