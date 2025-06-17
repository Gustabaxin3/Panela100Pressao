using UnityEngine;
public class Cadet : ISoldierState
{
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