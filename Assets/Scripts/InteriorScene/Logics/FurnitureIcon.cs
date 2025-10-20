using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class FurnitureIcon : MonoBehaviour{ // 기본 가구 클래스
    public FurnitureData data;
    SpriteRenderer icon;
    BoxCollider2D collider;
    InteriorSystem interiorSystem;
    void Awake()
    {
        icon = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        interiorSystem = FindObjectOfType<InteriorSystem>();
    }
    public void Init(FurnitureData data) {
        this.data = data;
        icon.sprite = data.icon;
        gameObject.name = data.itemName;
    }
    void OnTriggerEnter2D(Collider2D other) { // 가구에 플레이어 접촉시 해당 가구의 상호작용 팝업 띄우기
        if (other.CompareTag("Player") || !other.isTrigger)
        {
            interiorSystem.ShowFurniturePopup(data.id);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player") || !other.isTrigger) {
            interiorSystem.HideFurniturePopup();
        }
    }
}