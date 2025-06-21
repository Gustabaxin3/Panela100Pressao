using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrampolineLauncher : MonoBehaviour {
    [SerializeField] private float _launchForce = 10f;
    [SerializeField] private float _standTimeToLaunch = 1f;

    private Collider _collider;
    private Transform _player;
    private Rigidbody _playerRb;
    private float _timeInside = 0f;
    private bool _playerInside = false;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out ISoldierState soldier)) {
            _player = soldier.transform;
            _playerRb = soldier.GetComponent<Rigidbody>();
            _timeInside = 0f;
            _playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (_player != null && other.transform == _player) {
            _playerInside = false;
            _player = null;
            _playerRb = null;
            _timeInside = 0f;
        }
    }

    private void Update() {
        if (_playerInside && _player != null && _playerRb != null) {
            Vector3 velocity = _playerRb.linearVelocity;
            Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);
            bool isStandingStill = horizontalVel.magnitude < 0.1f;

            if (isStandingStill) {
                _timeInside += Time.deltaTime;

                if (_timeInside >= _standTimeToLaunch) {
                    LaunchPlayer();
                    _timeInside = 0f;
                }
            } else {
                _timeInside = 0f; 
            }
        }
    }

    private void LaunchPlayer() {
        if (_playerRb != null) {
            _playerRb.linearVelocity = new Vector3(_playerRb.linearVelocity.x, 0f, _playerRb.linearVelocity.z); // zera Y antes
            _playerRb.AddForce(Vector3.up * _launchForce, ForceMode.VelocityChange);
            Debug.Log("Lan�ado pelo trampolim!");
        }
    }
}
