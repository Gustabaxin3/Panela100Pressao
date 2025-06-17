using System;

public static class SoldierUnlockEvents {
    public static event Action<ISoldierState> OnSoldierUnlocked;

    public static void Unlock(ISoldierState soldierState) {
        OnSoldierUnlocked?.Invoke(soldierState);
    }
}
