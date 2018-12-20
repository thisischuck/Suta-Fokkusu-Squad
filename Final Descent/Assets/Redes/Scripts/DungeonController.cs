using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

public class DungeonController : NetworkBehaviour{
    public GameObject dung;
    public GameObject dungChild;
    public int seed;
    public int Count = 0;

    public void StartDungeon(int seed)
    {
        if (isLocalPlayer)
        {
            this.seed = seed;
            dung = GameObject.Find("DungeonHolder");
            dungChild = dung.transform.Find("Dungeon").gameObject;
            dungChild.GetComponent<CellularAutomata>().SeedInspector = seed;
            dungChild.GetComponent<CellularAutomata>().IsOnline = true;
            dungChild.GetComponent<ObjectPlacer>().IsOnline = true;
            dungChild.GetComponent<CellularAutomata>().manager = this.gameObject;
            dungChild.SetActive(true);
        }
    }

    public void ReceiveConfirmation(Vector3[] spawns, int seed)
    {
        CmdDungeonDone(spawns, seed);
    }

    [Command]
    public void CmdDungeonDone(Vector3[] spawns, int seed)
    {
        NetworkManager.singleton.GetComponent<LobbyManager>().DungeonDone(this, spawns, seed);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
