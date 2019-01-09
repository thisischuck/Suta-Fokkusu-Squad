using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicHud : MonoBehaviour
{
    public Transform player;
    public Transform ship;
    private Camera cam;
    public float speed;

    private float count = 10;
    private float outOfCombatTimer = 5;
    private bool recentlyInCombat;
    public TMP_Text txtWeaponName;
    public Image weaponSprite;
    public TMP_Text txtEnemyName;
    public GameObject enemyHp;
    public GameObject enemyInfo;

    public GameObject crossHair;
    
    private string enemy_Name;
    private float enemy_maxHp, enemy_currentHp;

    public bool isOnline = false;

    // Use this for initialization
    void Start()
    {
        if (isOnline == false)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            ship = GameObject.FindGameObjectWithTag("Ship").transform;
        }
        cam = Camera.main;
    }

    private void Update()
    {
        ControlWeapon();
        ControlEnemyHud();
        if (isOnline)
            FindObjects();
    }

    private void FindObjects()
    {
        player = GameObject.Find("localPlayer").transform;
        ship = GameObject.Find("localShip").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ship != null)
        {
            if (Vector3.Distance(transform.position, ship.position) >= 1)
            {
                Vector2 screenPos = Vector3.zero;
                screenPos = cam.WorldToScreenPoint(ship.position);

                float step = speed * Time.deltaTime;
                Vector3 l = Vector3.Lerp(transform.position, screenPos, step);
                transform.position = l;
            }
        }

        if (!isOnline)
            crossHair.transform.position = Input.mousePosition;

    }

    private void ControlWeapon()
    {
        if (PlayerStatsInfo.currentWeapons[PlayerStatsInfo.selectedWeapon] != null)
        {
            txtWeaponName.text = PlayerStatsInfo.currentWeapons[PlayerStatsInfo.selectedWeapon].name;
            weaponSprite.sprite = PlayerStatsInfo.currentWeapons[PlayerStatsInfo.selectedWeapon].sprite;
        }
    }

    public void SetEnemyStats(string enemyName, float enemyMaxHealth, float enemyCurrentHealth)
    {
        count = 0;
        enemy_Name = enemyName;
        enemy_maxHp = enemyMaxHealth;
        enemy_currentHp = enemyCurrentHealth;
    }

    void ControlEnemyHud()
    {
        if (recentlyInCombat)
        {
            if (count < outOfCombatTimer)
            {
                count += Time.deltaTime;
            }

            if (count >= outOfCombatTimer)
            {
                recentlyInCombat = false;
            }
            enemyInfo.SetActive(true);

            ControlEnemyStats();
        }
        else
        {
            enemyInfo.SetActive(false);
        }

        if (count == 0)
            recentlyInCombat = true;

        if (enemy_currentHp == 0)
        {
            recentlyInCombat = false;
            count = enemy_maxHp;
            enemyInfo.SetActive(false);
        }
    }

    private void ControlEnemyStats()
    {
        txtEnemyName.text = enemy_Name;
        txtEnemyName.color = Color.red;
        enemyHp.GetComponent<HealthBar>().SetMaxHealth(enemy_maxHp);
        enemyHp.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(enemyHp.GetComponent<HealthBar>().currentAmout, enemy_currentHp, 5f * Time.deltaTime);
    }
}
