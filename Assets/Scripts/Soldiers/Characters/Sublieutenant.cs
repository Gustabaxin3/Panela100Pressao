using AUDIO;
using UnityEngine;

public class Sublieutenant : ISoldierState {
    [SerializeField] private float detectRadius = 1.5f;
    private PushableObject currentPushable;

    //variáveis para sorteio do áudio
    private string[] soundsEmpurra = {

        "Audio/Empurra/SoldadoEmpurra01",
        "Audio/Empurra/SoldadoEmpurra02",
        "Audio/Empurra/SoldadoEmpurra03",
        "Audio/Empurra/SoldadoEmpurra04"
    };
    private int numSorteado = -1;

    public override void OnEnter(SoldierManager soldierManager) {
        base.OnEnter(soldierManager);
        Debug.Log("Sublieutenant state entered.");
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (currentPushable != null && currentPushable.IsBeingPushed) {
                currentPushable.StopPush();
                currentPushable = null;

                //para de tocar o SFX Empurra
                if (numSorteado >= 0)
                {
                    string soundPath = soundsEmpurra[numSorteado];
                    string soundName = System.IO.Path.GetFileNameWithoutExtension(soundPath);
                    
                    AudioManager.Instance.StopSoundEffect(soundName);
                }
                numSorteado = -1;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(_transform.position, detectRadius);
            foreach (var hit in hits) {
                if (hit.TryGetComponent(out PushableObject pushable)) {
                    pushable.StartPush(_transform);
                    currentPushable = pushable;

                    // implementação de áudio - SFX Empurra 
                                        
                    numSorteado = UnityEngine.Random.Range(0, soundsEmpurra.Length);
                    AudioManager.Instance.PlaySoundEffect(soundsEmpurra[numSorteado], loop: true, position: transform.position, spatialBlend: 0); ;
                    
                    break;
                }
            }
        }
    }

    public override void OnExit() {
        if (currentPushable != null) {
            currentPushable.StopPush();
            currentPushable = null;
        }
        base.OnExit();
    }
}
