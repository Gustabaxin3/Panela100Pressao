using UnityEngine;

public class BoxMissionTrigger : MonoBehaviour
{
    [SerializeField] private SoldierManager soldierManager;
    [SerializeField] private MissionManager missionManager;
    [SerializeField] private MissionID missionID = MissionID.IrAteCaixa;

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
            missionManager.CompleteMission(missionID);
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
}