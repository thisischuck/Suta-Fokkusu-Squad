using UnityEngine;

public class PlayerInfoReader : MonoBehaviour
{
    private GameObject ship;

    // Use this for initialization
    void Start()
    {
        ship = GetComponent<PlayerMovement>().ship;
        GameObject shipWithColor = ship.transform.Find("Player_aircraft").Find("Aircraft").gameObject;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip1 = PlayerStatsInfo.shipColor1;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip2 = PlayerStatsInfo.shipColor2;
        shipWithColor.GetComponent<DynamicTexture>().ColorShip3 = PlayerStatsInfo.shipColor3;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
