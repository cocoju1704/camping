using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class Furniture : MonoBehaviour{
    public FurnitureData data;
    SpriteRenderer icon;
    BoxCollider2D collider;
    UpgradePopup popup;

    void Awake() {
        icon = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }
    public void Init(FurnitureData data) {
        this.data = data;
        icon.sprite = data.icon;
        // 콜라이더 크기 재조정
        collider.size = this.data.size;
        switch(this.data.id) {
            case 0:
                popup = FindObjectOfType<CraftingTablePopup>(true);
                break;
            case 1:
                popup = FindObjectOfType<WeaponUpgradePopup>(true);
                break;
            default:
                popup = FindObjectOfType<FurnitureUpgradePopup>(true);
                break;
        }
        GameManager.instance.SetPlayerSpec();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") || !other.isTrigger) {
            popup.Set(data);
            popup.Toggle();
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") || !other.isTrigger) {
            popup.Toggle();
        }
    }
}