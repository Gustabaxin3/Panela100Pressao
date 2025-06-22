// ColorCell.cs
using UnityEngine;
using UnityEngine.UI;
using AUDIO;

public class ColorCell : MonoBehaviour {
    [SerializeField] private Image image;

    private int currentColorIndex = 0;
    private static readonly Color[] colorCycle = {
        new Color(0.2f, 0.2f, 0.2f),  // Cinza (desligado)
        new Color(0.3f, 0.8f, 0.4f),  // Verde
        new Color(0.4f, 0.6f, 1.0f),  // Azul
        new Color(0.9f, 0.3f, 0.3f)   // Vermelho
    };

    private void Start() {
        UpdateColor();
    }

    public void OnClick() {

        AudioManager.Instance.PlaySoundEffect("Audio/UI/MiniGame-MudaCor", spatialBlend: 0);
        
        int newIndex;
        do {
            newIndex = Random.Range(0, colorCycle.Length);
        } while (newIndex == currentColorIndex && colorCycle.Length > 1);
        currentColorIndex = newIndex;
        UpdateColor();
        ColorGridManager.Instance.CheckVictory();
    }

    private void UpdateColor() => image.color = colorCycle[currentColorIndex];

    public int GetColorIndex() => currentColorIndex;

    public void ResetColor() {
        currentColorIndex = 0;
        UpdateColor();
    }

    public void SetColorIndex(int index) {
        currentColorIndex = index % colorCycle.Length;
        UpdateColor();
    }
}
