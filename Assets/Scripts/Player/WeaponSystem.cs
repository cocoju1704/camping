using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.InputSystem;
public class WeaponSystem : MonoBehaviour, ISavable
{
    public Hand hand;
    public List<Weapon> weapons; // 0: primary, 1: secondary
    public Weapon currentWeapon;
    public Scanner sc;
    public Vector3 targetPos;
    public TMP_Text weaponName;
    public UnityEvent onWeaponLevelUp = new UnityEvent();
    public int currentWeaponIndex = 0; // 현재 선택된 무기 인덱스
    private Coroutine showWeaponNameCoroutine;

    void Awake()
    {
        sc = GetComponent<Scanner>();
        weapons = new List<Weapon>(GetComponentsInChildren<Weapon>());
        currentWeapon = weapons[0]; // 첫 번째 무기를 기본으로 선택
        targetPos = Vector3.zero;
        onWeaponLevelUp.AddListener(UpdateWeaponStats);
    }
    void Start() {
        InitWeapons();

    }
    void InitWeapons()
    {
        DebugWeapons();
        SwitchWeapon(currentWeaponIndex); // 첫 번째 무기 선택
    }

    void DebugWeapons()
    {
        // 테스트용 무기 추가
        ObtainWeapon(1, 0);
        ObtainWeapon(101, 0);
        ObtainWeapon(201, 0);
        ObtainWeapon(202, 0);
        ObtainWeapon(301, 0);
        ObtainWeapon(401, 0);
        ObtainWeapon(302, 0);
        SetPrimaryWeapon(302);
        SetSecondaryWeapon(1);
    }

    void Update()
    {
        targetPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        targetPos.z = 0;
        RotateHand(targetPos);

        if (Keyboard.current.spaceKey.wasPressedThisFrame) SwitchWeapon();
        if (Keyboard.current.rKey.wasPressedThisFrame) {
            if (!currentWeapon.isMelee && currentWeapon.canClick) {
                StartCoroutine(currentWeapon.Reload());
            }
        }
    }



    public void DisableHands()
    {
        hand.gameObject.SetActive(false);
        hand.enabled = false;
        foreach (var weapon in weapons)
        {
            weapon.enabled = false;
        }
    }

    public void EnableHands()
    {
        hand.gameObject.SetActive(true);
        hand.enabled = true;
        Debug.Log(weapons[0].weaponData);
        SetPrimaryWeapon(weapons[0].weaponData);
        SetSecondaryWeapon(weapons[1].weaponData);
        RefreshCurrentWeapon();
    }
    void RotateHand(Vector3 pos)
    {
        bool flipX = GetComponent<SpriteRenderer>().flipX;
        hand.GetComponent<SpriteRenderer>().flipX = flipX;

        Vector3 axisPos = transform.position + new Vector3(-0.18f, -0.28f, 0);
        Vector3 offset = new Vector3(0.2f, 0f, 0);
        float distance = offset.magnitude;

        Vector2 direction = pos - axisPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (flipX) angle += 180;
        hand.transform.position = axisPos + (Quaternion.Euler(0, 0, angle) * Vector3.right * distance * (flipX ? -1 : 1));
        hand.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    // 무기 관련 함수들
    public void SetPrimaryWeapon(int id)
    {
        SetWeapon(0, DataManager.instance.weaponDataDict[id]);
    }
    public void SetPrimaryWeapon(WeaponData weaponData) {
        SetWeapon(0, weaponData);
    }
    public void SetSecondaryWeapon(int id)
    {
        SetWeapon(1, DataManager.instance.weaponDataDict[id]);
    }
    public void SetSecondaryWeapon(WeaponData weaponData) {
        SetWeapon(1, weaponData);
    }
    private void SetWeapon(int index, WeaponData weaponData)
    {
        weapons[index].Init(weaponData);
        if (index == currentWeaponIndex) hand.UpdateVisual(weaponData.icon);
    }

    public void SwitchWeapon()
    {
        currentWeaponIndex = 1 - currentWeaponIndex; // 무기 인덱스 토글 (0, 1 번 무기 교체)
        SwitchWeapon(currentWeaponIndex);
    }

    private void SwitchWeapon(int index)
    {
        if (!currentWeapon.canClick) return;

        currentWeapon = weapons[index];

        foreach (var weapon in weapons)
        {
            weapon.enabled = false;
        }

        weapons[index].enabled = true;
        hand.UpdateVisual(currentWeapon.weaponData.icon);
        ShowWeaponNameTrigger();
    }
    public void RefreshCurrentWeapon() {
        SetWeapon(currentWeaponIndex, currentWeapon.weaponData);
        weapons[currentWeaponIndex].enabled = true;
    }
    void ObtainWeapon(int id, int lv)
    {
        // 이미 소유한 무기인지 확인
        foreach (var weapon in GameManager.instance.obtainedWeapons) {
            if (weapon.Item1.id == id) return;
        }
        WeaponData weaponData = DataManager.instance.weaponDataDict[id];
        GameManager.instance.obtainedWeapons.Add((weaponData, lv));
    }

    public void LevelUpWeapon(int idx)
    {
        GameManager.instance.obtainedWeapons[idx] = (GameManager.instance.obtainedWeapons[idx].Item1, GameManager.instance.obtainedWeapons[idx].Item2 + 1);
        onWeaponLevelUp.Invoke();
    }

    private void UpdateWeaponStats()
    {
        foreach (var weapon in weapons)
        {
            weapon.SetSpec();
        }
    }

    public void Save(GameData data)
    {
        List<KeyValuePair<int, int>> obtainedWeaponLvs = new List<KeyValuePair<int, int>>();
        foreach (var weapon in GameManager.instance.obtainedWeapons)
        {
            obtainedWeaponLvs.Add(new KeyValuePair<int, int>(weapon.Item1.id, weapon.Item2));
        }
        data.obtainedWeaponLvs = obtainedWeaponLvs;
        data.playerSelectedWeapon[0] = weapons[0].weaponData.id;
        data.playerSelectedWeapon[1] = weapons[1].weaponData.id;
    }

    public void Load(GameData data)
    {
        GameManager.instance.obtainedWeapons.Clear();
        foreach (var pair in data.obtainedWeaponLvs)
        {
            ObtainWeapon(pair.Key, pair.Value);
        }
        SetPrimaryWeapon(data.playerSelectedWeapon[0]);
        SetSecondaryWeapon(data.playerSelectedWeapon[1]);
    }

    // 무기 이름 표시 관련 함수
    public void ShowWeaponNameTrigger()
    {
        if (showWeaponNameCoroutine != null) StopCoroutine(showWeaponNameCoroutine);
        weaponName.DOKill();
        showWeaponNameCoroutine = StartCoroutine(ShowWeaponName());
    }

    IEnumerator ShowWeaponName()
    {
        weaponName.text = currentWeapon.weaponData.itemName + "+" + currentWeapon.level;
        weaponName.alpha = 1f;

        weaponName.DOFade(0, 0.5f).SetDelay(1f).OnComplete(() => weaponName.alpha = 0f);
        yield return new WaitForFixedUpdate();

        showWeaponNameCoroutine = null;
    }
}