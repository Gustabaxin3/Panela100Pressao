using UnityEngine;

public class ColorMiniGameTrigger : MonoBehaviour {
    private bool _playerInRange = false;
    private static int maquinaCount = 0;

    [field: SerializeField]
    public bool IsCompleted { get; private set; } = false;
    private void Start() {
        ColorGridManager.Instance.OnGameCompleted += HandleGameCompleted;
        maquinaCount = 0;
    }

    private void OnDisable() {
        if (ColorGridManager.Instance != null)
            ColorGridManager.Instance.OnGameCompleted -= HandleGameCompleted;
    }
    private void Update() {
        if (_playerInRange && !IsCompleted && !ColorGridManager.Instance._gameStarted && Input.GetKeyDown(KeyCode.E)) {
            ColorGridManager.Instance.StartGame(maquinaCount, this);
            maquinaCount++;
        }
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Sargeant")) {
            _playerInRange = true;
            if (!IsCompleted)
                InteractionHintUI.Instance.ShowHint("Pressione E para iniciar o minigame!", Color.white);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Sargeant")) {
            _playerInRange = false;
            InteractionHintUI.Instance.HideHint();
        }
    }

    public void CompleteMinigame() {
        if (IsCompleted) return;

        IsCompleted = true;
        ColorGridManager.Instance.CheckAllMinigamesCompleted();
    }

    private void HandleGameCompleted() {
        _playerInRange = false;
        InteractionHintUI.Instance.HideHint();
    }
    public void DisableInteraction() {
        IsCompleted = true;

        _playerInRange = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        InteractionHintUI.Instance.HideHint();
    }
}
