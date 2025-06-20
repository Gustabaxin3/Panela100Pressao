using UnityEngine;
using UnityEngine.EventSystems;

public class HoverRelay : MonoBehaviour, IPointerEnterHandler {
    [SerializeField] private SoldierSelectorUI _selectorUI;
    [SerializeField] private SoldierType _soldierType; 

    public void OnPointerEnter(PointerEventData eventData) {
        _selectorUI?.HandleHover(_soldierType);
    }
}

public enum SoldierType {
    Captain,
    Sublieutenant,
    Sargeant,
    Cadet
}
