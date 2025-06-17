using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PushableObject : MonoBehaviour {
    private Rigidbody _rigidBody;
    private void Awake() {
        if(_rigidBody == null) this.AddComponent<Rigidbody>();

        _rigidBody = GetComponent<Rigidbody>();
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }
    public void Push(Vector3 direction, float force) {
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

        _rigidBody.AddForce(direction.normalized * force, ForceMode.Impulse);

        StartCoroutine(FreezeLater());
    }
    private IEnumerator FreezeLater() {
        yield return new WaitForSeconds(0.5f);
        _rigidBody.linearVelocity = Vector3.zero;
        _rigidBody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
