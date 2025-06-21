using UnityEngine;

public class BallInventory : MonoBehaviour {
    public static BallInventory Instance { get; private set; }

    private int _ballCount = 0;
    public int BallCount => _ballCount;

    [SerializeField] private GameObject[] launcherBalls;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddBall() {
        _ballCount++;
        Debug.Log($"Bolinhas coletadas: {_ballCount}");
        UpdateLauncherBalls();
    }

    public int DeliverBalls() {
        int delivered = _ballCount;
        _ballCount = 0;
        UpdateLauncherBalls();
        return delivered;
    }

    private void UpdateLauncherBalls() {
        if (launcherBalls == null) return;
        for (int i = 0; i < launcherBalls.Length; i++) {
            launcherBalls[i].SetActive(i < _ballCount);
        }
    }
}
