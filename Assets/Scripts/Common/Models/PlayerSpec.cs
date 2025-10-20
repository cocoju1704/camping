using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSpec { //플레이어 기본 정보 묶음
    public float health;
    public int maxHealth;
    public float speed;
    public PlayerSpec() {
        health = 100;
        maxHealth = 100;
        speed = 5;
    }
    public PlayerSpec(float health, int maxHealth, float speed) {
        this.health = health;
        this.maxHealth = maxHealth;
        this.speed = speed;
    }
}