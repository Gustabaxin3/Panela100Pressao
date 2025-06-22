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
            MissionID.ResgatarSubtenente => "Resgatar o Subtenente",
            MissionID.ResgatarSargento => "Resgatar o Sargento",
            MissionID.ResgatarCadete => "Resgatar o Cadete",
            MissionID.SairDoLabirinto => "Sair do Labirinto",
            MissionID.HackearTodasAsMaquinas => "Hackear Todas as Máquinas",
            _ => ID.ToString()
        };
    }
}
