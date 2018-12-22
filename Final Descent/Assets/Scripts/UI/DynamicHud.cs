using System;
using TMPro;
using UnityEngine;

public class DynamicHud : MonoBehaviour
{
    private Transform player;
    private Camera cam;
    public float speed;

    private float count = 10;
    private float outOfCombatTimer = 5;
    private bool recentlyInCombat;
    public TMP_Text txtEnemyName;
    public GameObject enemyHp;
    public GameObject enemyInfo;
    
    private string enemy_Name;
    private float enemy_maxHp, enemy_currentHp;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Ship").transform;
        cam = Camera.main;
    }

    private void Update()
    {
        ControlEnemyHud();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, player.position) >= 1)
        {
            Vector2 screenPos = cam.WorldToScreenPoint(player.position);

            float step = speed * Time.deltaTime;
            Vector3 l = Vector3.Lerp(transform.position, screenPos, step);
            transform.position = l;
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
        enemyHp.GetComponent<HealthBar>().SetMaxHealth(enemy_maxHp);
        enemyHp.GetComponent<HealthBar>().currentAmout = Mathf.Lerp(enemyHp.GetComponent<HealthBar>().currentAmout, enemy_currentHp, 5f * Time.deltaTime);
    }
}
