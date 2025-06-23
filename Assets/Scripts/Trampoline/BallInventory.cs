using System.Collections.Generic;
using UnityEngine;

public class BallInventory : MonoBehaviour {
    public static BallInventory Instance { get; private set; }

    [SerializeField] private List<GameObject> _balls = new List<GameObject>();
    private const int TotalBalls = 5;
    private int contagem = 0;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddBall(GameObject ball) {
        _balls.Add(ball);
        ball.SetActive(false);
        UpdateMissionFeedback();
    }

    public List<GameObject> DeliverBalls() {
        var delivered = new List<GameObject>(_balls);
        contagem++;

        foreach (var ball in delivered) {
            if (ball != null) {
                Destroy(ball);
            }
        }

        _balls.Clear();
        UpdateMissionFeedback();
        return delivered;
    }

    private void UpdateMissionFeedback() {
        MissionFeedbackUI.ShowFeedback($"Bolinhas restantes: {contagem}/{TotalBalls}");
    }
}
