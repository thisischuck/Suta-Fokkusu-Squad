using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct Info
{
    public string name;
    public int gold;
    public Color c1;
    public Color c2;
    public Color c3;
    public List<int> unlocked;
    public int[] current;
    public int stage;
    public int volume;
    public float sens;
    public int crosshair;
    public Sprite currentCrosshair;

    public Info(string name, int gold, Color c1, Color c2, Color c3, List<int> unlocked, int[] current, int stage, int volume, float sens, int crosshair, Sprite currentCrosshair)
    {
        this.name = name;
        this.gold = gold;
        this.c1 = c1;
        this.c2 = c2;
        this.c3 = c3;
        this.unlocked = unlocked;
        this.current = current;
        this.stage = stage;
        this.volume = volume;
        this.sens = sens;
        this.crosshair = crosshair;
        this.currentCrosshair = currentCrosshair;
    }
}

public static class PlayerStatsInfo
{
    public static string name = "player";
    public static int gold = 5000;
    public static Color shipColor1;
    public static Color shipColor2;
    public static Color shipColor3;
    public static List<WeaponObject> allWeapons = new List<WeaponObject>();
    public static List<WeaponObject> unlockedWeapons = new List<WeaponObject>();
    public static WeaponObject[] currentWeapons = new WeaponObject[3];
    public static int selectedWeapon = 0;
    public static int stage = 2;

    public static int volume = 100;
    public static float sens = 50;
    public static int crosshairNumber = 0;
    public static Sprite currentCrosshair = null;

    private static WeaponObject[] IntArrayToWeaponObjectArray(int[] ar)
    {
        WeaponObject[] cur = new WeaponObject[ar.Length];
        int cont = 0;
        foreach (int i in ar)
        {
            foreach (WeaponObject w in allWeapons)
            {
                if (w.id == i)
                {
                    cur[cont] = w;
                    cont++;
                }

            }
        }

        return cur;
    }

    private static List<WeaponObject> IntListToWeaponObjectList(List<int> ar)
    {
        List<WeaponObject> cur = new List<WeaponObject>();
        foreach (int i in ar)
        {
            foreach (WeaponObject w in allWeapons)
            {
                if (w.id == i)
                {
                    cur.Add(w);
                }

            }
        }

        return cur;
    }

    private static List<int> WeaponObjectListToIntList(List<WeaponObject> ar)
    {
        List<int> cur = new List<int>();

        foreach (WeaponObject w in ar)
        {
            cur.Add(w.id);
        }
        return cur;
    }

    private static int[] WeaponObjectArrayToIntArray(WeaponObject[] ar)
    {
        int[] cur = new int[ar.Length];
        int cont = 0;

        foreach (WeaponObject w in ar)
        {
            cur[cont] = w.id;
            cont++;
        }

        return cur;
    }

    public static void FindAllWeapons(List<WeaponObject> w)
    {
        allWeapons = w;
    }

    public static void ResetInfo()
    {
        name = "player";
        gold = 5000;
        shipColor1 = Color.black;
        shipColor2 = Color.black;
        shipColor3 = Color.black;
        unlockedWeapons.Clear();
        currentWeapons[0] = null;
        currentWeapons[1] = null;
        currentWeapons[2] = null;
        stage = 2;
    }

    public static void SaveProgress()
    {
        Info info = new Info(name, gold, shipColor1, shipColor2, shipColor3, WeaponObjectListToIntList(unlockedWeapons), WeaponObjectArrayToIntArray(currentWeapons), 
            stage, volume, sens, crosshairNumber, currentCrosshair);
        JsonCharacterSaver.SaveCharacter(info, Path.Combine(Application.persistentDataPath, "Data.txt"));
    }

    public static bool LoadProgress()
    {
        Info info = JsonCharacterSaver.LoadCharacter(Path.Combine(Application.persistentDataPath, "Data.txt"));
        if (info.name == null)
        {
            ResetInfo();
            return false;
        }
        name = info.name;
        gold = info.gold;
        shipColor1 = info.c1;
        shipColor2 = info.c2;
        shipColor3 = info.c3;
        unlockedWeapons = IntListToWeaponObjectList(info.unlocked);
        currentWeapons = IntArrayToWeaponObjectArray(info.current);
        stage = info.stage;
        volume = info.volume;
        sens = info.sens;
        crosshairNumber = info.crosshair;
        currentCrosshair = info.currentCrosshair;
        return true;
    }
}

public static class JsonCharacterSaver
{
    public static Info info;
    public static string dataPath = Path.Combine(Application.persistentDataPath, "Data.txt");

    public static void SaveCharacter(Info data, string path)
    {
        string jsonString = JsonUtility.ToJson(data);

        using (StreamWriter streamWriter = File.CreateText(path))
        {
            streamWriter.Write(jsonString);
        }
    }

    public static Info LoadCharacter(string path)
    {
        Info i = new Info();
        if (!File.Exists(path))
            return i;
        using (StreamReader streamReader = File.OpenText(path))
        {
            if (streamReader == null)
                return i;
            string jsonString = streamReader.ReadToEnd();
            return JsonUtility.FromJson<Info>(jsonString);
        }
    }
}
