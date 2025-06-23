[System.Serializable]
public class Mission {
    public MissionID ID { get; private set; }
    public bool IsCompleted { get; private set; }

    public Mission(MissionID id) {
        ID = id;
        IsCompleted = false;
    }

    public void Complete() {
        if (IsCompleted) return;
        IsCompleted = true;
        MissionManager.Instance.NotifyMissionUpdated(this);
    }

    public string GetTitle() {
        return ID switch {
            MissionID.SairDoLabirinto => "Saia do Labirinto",
            MissionID.Trampolim => "Ative o Trampolim!",
            MissionID.HackearTodasAsMaquinas => "Hackear Todas as M�quinas",
            MissionID.IrAteArmarioMarrom => "V� at� o Arm�rio Marrom",
            _ => ID.ToString()
        };
    }
    public string GetDescription() {
        return ID switch {
            MissionID.SairDoLabirinto => "Sair do labirinto e encontrar a sa�da.",
            MissionID.Trampolim => "Recupere todas as bolas do trampolim perdidas debaixo dos m�veis!",
            MissionID.HackearTodasAsMaquinas => "Hackear todas as m�quinas do labirinto.",
            MissionID.IrAteArmarioMarrom => "V� pelas prateleiras e encontre o bot�o em cima do arm�rio.",
            _ => "Miss�o desconhecida."
        };
    }
}
