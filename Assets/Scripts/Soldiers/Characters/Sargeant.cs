using UnityEngine;

public class Sargeant : ISoldierState {
    [SerializeField] private float checkOffSet = 1f;
    [SerializeField] private float checkRadius = 2f;

    [SerializeField] private GameObject _originalParent;

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position + new Vector3(0, checkOffSet, 0), checkRadius, Vector3.up);

            foreach(RaycastHit hit in hits) {
                if (hit.collider.tag == "Zipline") {
                    hit.collider.GetComponent<Zipline>().StartZipLine(this.gameObject);
                }
            }
        }
    }
    public Transform ResetParentToOriginal() => this.gameObject.transform.parent = _originalParent.transform;
}
