using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrampolineTrigger : MonoBehaviour {
    [Header("Configuração")]
    [SerializeField] private int ballsNeeded = 5;

    [Header("Aesthetic Balls")]
    [Tooltip("Arraste aqui os GameObjects das bolinhas que começam desativadas")]
    [SerializeField] private GameObject[] trampolineBalls;

    private int ballsDelivered = 0;
    public bool IsActivated { get; private set; } = false;

    private void OnTriggerEnter(Collider other) {
        Debug.Log($"TrampolineTrigger: {other.name} entrou no gatilho.");
        if (IsActivated) return;
        if (other.CompareTag("Cadet") || other.CompareTag("Sublieutenant")) {

            List<GameObject> delivered = BallInventory.Instance.DeliverBalls();
            int count = delivered.Count;

            for (int i = 0; i < count; i++) {
                ActivateNextBall();
                ballsDelivered++;
                Debug.Log($"Bolinhas entregues (trampolim): {ballsDelivered}/{ballsNeeded}");
                if (ballsDelivered >= ballsNeeded) {
                    ActivateTrampoline();
                    break;
                }
            }
        }
    }

    private void ActivateNextBall() {
        for (int i = 0; i < trampolineBalls.Length; i++) {
            if (trampolineBalls[i] != null && !trampolineBalls[i].activeSelf) {
                trampolineBalls[i].SetActive(true);
                return;
            }
        }
    }

    private void ActivateTrampoline() {
        IsActivated = true;
        Debug.Log("Trampolim ativado!");
        MissionFeedbackUI.ShowFeedback("Trampolim foi ativado!");
        MissionManager.Instance.CompleteMission(MissionID.Trampolim);

    }
}
