using System.Collections;
using UnityEngine;

public class FinalBoxCutsceneTrigger : MonoBehaviour {
    [Header("Referências")]
    [SerializeField] private FinalBoxCutscene finalBoxCutscene;
    [SerializeField] private SoldierManager soldierManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private MissionID finalBoxMissionID;
    [SerializeField] private string interactionHint = "Pressione E para abrir a caixa final (apenas Captain)";

    private bool _playerInRange = false;
    private bool _cutsceneStarted = false;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Captain")) {
            if (soldierManager.GetCurrentSoldierType() == SoldierType.Captain && !_cutsceneStarted) {
                InteractionHintUI.Instance.ShowHint(interactionHint);
                _playerInRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Captain")) {
            InteractionHintUI.Instance.HideHint();
            _playerInRange = false;
        }
    }

    private void Update() {
        if (_cutsceneStarted) return;

        if (_playerInRange && Input.GetKeyDown(KeyCode.E)) {
            if (soldierManager.GetCurrentSoldierType() == SoldierType.Captain) {
                _cutsceneStarted = true;
                InteractionHintUI.Instance.HideHint();
                missionManager.CompleteMission(finalBoxMissionID);
                MissionFeedbackUI.ShowFeedback("Você chegou até o armário marrom!");
                StartCoroutine(PlayCutsceneAfterDelay());
            }
        }
    }

    private IEnumerator PlayCutsceneAfterDelay() {
        yield return new WaitForSeconds(2f);
        finalBoxCutscene.PlayCutscene();
    }
}