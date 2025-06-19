using UnityEngine;

public class Sargeant : ISoldierState {
    [Header("Zipline Raycast Settings")]
    [SerializeField] private float _checkOffSet = 1f;
    [SerializeField] private float _checkRadius = 2f;

    [SerializeField] private GameObject _defaultPose;
    [SerializeField] private GameObject _zipLinePose;

    protected override void Start() {
        base.Start();
        _defaultPose.SetActive(true);
        _zipLinePose.SetActive(false);
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            TryDetectZipline();
        }
    }

    private void TryDetectZipline() {
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 dir = transform.forward;

        RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0, _checkOffSet, 0), _checkRadius, Vector3.up);
        foreach (RaycastHit hit in hits) {
            if (hit.collider.tag == "ZiplinePoint") {
                var zip = hit.collider.GetComponentInParent<Zipline>();
                zip.TryStartRide(this.gameObject);
                InteractionHintUI.Instance.HideHint();
                _defaultPose.SetActive(false);
                _zipLinePose.SetActive(true);
            }
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("ZiplinePoint")) {
            InteractionHintUI.Instance.ShowHint("Pressione E para usar a tirolesa.");
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("ZiplinePoint")) {
            InteractionHintUI.Instance.HideHint();
        }
    }
}