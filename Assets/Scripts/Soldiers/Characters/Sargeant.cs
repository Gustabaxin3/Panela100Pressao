using UnityEngine;

public class Sargeant : ISoldierState {
    [SerializeField] private float checkOffSet = 1f;
    [SerializeField] private float checkRadius = 2f;

    [SerializeField] private GameObject _defaultPose;
    [SerializeField] private GameObject _zipLinePose;

    protected override void Start() {
        _defaultPose.SetActive(true);
        _zipLinePose.SetActive(false);
    }
    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0, checkOffSet, 0), checkRadius, Vector3.up);

            foreach(RaycastHit hit in hits) {
                if (hit.collider.tag == "Zipline") {
                    this.transform.position = hit.collider.transform.position;
                    hit.collider.GetComponent<Zipline>().StartZipLine(this.gameObject);
                    _defaultPose.SetActive(false);
                    _zipLinePose.SetActive(true);
                }
            }
        }
    }
    public Transform ResetToOriginal() {
        _zipLinePose.SetActive(false);
        _defaultPose.SetActive(true);
        return this.gameObject.transform.parent = _soldierManager._originalParent; 
    }
}
