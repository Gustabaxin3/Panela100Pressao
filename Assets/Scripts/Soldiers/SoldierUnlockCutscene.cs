using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using TMPro;
using AUDIO;

public class SoldierUnlockCutscene : MonoBehaviour
{
    [SerializeField] private ISoldierState _soldier;
    [SerializeField] private Animator _soldierAnimator;
    [SerializeField] private CinemachineCamera _cutsceneCamera;

    [SerializeField] private SoldierManager _soldierManager;

    [SerializeField] private float _startTransitionDuration = 1f;
    [SerializeField] private float _unlockAnimationDuration = 2f;
    [SerializeField] private float _endTransitionDuration = 1f;

    [SerializeField] private GameObject _AllHud;

    [Header("Cutscene UI")]
    [SerializeField] private TextMeshProUGUI _cutsceneText;
    [SerializeField] private CanvasGroup _cutsceneTextCanvasGroup;
    [SerializeField] private float _fadeDuration = 0.5f;

    [Header("Soldier Info")]
    [SerializeField] private string _soldierDisplayName = "Soldado";
    [SerializeField] private string _abilityHint = "Use E para\nativar a habilidade";

    [Header("Background Music")]
    [Range(0f, 1f)]
    [SerializeField] private float _musicVolumeDuringCutscene = 0.2f;

    [Header("Sons dos Soldados Desbloqueados")]
    private string[] soundsUnlockSoldier = {
        "Audio/Soldados/SoldadoRespeito01",
        "Audio/Soldados/SoldadoRespeito02",
        "Audio/Soldados/SoldadoRespeito03",
        "Audio/Soldados/SoldadoRespeito04"
    };

    private float _originalMusicVolume = 0.5f; //deixa em 0.5f na real

    private int? _firstPlayedIndex = null;

    private void Start()
    {
        _cutsceneCamera = GetComponentInParent<CinemachineCamera>();
        _cutsceneTextCanvasGroup.alpha = 0f;
        _cutsceneText.text = string.Empty;
    }

    private void OnEnable()
    {
        SoldierUnlockEvents.OnSoldierUnlocked += PlayCutsceneIfMatches;
    }

    private void OnDisable()
    {
        SoldierUnlockEvents.OnSoldierUnlocked -= PlayCutsceneIfMatches;
    }

    private void HandleHud(bool isOnCutscene)
    {
        CanvasGroup canvasGroup = _AllHud.GetComponent<CanvasGroup>();

        canvasGroup.alpha = isOnCutscene ? 0f : 1f;
        canvasGroup.interactable = !isOnCutscene;
        canvasGroup.blocksRaycasts = !isOnCutscene;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private IEnumerator ShowCutsceneText(string message)
    {
        if (_cutsceneText != null && _cutsceneTextCanvasGroup != null)
        {
            _cutsceneText.text = message;
            yield return StartCoroutine(FadeCanvasGroup(_cutsceneTextCanvasGroup, 0f, 1f, _fadeDuration));
        }
    }

    private IEnumerator HideCutsceneText()
    {
        if (_cutsceneTextCanvasGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(_cutsceneTextCanvasGroup, 1f, 0f, _fadeDuration));
        }
    }

    private void PlayCutsceneIfMatches(ISoldierState unlockedSoldier)
    {
        if (unlockedSoldier == _soldier)
        {
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene()
    {
        // Salva o volume atual da música
        AudioManager.Instance.musicMixer.audioMixer.GetFloat(AudioManager.MUSIC_VOLUME_PARAMETER_NAME, out float currentDb);
        _originalMusicVolume = Mathf.Pow(10f, currentDb / 20f);

        AudioManager.Instance.SetMusicVolume(_musicVolumeDuringCutscene, false);

        StartCutsceneTransition();
        yield return new WaitForSeconds(_startTransitionDuration);

        SwitchToCutsceneCamera();
        yield return new WaitForSeconds(_endTransitionDuration);

        yield return HideCutsceneText();

        yield return PlayUnlockAnimation();

        yield return ShowUnlockMessage();
        yield return ShowAbilityHint();

        yield return EndCutsceneTransition();


    }

    private void StartCutsceneTransition()
    {
        _soldierManager.PlayStartTransition();
        HandleHud(true);
    }

    private void SwitchToCutsceneCamera()
    {
        _soldierManager.PlayEndTransition();
        _cutsceneCamera.Priority = 20;
    }

    private IEnumerator PlayUnlockAnimation()
    {
        _soldierAnimator.SetBool("Unlock", true);
        yield return new WaitForSeconds(_unlockAnimationDuration);
        _soldierAnimator.SetBool("Unlock", false);
        _soldierAnimator.SetBool("Idle", true);
    }

    private IEnumerator ShowUnlockMessage()
    {

        // sorteia o som, sem repetir o primeiro escolhido
        int numSorteado;
        if (_firstPlayedIndex == null)
        {
            numSorteado = UnityEngine.Random.Range(0, soundsUnlockSoldier.Length);
            _firstPlayedIndex = numSorteado;
        }
        else
        {

            System.Collections.Generic.List<int> indices = new System.Collections.Generic.List<int>();
            for (int i = 0; i < soundsUnlockSoldier.Length; i++)
            {
                if (i != _firstPlayedIndex.Value) indices.Add(i);
            }
            numSorteado = indices[UnityEngine.Random.Range(0, indices.Count)];
        }

        AudioManager.Instance.PlaySoundEffect(soundsUnlockSoldier[numSorteado], spatialBlend: 0);


        yield return ShowCutsceneText($"Você desbloqueou\n{_soldierDisplayName}!");
        yield return new WaitForSeconds(1.5f);
        yield return HideCutsceneText();
    }

    private IEnumerator ShowAbilityHint()
    {
        yield return ShowCutsceneText(_abilityHint);
        yield return new WaitForSeconds(1.5f);
        yield return HideCutsceneText();
    }

    private IEnumerator EndCutsceneTransition()
    {
        _soldierManager.PlayStartTransition();
        yield return new WaitForSeconds(_startTransitionDuration);

        _cutsceneCamera.Priority = 0;
        HandleHud(false);

        _soldierManager.PlayEndTransition();
        yield return new WaitForSeconds(_endTransitionDuration);

        AudioManager.Instance.SetMusicVolume(_originalMusicVolume, false);
    }
}
