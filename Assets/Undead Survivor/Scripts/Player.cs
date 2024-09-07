using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Basic Variables")]
    public Vector2 inputVec;
    public float speed = 5;
    public Scanner scanner;
    public bool isLive = true;
    int health;
    [Header("UI")]
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();    
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        speed = 5;
        health = DataManager.instance.playerData.maxHealth;
    }

    void FixedUpdate() {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }
    void OnMove(InputValue value) {
        inputVec = value.Get<Vector2>();
    }
    void LateUpdate() {
        animator.SetFloat("Speed", inputVec.magnitude);
        //Sprite Flip
        if (inputVec.x > 0) {
            spriteRenderer.flipX = false;
        } else if (inputVec.x < 0) {
            spriteRenderer.flipX = true;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;
        Enemy enemy = collision.GetComponent<Enemy>();
        health -= enemy.damage;
        //StartCoroutine(KnockBack(3));
        if (health > 0) {
        }
        else {
            isLive = false;
        }
    }
}
