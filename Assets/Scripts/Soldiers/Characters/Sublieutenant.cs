using AUDIO;
using Unity.VisualScripting;
using UnityEngine;

public class Sublieutenant : ISoldierState {
    [SerializeField] private float detectRadius = 1.5f;
    private PushableObject currentPushable;

    //vari�veis para sorteio do �udio
    private string[] soundsEmpurra = {

        "Audio/Empurra/SoldadoEmpurra01",
        "Audio/Empurra/SoldadoEmpurra02",
        "Audio/Empurra/SoldadoEmpurra03",
        "Audio/Empurra/SoldadoEmpurra04"
    };
    private int numSorteado = -1;
    private int lastNumSorteado = -1;
    private AudioSource empurraSource = null;

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
                if (empurraSource != null)
                {
                    empurraSource.Stop();
                    GameObject.Destroy(empurraSource.gameObject);
                    empurraSource = null;
                    
                }
                numSorteado = -1;
                lastNumSorteado = -1;
                return;
            }

            Collider[] hits = Physics.OverlapSphere(_transform.position, detectRadius);
            foreach (var hit in hits) {
                if (hit.TryGetComponent(out PushableObject pushable)) {
                    pushable.StartPush(_transform);
                    currentPushable = pushable;

                    // implementa��o de �udio - SFX Empurra 
                                        
                    numSorteado = UnityEngine.Random.Range(0, soundsEmpurra.Length);
                    lastNumSorteado = numSorteado;
                    empurraSource = AudioManager.Instance.PlaySoundEffect(
                        soundsEmpurra[numSorteado],
                        loop: false,
                        position: transform.position,
                        spatialBlend: 0
                        );

                    break;
                }
            }
        }

        //loop manual e randomiza��o - for�a o pr�ximo som randomizado a n�o ser o �ltimo tocado

        if (empurraSource != null && !empurraSource.isPlaying && currentPushable != null && currentPushable.IsBeingPushed)
        {
            int novoSorteio;
            do
            {
                novoSorteio = UnityEngine.Random.Range(0, soundsEmpurra.Length);
            } while (novoSorteio == lastNumSorteado && soundsEmpurra.Length > 1);

            numSorteado = novoSorteio;
            lastNumSorteado = numSorteado;

            GameObject.Destroy(empurraSource.gameObject);

            empurraSource = AudioManager.Instance.PlaySoundEffect(
                        soundsEmpurra[numSorteado],
                        loop: false,
                        position: transform.position,
                        spatialBlend: 0
                        );

        }

    }

    public override void OnExit() {
        if (currentPushable != null) {
            currentPushable.StopPush();
            currentPushable = null;
        }

        if (empurraSource != null)
        {
            empurraSource.Stop();
            GameObject.Destroy(empurraSource.gameObject);
            empurraSource = null;
        }
        numSorteado = -1;
        lastNumSorteado = -1;
        base.OnExit();
    }
}
