using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LabyrinthExitTrigger : MonoBehaviour {
    [Tooltip("Miss�o que ser� conclu�da quando o jogador entrar")]
    [SerializeField] private MissionID _missionID = MissionID.SairDoLabirinto;
    private string _missionTitle = "Miss�o 'Sair do Labirinto' foi completada!";

    private void Awake() {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Cadet"))
        {
            MissionFeedbackUI.ShowFeedback(_missionTitle);
            MissionManager.Instance.CompleteMission(_missionID);

            gameObject.SetActive(false);
        }
    }
}
