using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SoldierSelectorData {
    [Header("Buttons")]
    public Button captainButton;
    public Button sublieutenantButton;
    public Button sargeantButton;
    public Button cadetButton;

    [Header("Roulette")]
    public RectTransform choiceRoulette;
    public CanvasGroup canvasGroupPanel;
    public float choiceRouletteScale = 1.5f;
    public float rouletteGrowDuration = 0.25f;

    [NonSerialized] public List<Image> buttonPortraits;
    [NonSerialized] public List<Button> buttons;
    [NonSerialized] public List<SoldierType> types;

    // Ângulos fixos para cada botão na roleta
    public readonly float[] buttonAngles = { 0f, 90f, 180f, 270f };

    public readonly Dictionary<SoldierType, float> soldierAngles = new() {
    { SoldierType.Captain, 0f },
    { SoldierType.Sublieutenant, 90f },
    { SoldierType.Sargeant, -90f },
    { SoldierType.Cadet, 180f }
};


    public void Initialize() {
        buttons = new List<Button> { captainButton, sublieutenantButton, sargeantButton, cadetButton };
        types = new List<SoldierType> { SoldierType.Captain, SoldierType.Sublieutenant, SoldierType.Sargeant, SoldierType.Cadet };

        buttonPortraits = new List<Image> {
            captainButton.GetComponent<Image>(),
            sublieutenantButton.GetComponent<Image>(),
            sargeantButton.GetComponent<Image>(),
            cadetButton.GetComponent<Image>()
        };
    }
}
