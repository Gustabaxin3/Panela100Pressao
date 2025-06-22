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
    public string GetDescription() {
        return ID switch {
            MissionID.ResgatarSubtenente => "Resgatar o Subtenente que está preso no labirinto.",
            MissionID.ResgatarSargento => "Resgatar o Sargento que está preso no labirinto.",
            MissionID.ResgatarCadete => "Resgatar o Cadete que está preso no labirinto.",
            MissionID.SairDoLabirinto => "Sair do labirinto e encontrar a saída.",
            MissionID.HackearTodasAsMaquinas => "Hackear todas as máquinas do labirinto.",
            _ => "Missão desconhecida."
        };
    }
}
