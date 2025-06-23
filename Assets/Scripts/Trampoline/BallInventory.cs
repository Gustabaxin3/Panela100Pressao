using System.Collections.Generic;
using UnityEngine;

public class BallInventory : MonoBehaviour {
    public static BallInventory Instance { get; private set; }

    [SerializeField]private  List<GameObject> _balls = new List<GameObject>();

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
        Debug.Log($"Bolinhas coletadas: {_balls.Count}");
    }
    public List<GameObject> DeliverBalls() {
        var delivered = new List<GameObject>(_balls);

        foreach(var ball in delivered) {
            if (ball != null) {
                Destroy(ball);
            }
        }

        _balls.Clear();
        return delivered;
    }
}
