using UnityEngine;

public class Sargeant : ISoldierState
{
    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
		Debug.Log("Sargeant state entered.");
    }
    public override void OnUpdate()
	{
	}
	public override void OnExit()
	{
        base.OnExit();
	}
}