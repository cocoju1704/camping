using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum HoverType { FurnitureUpgrade, WeaponUpgrade, VehicleUpgrade }
    public HoverType hoverType;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponentInParent<WeaponUpgradePopup>().ShowWeaponIngredientPopup();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponentInParent<WeaponUpgradePopup>().HideWeaponIngredientPopup();
    }
}
