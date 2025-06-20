using System.Collections;
using UnityEngine;

public class Zipline : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;
    [SerializeField] private Transform _ropeVisual;
    [SerializeField] private Transform _travelPoint;

    [Header("Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _arrivalThreshold = 0.1f;

    private bool _isRiding = false;
    private bool _goingForward = true;
    private GameObject _rider;

    [Header("Player Y Offset")]
    [SerializeField] private float _riderYOffset = -1.4f;

    private Transform _originalLayer;

    void Start() {
        UpdateRopeVisual();
    }

    void FixedUpdate() {
        if (!_isRiding) return;

        Transform target = _goingForward ? _pointB : _pointA;
        _travelPoint.position = Vector3.MoveTowards(
            _travelPoint.position,
            target.position,
            _speed * Time.deltaTime
        );

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

        float distanceToA = Vector3.Distance(player.transform.position, _pointA.position);
        float distanceToB = Vector3.Distance(player.transform.position, _pointB.position);

        _goingForward = distanceToA < distanceToB;

        _travelPoint.position = _goingForward ? _pointA.position : _pointB.position;

        _rider.transform.SetParent(_travelPoint, worldPositionStays: false);
        _rider.transform.localPosition = new Vector3(0f, _riderYOffset, 0f);

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
            Rigidbody rb = _rider.GetComponent<Rigidbody>();
            if (rb != null) 
                rb.useGravity = true;
                rb.isKinematic = false;

            SoldierMovement movement = _rider.GetComponent<SoldierMovement>();
            if (movement != null) 
                movement.SetMovementEnabled(true);
            

            _rider.transform.parent = _originalLayer;
            _goingForward = !_goingForward;

            Sargeant Sargeant = _rider.GetComponent<Sargeant>();
            if (Sargeant != null) 
                Sargeant.ChangePose(false);
            

            _rider = null;
        }
    }
}
