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
            MissionID.HackearTodasAsMaquinas => "Hackear Todas as Máquinas",
            MissionID.IrAteArmarioMarrom => "Vá até o Armário Marrom",
            _ => ID.ToString()
        };
    }
    public string GetDescription() {
        return ID switch {
            MissionID.SairDoLabirinto => "Sair do labirinto e encontrar a saída.",
            MissionID.Trampolim => "Recupere todas as bolas do trampolim perdidas debaixo dos móveis!",
            MissionID.HackearTodasAsMaquinas => "Hackear todas as máquinas do labirinto.",
            MissionID.IrAteArmarioMarrom => "Vá pelas prateleiras e encontre o botão em cima do armário.",
            _ => "Missão desconhecida."
        };
    }
}
