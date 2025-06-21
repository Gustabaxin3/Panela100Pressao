using UnityEngine;
public class Cadet : ISoldierState
{
    protected override void Awake() {
        base.Awake();
        soldierType = SoldierType.Cadet;
    }
    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Cadet state entered.");
    }
    public override void OnUpdate()
    {
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}