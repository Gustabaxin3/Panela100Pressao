using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class FinalBoxCutscene : MonoBehaviour {
    [Header("Referências")]
    [SerializeField] private GameObject boxObject;
    [SerializeField] private CinemachineCamera cutsceneCamera;
    [SerializeField] private SoldierManager soldierManager;
    [SerializeField] private GameObject allHud;

    [Header("Cutscene UI")]
    [SerializeField] private TextMeshProUGUI cutsceneText;
    [SerializeField] private CanvasGroup cutsceneTextCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;
    [TextArea]
    [SerializeField] private string[] finalMessages = { "Vá até a caixa!" };

    [Header("Transições")]
    [SerializeField] private float startTransitionDuration = 1f;
    [SerializeField] private float endTransitionDuration = 1f;
    [SerializeField] private float boxDropDelay = 0.5f;
    [SerializeField] private float boxDropAnimationDuration = 2f;
    private Vector3 boxStartPosition;
    private Rigidbody boxRigidbody;

    private void Start() {
        boxObject.SetActive(false);
        cutsceneTextCanvasGroup.alpha = 0f;
        cutsceneText.text = string.Empty;
        boxStartPosition = boxObject.transform.position;
        boxRigidbody = boxObject.GetComponent<Rigidbody>();
        boxRigidbody.isKinematic = true;
    }

    private void Update() {
        PlayCutscene();
    }

    public void PlayCutscene() {
        StartCoroutine(PlayFinalCutscene());
    }

    private IEnumerator PlayFinalCutscene() {
        soldierManager.PlayStartTransition();
        HandleHud(true);
        yield return new WaitForSeconds(startTransitionDuration);

        cutsceneCamera.Priority = 20;

        soldierManager.PlayEndTransition();
        yield return new WaitForSeconds(endTransitionDuration);

        yield return new WaitForSeconds(boxDropDelay);
        boxObject.SetActive(true);

        float originalGravity = Physics.gravity.y;
        float fastGravity = originalGravity * 25f;
        if (boxRigidbody != null) {
            boxObject.transform.position = boxStartPosition;
            boxRigidbody.linearVelocity = Vector3.zero;
            boxRigidbody.angularVelocity = Vector3.zero;
            boxRigidbody.isKinematic = false;
            Physics.gravity = new Vector3(0, fastGravity, 0);
            yield return new WaitForSeconds(boxDropAnimationDuration);
            boxRigidbody.isKinematic = true;
            Physics.gravity = new Vector3(0, originalGravity, 0);
        } else {
            yield return new WaitForSeconds(boxDropAnimationDuration);
        }

        foreach (var msg in finalMessages) {
            yield return ShowCutsceneText(msg);
            yield return new WaitForSeconds(1.5f);
            yield return HideCutsceneText();
        }

        soldierManager.PlayStartTransition();
        yield return new WaitForSeconds(startTransitionDuration);

        cutsceneCamera.Priority = 0;
        HandleHud(false);

        soldierManager.PlayEndTransition();
        yield return new WaitForSeconds(endTransitionDuration);
    }

    private void HandleHud(bool isOnCutscene) {
        CanvasGroup canvasGroup = allHud.GetComponent<CanvasGroup>();
        canvasGroup.alpha = isOnCutscene ? 0f : 1f;
        canvasGroup.interactable = !isOnCutscene;
        canvasGroup.blocksRaycasts = !isOnCutscene;
    }

    private IEnumerator ShowCutsceneText(string message) {
        cutsceneText.text = message;
        yield return StartCoroutine(FadeCanvasGroup(cutsceneTextCanvasGroup, 0f, 1f, fadeDuration));
    }

    private IEnumerator HideCutsceneText() {
        yield return StartCoroutine(FadeCanvasGroup(cutsceneTextCanvasGroup, 1f, 0f, fadeDuration));
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
}