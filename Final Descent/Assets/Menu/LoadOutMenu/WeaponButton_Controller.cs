using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponButton_Controller : MonoBehaviour
{
    private Button button;
    public WeaponObject weaponObject;
    private LoadOutMenu_Controller daddy;

    // Use this for initialization
    void Start()
    {
        daddy = GetComponentInParent<LoadOutMenu_Controller>();
        button = GetComponent<Button>();
        button.onClick.AddListener(() => SwitchSelectedWeapon(weaponObject));

        if (weaponObject != null)
        {
            GetComponent<Image>().sprite = weaponObject.sprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (weaponObject == null)
        {
            button.interactable = false;
        }
        else
        {
            button.interactable = true;
        }
        AlreadySelected();

        if (daddy.GetComponent<LoadOutMenu_Controller>().selectedWeapon != weaponObject)
        {
            GetComponent<CanvasGroup>().alpha = 0.3f;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    void AlreadySelected()
    {
        foreach (WeaponObject w in PlayerStatsInfo.currentWeapons)
        {
            if (w != null)
            {
                if (weaponObject.name == w.name)
                {
                    button.interactable = false;
                }
            }
        }
        
    }

    void SwitchSelectedWeapon(WeaponObject weaponObject)
    {
        daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponDesc.text = "Description: " + weaponObject.description;
        daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponName.text = "Name: " + weaponObject.name;
        daddy.GetComponent<LoadOutMenu_Controller>().selectedWeapon = weaponObject;
        if (!PlayerStatsInfo.unlockedWeapons.Contains(daddy.GetComponent<LoadOutMenu_Controller>().selectedWeapon))
        {
            daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponPrice.text = "PRICE: " + weaponObject.price.ToString();
            if (weaponObject.price > PlayerStatsInfo.gold)
                daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponPrice.color = Color.red;
            else if (weaponObject.price <= PlayerStatsInfo.gold)
                daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponPrice.color = Color.green;
        }
        else
        {
            daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponPrice.color = Color.green;
            daddy.GetComponent<LoadOutMenu_Controller>().selectedWeaponPrice.text = "BOUGHT";
        }
        // MANTER COR EUNQUANTO ESTIVER SELECIONADA
    }
}
