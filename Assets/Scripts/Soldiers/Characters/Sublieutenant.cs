using UnityEngine;

public class Sublieutenant : ISoldierState
{
    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Sublieutenant state entered.");
    }
    public override void OnUpdate()
    {
    }
    public override void OnExit()
    {
        base.OnExit();
    }
}