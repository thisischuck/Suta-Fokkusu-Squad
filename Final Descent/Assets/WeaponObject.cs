using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Laser, Projectile};

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class WeaponObject : ScriptableObject {

    public int id;
    public new string name;
    public string description;
    public GameObject weaponModel;
    public WeaponType weaponType;

    public Sprite sprite;

    public int price;
    public int level;
}
