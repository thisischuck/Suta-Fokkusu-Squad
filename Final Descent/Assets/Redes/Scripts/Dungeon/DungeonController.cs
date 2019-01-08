using Prototype.NetworkLobby;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DungeonController : NetworkBehaviour{
    public GameObject dung;
    public GameObject dungChild;
    [SyncVar]
    public int seed;
    public int Count = 0;
    private GameObject lobbyPlayer;

    Vector3[] spawns;

    private void Start()
    {
       
    }
    [ClientRpc]
    public void RpcGivePlayerStats(GameObject lP)
    {
        lobbyPlayer = lP;
    }

    [ClientRpc]
    public void RpcStartDungeon(int seed)
    {
        this.seed = seed;
        dung = GameObject.Find("DungeonHolder");
        dungChild = dung.transform.Find("Dungeon_real").gameObject;
        dungChild.GetComponent<CellularAutomata>().SeedInspector = seed;
        dungChild.GetComponent<CellularAutomata>().IsOnline = true;
        if (isServer)
        {
            dungChild.GetComponent<CellularAutomata>().isServer = true;
            dungChild.GetComponent<ObjectPlacer>().IsServer = true;
        }
        //dungChild.GetComponent<ObjectPlacer>().IsOnline = true;
        dungChild.GetComponent<CellularAutomata>().manager = this.gameObject;
        dungChild.SetActive(true);

    }

    private void FixedUpdate()
    {
        //StartDungeon(seed);
    }

    public void ReceiveConfirmation(Vector3[] spawns, int seed)
    {
        this.spawns = spawns;
        this.seed = seed;
        CmdDungeonDone(spawns, seed, lobbyPlayer);
    }


    public void AllDone()
    {
        if (!isServer)
            CmdDungeonDone(spawns, seed, lobbyPlayer);
        else
            NetworkManager.singleton.GetComponent<LobbyManager>().DungeonDone(this, spawns, seed, lobbyPlayer);
    }

    public void SpawnEnemy(Vector3 position, Quaternion rotation)
    {
            CmdSpawnEnemy(position, rotation);
    }

    [Command]
    private void CmdSpawnEnemy(Vector3 position, Quaternion rotation)
    {
        GameObject g = Instantiate(NetworkManager.singleton.GetComponent<LobbyManager>().spawnPrefabs[3], position, rotation);
        NetworkServer.Spawn(g);
    }

    [Command]
    public void CmdDungeonDone(Vector3[] spawns, int seed, GameObject lobbyPlayer)
    {
        NetworkManager.singleton.GetComponent<LobbyManager>().DungeonDone(this, spawns, seed, lobbyPlayer);
    }


    // Update is called once per frame
    void Update () {
		
	}
}
