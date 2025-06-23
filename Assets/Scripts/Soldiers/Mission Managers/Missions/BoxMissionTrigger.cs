using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoxMissionTrigger : MonoBehaviour
{
    [SerializeField] private SoldierManager soldierManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private MissionID missionID = MissionID.IrAteCaixa;
    [SerializeField] private CinemachineCamera missionCamera;
    [SerializeField] private CanvasGroup HudChoice;
    [SerializeField] private CanvasGroup HudHint;
    [SerializeField] private CanvasGroup HudMission;



    private void Update() {
        if(Input.GetKeyDown(KeyCode.F)) {
            StartCoroutine(ShowMissionCompleteFeedbackAndTransition());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ISoldierState soldier = other.GetComponent<ISoldierState>();
        if (soldier == null) return;

        SoldierType type = soldierManager.GetCurrentSoldierType();
        if (!soldierManager.IsSoldierActive(type)) return;

        soldierManager.SetSoldierActive(type, false);
        other.gameObject.SetActive(false);

        if (!soldierManager.IsCaptainActive && !soldierManager.IsSublieutenantActive &&
            !soldierManager.IsSargeantActive && !soldierManager.IsCadetActive)
        {
            missionCamera.Priority = 50;
            missionManager.CompleteMission(missionID);
            MissionFeedbackUI.ShowFeedback("Todos os soldados Foram Salvos. Missão concluída.");
            return;
        }

        SoldierType[] allTypes = { SoldierType.Captain, SoldierType.Sublieutenant, SoldierType.Sargeant, SoldierType.Cadet };
        var available = new System.Collections.Generic.List<SoldierType>();
        foreach (var t in allTypes)
            if (soldierManager.IsSoldierActive(t) && t != type)
                available.Add(t);

        if (available.Count > 0)
        {
            var nextType = available[Random.Range(0, available.Count)];
            switch (nextType)
            {
                case SoldierType.Captain: soldierManager.SelectCaptain(); break;
                case SoldierType.Sublieutenant: soldierManager.SelectSublieutenant(); break;
                case SoldierType.Sargeant: soldierManager.SelectSargeant(); break;
                case SoldierType.Cadet: soldierManager.SelectCadet(); break;
            }
        }
    }
    private IEnumerator ShowMissionCompleteFeedbackAndTransition() {
        missionCamera.Priority = 50;
        missionManager.CompleteMission(missionID);
        HudChoice.alpha = 0;
        HudHint.alpha = 0;
        HudMission.alpha = 0;
        MissionFeedbackUI.ShowFeedback("Todos os soldados Foram Salvos. Missão concluída.");
        yield return new WaitForSeconds(3f);
        soldierManager.PlayStartTransitionWithoutDisable();
        yield return new WaitForSeconds(2f);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}