using System.Collections;
using UnityEngine;

public class Zipline : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private Transform _ropeVisual;
    [SerializeField] private Transform _travelPoint;
    [SerializeField] private Transform _mountPoint;

    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _arrivalThreshold = 0.1f;

    private bool _isRiding = false;
    private bool _goingForward = true;
    private GameObject _rider;

    [Header("Player Y Offset")]
    [SerializeField] private float _riderYOffset = -1.5f;

    private Transform _originalLayer;

    void Start() {
        _travelPoint.position = _pointA.position;
        UpdateRopeVisual();
    }

    void Update() {
        if (!_isRiding) return;

        Transform target = _goingForward ? _pointB : _pointA;
        _travelPoint.position = Vector3.MoveTowards(
            _travelPoint.position,
            target.position,
            _speed * Time.deltaTime
        );

        // Sincronize o mountPoint com o travelPoint
        _mountPoint.position = _travelPoint.position;

        UpdateRopeVisual();

        if (Vector3.Distance(_travelPoint.position, target.position) <= _arrivalThreshold) {
            StartCoroutine(EndRide());
        }
    }

    private void UpdateRopeVisual() {
        Vector3 dir = _pointB.position - _pointA.position;
        float dist = dir.magnitude;
        _ropeVisual.position = _pointA.position + dir * 0.5f;
        _ropeVisual.rotation = Quaternion.LookRotation(dir);
        _ropeVisual.localScale = new Vector3(
            _ropeVisual.localScale.x,
            _ropeVisual.localScale.y,
            dist
        );
    }

    public void TryStartRide(GameObject player) {
        if (_isRiding) return;

        _rider = player;
        var rb = _rider.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        _rider.GetComponent<SoldierMovement>().SetMovementEnabled(false);

        _originalLayer = _rider.transform.parent;

        _rider.transform.SetParent(_travelPoint, worldPositionStays: false);
        _rider.transform.localPosition = new Vector3(0f, _riderYOffset, 0f);


        _goingForward = (_travelPoint.position == _pointA.position);

        Vector3 zipDir = (_goingForward ? _pointB.position - _pointA.position : _pointA.position - _pointB.position);
        zipDir.y = 0f;
        if (zipDir.sqrMagnitude > 0.001f) {
            _rider.transform.rotation = Quaternion.LookRotation(zipDir.normalized);
        }

        _isRiding = true;
    }

    private IEnumerator EndRide() {
        _isRiding = false;
        yield return null;

        if (_rider != null) {
            var rb = _rider.GetComponent<Rigidbody>();
            if (rb != null) {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
            var movement = _rider.GetComponent<SoldierMovement>();
            if (movement != null) {
                movement.SetMovementEnabled(true);
            }

            _rider.transform.parent = _originalLayer;
            _goingForward = !_goingForward;
            _rider = null;
        }
    }
}
