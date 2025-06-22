using UnityEngine;

public class RescuePoint : MonoBehaviour {
    [SerializeField] private ISoldierState _targetSoldier;

    [Header("Missão")]
    [Tooltip("O título da missão que será marcada como concluída")]
    [SerializeField] private MissionID _missionID;

    private bool _alreadyUnlocked = false;

    private void OnTriggerEnter(Collider other) {
        if (_alreadyUnlocked) return;

        if (other.TryGetComponent<ISoldierState>(out ISoldierState soldier)) {
            _alreadyUnlocked = true;

            SoldierUnlockEvents.Unlock(_targetSoldier);

            MissionManager.Instance.CompleteMission(_missionID);

            MissionFeedbackUI.ShowFeedback($"Soldado {_targetSoldier.soldierType} desbloqueado!");
        }
    }
}
