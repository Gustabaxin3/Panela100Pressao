// ColorGridManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ColorGridManager : MonoBehaviour {
    public static ColorGridManager Instance { get; private set; }

    [Header("Grid Setup")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private int rows = 3;
    [SerializeField] private int cols = 3;

    [Header("UI")]
    [SerializeField] private TMP_Text feedbackText;

    private ColorCell[] cells;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    private void Start() {
        ConfigureGridLayout();
        GenerateGrid();
        RandomizeColors();
    }

    private void ConfigureGridLayout() {
        GridLayoutGroup layout = gridParent.GetComponent<GridLayoutGroup>();
        if (layout != null) {
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = cols;
            layout.cellSize = new Vector2(100, 100); // Ajuste conforme o design desejado
            layout.spacing = new Vector2(5, 5);
            layout.padding = new RectOffset(5, 5, 5, 5);
        }
    }

    private void GenerateGrid() {
        foreach (Transform child in gridParent) {
            Destroy(child.gameObject);
        }

        cells = new ColorCell[rows * cols];

        for (int i = 0; i < rows * cols; i++) {
            GameObject go = Instantiate(cellPrefab, gridParent);
            ColorCell cell = go.GetComponent<ColorCell>();
            cells[i] = cell;

            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(cell.OnClick);
        }
    }

    private void RandomizeColors() {
        int totalCells = cells.Length;
        int maxPerColor = totalCells / 2;
        int colorCount = 4;

        Dictionary<int, int> colorUsage = new Dictionary<int, int>();
        for (int i = 0; i < colorCount; i++) colorUsage[i] = 0;

        System.Random rand = new System.Random();

        foreach (var cell in cells) {
            List<int> validIndices = new List<int>();
            for (int i = 0; i < colorCount; i++) {
                if (colorUsage[i] < maxPerColor) {
                    validIndices.Add(i);
                }
            }

            int selected = validIndices[rand.Next(validIndices.Count)];
            colorUsage[selected]++;
            cell.SetColorIndex(selected);
        }
    }

    public void CheckVictory() {
        if (cells.Length == 0) return;

        int target = cells[0].GetColorIndex();
        foreach (var cell in cells) {
            if (cell.GetColorIndex() != target) {
                return;
            }
        }

        feedbackText.text = "Você venceu!";
        feedbackText.color = Color.green;
    }

    public void ResetGrid() {
        foreach (var cell in cells) {
            cell.ResetColor();
        }
        feedbackText.text = "";
    }
}
