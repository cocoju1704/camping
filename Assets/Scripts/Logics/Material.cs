using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    [Header("#Basic Info")]
    public int ingredientId;
    public string ingredientName;
    public string ingredientDesc;
    public int ingredientTier;
    
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
        IngredientData data = DataManager.instance.materialDataDict[id];
        ingredientId = data.id;
        ingredientName = data.itemName;
        ingredientDesc = data.desc;
        ingredientTier = data.tier;
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
