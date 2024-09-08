using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class Material : MonoBehaviour
{
    [Header("#Basic Info")]
    public int materialId;
    public string materialName;
    public string materialDesc;
    public int materialTier;
    

    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Init(int materialType, Vector3 pos) {
        MaterialData data = DataManager.instance.materialDatas[materialType];
        materialId = data.id;
        materialName = data.name;
        materialDesc = data.desc;
        materialTier = data.tier;
        spriteRenderer.sprite = data.icon;
        transform.position = pos;
    }
    public void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        InventorySystem inventorySystem = DataManager.instance.inventorySystem;
        if (inventorySystem.HasSpace()) {
            inventorySystem.Obtain(this);
            gameObject.SetActive(false);
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) return;
        gameObject.SetActive(false);
    }
}
