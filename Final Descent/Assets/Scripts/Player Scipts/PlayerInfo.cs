using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatsInfo {
    public static string name = "player";
    public static int gold = 900;
    public static Color shipColor1;
    public static Color shipColor2;
    public static Color shipColor3;
    public static List<WeaponObject> unlockedWeapons = new List<WeaponObject>();
    public static WeaponObject[] currentWeapons = new WeaponObject[3];
    public static int stage = 0;

    public static int volume = 100;
    public static float sens = 50;
    public static int crosshairNumber = 0;
    public static Sprite currentCrosshair = null;

}
