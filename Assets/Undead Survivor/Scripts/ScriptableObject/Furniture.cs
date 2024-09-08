using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Image = UnityEngine.UI.Image;
public class Furniture : MonoBehaviour {
    public FurnitureData furnitureData;
    public int level;
    SpriteRenderer icon;
    public Crafting crafting;
    BoxCollider2D collider;
    FurniturePopup popup;
    public void Init(FurnitureData data) {
        this.furnitureData = data;
        icon.sprite = data.icon;
        level = data.level;
        // 콜라이더 크기 재조정
        collider.size = furnitureData.size;
    }
    void Awake() {
        icon = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        popup = FindObjectOfType<FurniturePopup>(true);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            Debug.Log("Furniture");
            popup.gameObject.SetActive(true);
            popup.Set(furnitureData, level);
        }
    }
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            popup.Reset();
            popup.gameObject.SetActive(false);
        }
    }
    void FurnitureLevelUp() {
        switch(furnitureData.furnitureType) {
            case FurnitureData.FurnitureType.Crafting:
                if (level == 0) {
                    GameObject newCrafting = new GameObject();
                    crafting = newCrafting.AddComponent<Crafting>();
                }
                level++;
                break;
        }
    }
}