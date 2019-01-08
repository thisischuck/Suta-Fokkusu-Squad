using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOutMenu_Controller : MonoBehaviour {
    public GameObject loadoutMenuUI;
    public GameObject choosingWeaponUI;
    public GameObject plane;
    public bool isOnline = false;

    //LoadOut
    [Header("General Stats")]
    public TMP_Text nextStage;
    public TMP_Text currentGold;
    public Button nextStageButton, mainMenuButton;

    [Header("Current Weapons")]
    public Button weapon1;
    public Button weapon2;
    public Button weapon3;
    [Tooltip("The defaut no weapon sprite")]
    public Sprite buttonDefaultSprite; //current weapons "no weapon" sprite
    [Space]
    public int weaponChange = -1;
    public GameObject colorPicker;
    public int colorChannel = 0;
    public Button btnColor1, btnColor2, btnColor3;
    private Color color1, color2, color3;

    [Header("Weapon selection stuff")]
    public WeaponObject selectedWeapon;
    public TMP_Text selectedWeaponDesc;
    public TMP_Text selectedWeaponName;
    public TMP_Text selectedWeaponPrice;
    public TMP_Text currentGoldInWeaponSelection;
    public Button buyButton, selectButton;

    //public WeaponObject[] weaponList;

	// Use this for initialization
	void Start () {
        //SweaponList = new WeaponObject[24];
        nextStageButton.onClick.AddListener(() => NextScene());
        mainMenuButton.onClick.AddListener(() => LoadMenu());
        weapon1.onClick.AddListener(() => ChangeWeapon(0));
        weapon2.onClick.AddListener(() => ChangeWeapon(1));
        weapon3.onClick.AddListener(() => ChangeWeapon(2));
        btnColor1.onClick.AddListener(() => ChangeColorChanel(0));
        btnColor2.onClick.AddListener(() => ChangeColorChanel(1));
        btnColor3.onClick.AddListener(() => ChangeColorChanel(2));
        buyButton.onClick.AddListener(() => BuyButtonClicked());
        selectButton.onClick.AddListener(() => SelectButtonClicked());
    }
	
	// Update is called once per frame
	void Update ()
    {
        nextStage.text = "Next Stage: " + (PlayerStatsInfo.stage - 1).ToString();
        currentGold.text = "Gold: " + PlayerStatsInfo.gold;
        currentGoldInWeaponSelection.text = "CURRENT GOLD: " + PlayerStatsInfo.gold;

        UpdateButton(weapon1, 0);
        UpdateButton(weapon2, 1);
        UpdateButton(weapon3, 2);
        UpdateChoosingWeaponButtons();
        ColorSelection();
    }

    public void GiveWeaponNames(out string w1, out string w2, out string w3)
    {
        w1 = "";
        w2 = "";
        w3 = "";
        if (PlayerStatsInfo.currentWeapons[0] != null)
            w1 = PlayerStatsInfo.currentWeapons[0].name;
        if (PlayerStatsInfo.currentWeapons[1] != null)
            w2 = PlayerStatsInfo.currentWeapons[1].name;
        if (PlayerStatsInfo.currentWeapons[2] != null)
            w3 = PlayerStatsInfo.currentWeapons[2].name;
    }

    public void GiveColors(out Color c1, out Color c2, out Color c3)
    {
        c1 = color1;
        c2 = color2;
        c3 = color3;
        btnColor1.interactable = false;
        btnColor2.interactable = false;
        btnColor3.interactable = false;
    }

    public void GiveWeapons(out GameObject w1, out GameObject w2, out GameObject w3)
    {
        w1 = null;
        w2 = null;
        w3 = null;
        if (PlayerStatsInfo.currentWeapons[0] != null)
            w1 = PlayerStatsInfo.currentWeapons[0].weaponModel;
        if (PlayerStatsInfo.currentWeapons[1] != null)
            w2 = PlayerStatsInfo.currentWeapons[1].weaponModel;
        if (PlayerStatsInfo.currentWeapons[2] != null)
            w3 = PlayerStatsInfo.currentWeapons[2].weaponModel;
        weapon1.interactable = false;
        weapon2.interactable = false;
        weapon3.interactable = false;
    }

    private void ColorSelection()
    {
        switch (colorChannel)
        {
            case 0:
                color1 = colorPicker.GetComponent<CUIColorPicker>().Color;
                btnColor1.GetComponent<Image>().color = color1;
                break;
            case 1:
                color2 = colorPicker.GetComponent<CUIColorPicker>().Color;
                btnColor2.GetComponent<Image>().color = color2;
                break;
            case 2:
                color3 = colorPicker.GetComponent<CUIColorPicker>().Color;
                btnColor3.GetComponent<Image>().color = color3;
                break;
        }
        PlayerStatsInfo.shipColor1 = color1;
        PlayerStatsInfo.shipColor2 = color2;
        PlayerStatsInfo.shipColor3 = color3;

        plane.GetComponent<DynamicTexture>().ColorShip1 = PlayerStatsInfo.shipColor1;
        plane.GetComponent<DynamicTexture>().ColorShip2 = PlayerStatsInfo.shipColor2;
        plane.GetComponent<DynamicTexture>().ColorShip3 = PlayerStatsInfo.shipColor3;
    }

    private void ChangeColorChanel(int button)
    {
        colorChannel = button;
    }

    private void UpdateChoosingWeaponButtons()
    {
        if (selectedWeapon == null)
        {
            selectButton.interactable = false;
            buyButton.interactable = false;
        }
        else
        {
            if (!FindWeapon())
            {
                selectButton.interactable = false;
            }
            else
            {
                selectButton.interactable = true;
            }

            if (FindWeapon())
            {
                buyButton.interactable = false;
            }
            else
            {
                if (selectedWeapon.price > PlayerStatsInfo.gold)
                {
                    buyButton.interactable = false;
                }
                else
                {
                    buyButton.interactable = true;
                }
            }
        }
    }

    private void ChangeWeapon(int weapon)
    {
        weaponChange = weapon;
    }

    private void UpdateButton(Button b, int i)
    {
        if (PlayerStatsInfo.currentWeapons[i] != null)
        {
            b.GetComponent<Image>().sprite = PlayerStatsInfo.currentWeapons[i].sprite;
            b.GetComponentInChildren<TMP_Text>().text = "";//PlayerInfo.currentWeapons[i].name;
        }
        else
        {
            b.GetComponent<Image>().sprite = buttonDefaultSprite;
            b.GetComponentInChildren<TMP_Text>().text = "EMPTY WEAPON SLOT";//PlayerInfo.currentWeapons[i].name;
        }
    }

    private bool FindWeapon()
    {
        foreach (WeaponObject w in PlayerStatsInfo.unlockedWeapons)
        {
            if (w.name == selectedWeapon.name)
            {
                return true;
            }
        }
        return false;
    }

    public void LoadOutMenu()
    {
        loadoutMenuUI.SetActive(false);
        loadoutMenuUI.GetComponent<CanvasGroup>().alpha = 0.15f;
        loadoutMenuUI.GetComponent<CanvasGroup>().interactable = false;
        loadoutMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        plane.SetActive(false);
        colorPicker.SetActive(false);

        choosingWeaponUI.SetActive(true);
        choosingWeaponUI.GetComponent<CanvasGroup>().alpha = 1f;
        choosingWeaponUI.GetComponent<CanvasGroup>().interactable = true;
        choosingWeaponUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (isOnline)
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public void LoadOutMenuBack()
    {
        loadoutMenuUI.SetActive(true);
        loadoutMenuUI.GetComponent<CanvasGroup>().alpha = 1f;
        loadoutMenuUI.GetComponent<CanvasGroup>().interactable = true;
        loadoutMenuUI.GetComponent<CanvasGroup>().blocksRaycasts = true;
        plane.SetActive(true);
        colorPicker.SetActive(true);

        choosingWeaponUI.SetActive(false);
        choosingWeaponUI.GetComponent<CanvasGroup>().interactable = false;
        choosingWeaponUI.GetComponent<CanvasGroup>().blocksRaycasts = false;
        choosingWeaponUI.GetComponent<CanvasGroup>().alpha = 0f;
        selectedWeapon = null;
        weaponChange = -1;
        selectedWeaponDesc.text = "DESCRIPTION: ";
        selectedWeaponName.text = "Name: ";
        selectedWeaponPrice.text = "PRICE: ";
        selectedWeaponPrice.color = Color.white;
        if (isOnline)
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            GetComponent<Canvas>().worldCamera = Camera.main;
        }
    }

    public void SelectButtonClicked()
    {
        if (selectedWeapon != null)
        {
            if (FindWeapon())
            {
                PlayerStatsInfo.currentWeapons[weaponChange] = selectedWeapon;
            }
        }
        LoadOutMenuBack();
    }

    public void BuyButtonClicked()
    {
        if (selectedWeapon != null)
        {
            if (!FindWeapon() && PlayerStatsInfo.gold >= selectedWeapon.price)
            {
                PlayerStatsInfo.gold -= selectedWeapon.price;
                PlayerStatsInfo.unlockedWeapons.Add(selectedWeapon);
            }
        }
    }

    public void LoadMenu()
    {
        PlayerStatsInfo.SaveProgress();
        PlayerStatsInfo.ResetInfo();
        SceneManager.LoadScene(0); 
    }

    public void NextScene()
    {
        SceneManager.LoadScene(PlayerStatsInfo.stage);
    }
}
