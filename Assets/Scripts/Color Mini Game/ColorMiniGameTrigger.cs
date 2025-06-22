using UnityEngine;

public class ColorMiniGameTrigger : MonoBehaviour {
    [SerializeField] private int _customRows = 3;
    [SerializeField] private int _customCols = 3;
    [SerializeField] private GameObject interactionHintUI;

    private bool _playerInRange = false;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Sargeant")) {
            _playerInRange = true;
            InteractionHintUI.Instance.ShowHint("Pressione E para iniciar o minigame!", Color.white);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Sargeant")) {
            _playerInRange = false;
            InteractionHintUI.Instance.HideHint();
        }
    }

    void Update() {
        if (_playerInRange && !ColorGridManager.Instance._gameStarted && Input.GetKeyDown(KeyCode.E)) {
            SetGridSize(_customRows, _customCols);
            ColorGridManager.Instance.StartGame();
            ColorGridManager.Instance._gameStarted = true;
        }
    }

    private void SetGridSize(int rows, int cols) {
        var manager = ColorGridManager.Instance;

        var rowsField = typeof(ColorGridManager).GetField("rows", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var colsField = typeof(ColorGridManager).GetField("cols", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        if (rowsField != null && colsField != null) {
            rowsField.SetValue(manager, rows);
            colsField.SetValue(manager, cols);
        }
    }
}
