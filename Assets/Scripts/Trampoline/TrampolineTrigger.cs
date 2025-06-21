using UnityEngine;
using UnityEngine.Events;

public class TrampolineTrigger : MonoBehaviour {
    [SerializeField] private int _ballsNeeded = 5;
    [SerializeField] private GameObject _closedVisual;
    [SerializeField] private GameObject _openVisual;
    [SerializeField] private UnityEvent OnTrampolineActivated;

    private int _ballsDelivered = 0;
    private bool _isActivated = false;

    private void OnTriggerEnter(Collider other) {
        if (_isActivated) return;
        if (!other.TryGetComponent(out ISoldierState soldier)) return;

        int deliveredNow = BallInventory.Instance.DeliverBalls();
        _ballsDelivered += deliveredNow;

        Debug.Log($"Entregues: {deliveredNow} (Total: {_ballsDelivered})");

        if (_ballsDelivered >= _ballsNeeded) {
            ActivateTrampoline();
        }
    }

    private void ActivateTrampoline() {
        _isActivated = true;
        OnTrampolineActivated?.Invoke();

        if (_closedVisual != null) _closedVisual.SetActive(false);
        if (_openVisual != null) _openVisual.SetActive(true);

        Debug.Log("Trampolim ativado!");
    }
}
