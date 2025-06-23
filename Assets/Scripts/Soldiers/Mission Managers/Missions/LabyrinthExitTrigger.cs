using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LabyrinthExitTrigger : MonoBehaviour {
    [Tooltip("Miss�o que ser� conclu�da quando o jogador entrar")]
    [SerializeField] private MissionID _missionID = MissionID.SairDoLabirinto;
    private string _missionTitle = "Miss�o 'Sair do Labirinto' foi completada!";

    [Header("Soldado a desbloquear ap�s o labirinto")]
    [SerializeField] private ISoldierState soldierToUnlock;

    private void Awake() {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cadet")) {
            StartCoroutine(HandleExit());
        }
    }

    private IEnumerator HandleExit() {
        MissionFeedbackUI.ShowFeedback(_missionTitle);
        MissionManager.Instance.CompleteMission(_missionID);

        yield return new WaitForSeconds(2f);

        SoldierUnlockEvents.Unlock(soldierToUnlock);

        gameObject.SetActive(false);
    }
}
