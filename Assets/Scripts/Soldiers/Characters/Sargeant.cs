using UnityEngine;

public class Sargeant : ISoldierState {
    [Header("Zipline Raycast Settings")]
    [SerializeField] private float _checkOffSet = 1f;
    [SerializeField] private float _checkRadius = 2f;
    [SerializeField] public Animator _animator;

    protected override void Awake() {
        base.Awake();
        soldierType = SoldierType.Sargeant;
    }
    protected override void Start() {
        base.Start();
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
                _animator.SetBool("Zipline", true);
                InteractionHintUI.Instance.HideHint();
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