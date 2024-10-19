using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Material : MonoBehaviour
{
    [Header("#Basic Info")]
    public int materialId;
    public string materialName;
    public string materialDesc;
    public int materialTier;
    
    [Header("#Visual Info")]
    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DisableColliderForOneSecond());
    }
    void OnEnable() {
        StartCoroutine(DisableColliderForOneSecond());
    }
    public void Init(int id, Vector3 pos) {
        MaterialData data = DataManager.instance.materialDataDict[id];
        materialId = data.id;
        materialName = data.itemName;
        materialDesc = data.desc;
        materialTier = data.tier;
        spriteRenderer.sprite = data.icon;
        transform.position = pos;
    }
    public void OnTriggerEnter2D(Collider2D collision) {
        if (!collision.CompareTag("Player")) return;
        if (collision.isTrigger) return;
        Inventory inven = StageManager.instance.inventory;
        if (inven.HasSpace()) {
            inven.Obtain(this);
            gameObject.SetActive(false);
        }
    }
    void OnTriggerExit2D(Collider2D collision) {
        if (!collision.CompareTag("Area")) return;
        gameObject.SetActive(false);
    }
    IEnumerator DisableColliderForOneSecond() {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        GetComponent<Collider2D>().enabled = true;
    }
}
