using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using TMPro;

public class SoldierUnlockCutscene : MonoBehaviour {
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

    private void Start() {
        _cutsceneCamera = GetComponentInParent<CinemachineCamera>();
        _cutsceneTextCanvasGroup.alpha = 0f;
        _cutsceneText.text = string.Empty;
    }

    private void OnEnable() {
        SoldierUnlockEvents.OnSoldierUnlocked += PlayCutsceneIfMatches;
    }

    private void OnDisable() {
        SoldierUnlockEvents.OnSoldierUnlocked -= PlayCutsceneIfMatches;
    }

    private void HandleHud(bool isOnCutscene) {
        CanvasGroup canvasGroup = _AllHud.GetComponent<CanvasGroup>();

        canvasGroup.alpha = isOnCutscene ? 0f : 1f;
        canvasGroup.interactable = !isOnCutscene;
        canvasGroup.blocksRaycasts = !isOnCutscene;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration) {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }

    private IEnumerator ShowCutsceneText(string message) {
        if (_cutsceneText != null && _cutsceneTextCanvasGroup != null) {
            _cutsceneText.text = message;
            yield return StartCoroutine(FadeCanvasGroup(_cutsceneTextCanvasGroup, 0f, 1f, _fadeDuration));
        }
    }

    private IEnumerator HideCutsceneText() {
        if (_cutsceneTextCanvasGroup != null) {
            yield return StartCoroutine(FadeCanvasGroup(_cutsceneTextCanvasGroup, 1f, 0f, _fadeDuration));
        }
    }

    private void PlayCutsceneIfMatches(ISoldierState unlockedSoldier) {
        if (unlockedSoldier == _soldier) {
            StartCoroutine(PlayCutscene());
        }
    }

    private IEnumerator PlayCutscene() {

        _soldierManager.PlayStartTransition();
        HandleHud(true);

        yield return new WaitForSeconds(_startTransitionDuration);

        _soldierManager.PlayEndTransition();
        _cutsceneCamera.Priority = 20;

        yield return new WaitForSeconds(_endTransitionDuration);

        yield return HideCutsceneText();

        _soldierAnimator.SetBool("Unlock", true);

        yield return new WaitForSeconds(_unlockAnimationDuration);
        _soldierAnimator.SetBool("Unlock", false);
        _soldierAnimator.SetBool("Idle", true);

        yield return ShowCutsceneText($"Você desbloqueou\n{_soldierDisplayName}!");
        yield return new WaitForSeconds(1.5f);
        yield return HideCutsceneText();

        yield return ShowCutsceneText(_abilityHint);
        yield return new WaitForSeconds(1.5f);
        yield return HideCutsceneText();

        _soldierManager.PlayStartTransition();

        yield return new WaitForSeconds(_startTransitionDuration);
        _cutsceneCamera.Priority = 0;
        HandleHud(false);

        _soldierManager.PlayEndTransition();

        yield return new WaitForSeconds(_endTransitionDuration);
    }
}
