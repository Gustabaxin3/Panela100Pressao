using System.Collections;
using TMPro;
using UnityEngine;

public class RopeClimb : MonoBehaviour {
    [SerializeField] private float climbSpeed = 5f;

    private Rigidbody rb;
    private bool isClimbing = false;
    private Transform ropeSegment;
    private SoldierMovement soldierMovement;

    [SerializeField] private TextMeshProUGUI _ropeText;
    [SerializeField] private CanvasGroup _ropeTextCanvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine fadeCoroutine;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        soldierMovement = GetComponent<SoldierMovement>();
        _ropeText.text = "espaço para sair da corda";
        _ropeTextCanvasGroup.alpha = 0f;
    }

    public void EnterRope(Transform segment) {
        ropeSegment = segment;
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        soldierMovement.SetMovementEnabled(false);

        ShowRopeText();
    }

    public void ExitRope() {
        isClimbing = false;
        ropeSegment = null;
        rb.useGravity = true;

        soldierMovement.SetMovementEnabled(true);

        HideRopeText();
    }

    private void Update() {
        if (!isClimbing) return;

        if (Input.GetKeyDown(KeyCode.Space)) {
            ExitRope();
            return;
        }

        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = Vector3.up * vertical * climbSpeed;
        rb.linearVelocity = move;

        Vector3 alignedPos = new Vector3(ropeSegment.position.x, transform.position.y, ropeSegment.position.z);
        transform.position = Vector3.Lerp(transform.position, alignedPos, Time.deltaTime * 10f);
    }

    private void FixedUpdate() {
        if (!isClimbing && rb.useGravity == false) {
            rb.useGravity = true;
        }
    }

    private void ShowRopeText() {
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(_ropeTextCanvasGroup, _ropeTextCanvasGroup.alpha, 1f, fadeDuration));
    }

    private void HideRopeText() {
        fadeCoroutine = StartCoroutine(FadeCanvasGroup(_ropeTextCanvasGroup, _ropeTextCanvasGroup.alpha, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration) {
        float elapsed = 0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }
}
