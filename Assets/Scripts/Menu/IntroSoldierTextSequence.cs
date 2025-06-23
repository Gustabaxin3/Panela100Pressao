using AUDIO;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSoldierTextSequence : MonoBehaviour {
    [Header("Referências")]
    [SerializeField] private Animator _transitionAnimator;
    [SerializeField] private TextMeshProUGUI _cutsceneText;
    [SerializeField] private CanvasGroup _cutsceneTextCanvasGroup;
    [SerializeField] private string _sceneToLoad = "GameScene";
    [Header("Configuração de Mensagens")]
    [SerializeField] private string _soldierDisplayName = "Cadete";
    [SerializeField]
    [TextArea]
    private string[] _introMessages = {
    };
    [SerializeField]
    [TextArea]
    private string[] _abilityHints = {
    };

    [Header("Fade & Timing")]
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private float _textDisplayTime = 1.5f;
    [SerializeField] private CinemachineCamera _introCamera;
    private void Start() {
        _cutsceneTextCanvasGroup.alpha = 0f;
        _cutsceneText.text = "";
    }
    public void SetIntroCameraPriority(int priority) {
        if (_introCamera != null) {
            _introCamera.Priority = priority;
        }
    }
    public void PlayIntroSequenceExternally() {
        StartCoroutine(PlayIntroSequence());
    }
    private IEnumerator PlayIntroSequence() {
        _transitionAnimator.SetBool("Start", true);


        yield return new WaitForSeconds(GetCurrentAnimationDuration() + 1f);
        var brain = Camera.main.GetComponent<CinemachineBrain>();
        brain.DefaultBlend.Style = CinemachineBlendDefinition.Styles.Cut;
        yield return new WaitForSeconds(GetCurrentAnimationDuration());
        _transitionAnimator.SetBool("Start", false);
        _transitionAnimator.SetBool("End", true);
        yield return new WaitForSeconds(GetCurrentAnimationDuration());
        SetIntroCameraPriority(30);

        yield return ShowMessages(_introMessages);
        yield return ShowMessages(_abilityHints);


        _transitionAnimator.SetBool("End", false);

        _transitionAnimator.SetBool("Start", true);
        yield return new WaitForSeconds(GetCurrentAnimationDuration());

        AudioManager.Instance.StopAllTracks();
        SceneManager.LoadScene(_sceneToLoad, LoadSceneMode.Single);
    }

    private IEnumerator ShowMessages(string[] messages) {
        foreach (var msg in messages) {
            string formatted = string.Format(msg, _soldierDisplayName);
            yield return ShowText(formatted);
            yield return new WaitForSeconds(_textDisplayTime);
            yield return HideText();
        }
    }

    private IEnumerator ShowText(string message) {
        _cutsceneText.text = message;
        yield return FadeCanvasGroup(_cutsceneTextCanvasGroup, 0f, 1f, _fadeDuration);
    }

    private IEnumerator HideText() {
        yield return FadeCanvasGroup(_cutsceneTextCanvasGroup, 1f, 0f, _fadeDuration);
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

    private float GetCurrentAnimationDuration() {
        return _transitionAnimator.GetCurrentAnimatorStateInfo(0).length;
    }
}
