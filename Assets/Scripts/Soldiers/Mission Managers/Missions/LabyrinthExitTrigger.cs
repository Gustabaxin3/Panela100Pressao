using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class LabyrinthExitTrigger : MonoBehaviour {
    [Tooltip("Missão que será concluída quando o jogador entrar")]
    [SerializeField] private MissionID _missionID = MissionID.SairDoLabirinto;
    private string _missionTitle = "Missão 'Sair do Labirinto' foi completada!";

    [Header("Soldado a desbloquear após o labirinto")]
    [SerializeField] private ISoldierState soldierToUnlock;
    [SerializeField] private SoldierSelectorUI soldierSelectorUI;

    private void Awake() {
        var col = GetComponent<Collider>();
        soldierSelectorUI = FindObjectOfType<SoldierSelectorUI>();

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
        soldierSelectorUI.HandleAllChoicesInteractivity();
        gameObject.SetActive(false);
    }
}
