using UnityEngine;

public class ShieldWeapon : MonoBehaviour {
    public WeaponData weaponData;
    void PlaceShield() {
        // 구현 예정
    }
    // void PlaceShield() {
    //     for (int i = 0; i < count; i++ ) {
    //         Transform bullet;
    //         if (i < transform.childCount) { // 이미 있는거는 레벨업시 위치만 이동
    //             bullet = transform.GetChild(i);
    //         } 
    //         else { // 새로 생긴거는 새로 풀링하고 위치 이동
    //             bullet = .instance.poolManager.Get(prefabId).transform;
    //             bullet.parent = transform;
    //         }
    //         bullet.localPosition = Vector3.zero;
    //         bullet.localRotation = Quaternion.identity;

    //         Vector3 rotVec = Vector3.forward * (360 / count) * i;
    //         bullet.Rotate(rotVec);
    //         bullet.Translate(bullet.up * 1.5f, Space.World);
    //         bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);

    //     }
    // }
}