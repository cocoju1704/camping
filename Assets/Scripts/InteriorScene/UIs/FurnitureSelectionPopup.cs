using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureSelectionPopup : MonoBehaviour, IToggleable
{
    [Header("데이터")]
    public FurnitureData[] furnitureList;
    public int index;

    [Header("내부 오브젝트")]
    public GameObject panel;
    public Image furnitureImage;
    public TMP_Text furnitureName;
    public TMP_Text furnitureDesc;
    public IngredientPopup ingredientPopup;
    public Button toLeft;
    public Button toRight;
    public Button furintureCreate;
    public Button showHideButton;
    [Header("외부 오브젝트")]
    public InteriorManager interiorSystem;
    [Header("기타")]
    public Vector2Int playerPos;
    public void Start() {
        furnitureList = DataManager.instance.furnitureDataList;
        index = 0;
        interiorSystem = FindObjectOfType<InteriorManager>();
        toLeft.onClick.AddListener(ToLeft);
        toRight.onClick.AddListener(ToRight);
        showHideButton.onClick.AddListener(Toggle);
        furintureCreate.onClick.AddListener(CreateFurniture);
        Toggle();
    }
    void LateUpdate() {
        if (panel.activeSelf) {
            interiorSystem.ShowFurniturePreview(furnitureList[index]);
        }
    }
    public void UpdateUI() {
        furnitureImage.sprite = furnitureList[index].icon;
        furnitureName.text = furnitureList[index].itemName;
        furnitureDesc.text = furnitureList[index].desc;
        ingredientPopup.Set(furnitureList[index].levelIngredients[0].materials);
        interiorSystem.ShowFurniturePreview(furnitureList[index]);
    }
    public void ToLeft() {
        index = (index - 1 + furnitureList.Length) % furnitureList.Length;
        UpdateUI();
    }
    public void ToRight() {
        index = (index + 1) % furnitureList.Length;
        UpdateUI();
    }
    public void Toggle() {
        // 팝업 여는 경우 UpdateUI까지 호출
        panel.SetActive(!panel.activeSelf);
        if (panel.activeSelf) {
            UpdateUI();
        }
        else {
            interiorSystem.HideFurniturePreview();
        }
    }
    void CreateFurniture() {
        // 플레이어 발이 기준이라 -0.5f 해줘야함
        Vector3 playerPos = Player.instance.transform.position - new Vector3(0, 0.5f, 0);
        interiorSystem.PlaceFurniture(Utils.WorldToGrid(playerPos), furnitureList[index]);
        Toggle();
    }
}
