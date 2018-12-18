using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


struct PlayerState
{
    public float hor, ver;
    public Vector3 playerPosition;
    public Quaternion playerRotation;
    public int msgNum;
}

public class Network_PlayerMovement : NetworkBehaviour {

    [SyncVar(hook = "OnServerStateChange")] PlayerState serverState;
    Queue<KeyCode> pendingMoves;
    PlayerState predictedState;
    Rigidbody rB;
    private float _timePerUpdate = 0.16f;
    private float _lastUpdateTime;

    void Awake()
    {
        InitState();
    }

    void Start()
    {
        SyncState();
        if (isLocalPlayer)
        {
            rB = new Rigidbody();
            
            pendingMoves = new Queue<KeyCode>();
            UpdatePredictedState();
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            KeyCode[] arrowKeys = { KeyCode.RightArrow, KeyCode.LeftArrow , KeyCode.None};
            foreach (KeyCode arrowKey in arrowKeys)
            {
                if (!Input.GetKey(arrowKey)) continue;
                pendingMoves.Enqueue(arrowKey);
                UpdatePredictedState();
                CmdMove(arrowKey);
                _lastUpdateTime = Time.time;
            }
        }
        SyncState();
    }

    void GetInputs()
    {
        //Input.GetAxis
    }

    [Command]
    void CmdMove(KeyCode arrowKey)
    {
        serverState = Move(serverState, arrowKey);
    }

    [Server]
    void InitState()
    {
        serverState = new PlayerState
        {
            playerPosition = Vector3.zero,
            playerRotation = Quaternion.identity,
        };
    }

    PlayerState Move(PlayerState previous, KeyCode arrowKey)
    {
        int dx = 0;
        switch (arrowKey)
        {
            case KeyCode.RightArrow:
                dx = 1;
                break;
            case KeyCode.LeftArrow:
                dx = -1;
                break;
            case KeyCode.None:
                dx = 0;
                break;
        }
        return new PlayerState
        {
            hor = dx,
            msgNum = previous.msgNum + 1
        };
    }

    void SyncState()
    {
        float f = (Time.time - _lastUpdateTime) / _timePerUpdate;
        PlayerState st = isLocalPlayer ? predictedState : serverState;
        //if (transform.position.x != st.x)
        //{
        Rigidbody rigidB = GetComponent<Rigidbody>();
        Vector3 move = new Vector3(st.hor, 0.0f, st.ver);
        move = transform.TransformDirection(move);
        rigidB.velocity = move * 20;
        Debug.Log(rigidB.position.ToString());
        //}
    }


    void UpdatePredictedState()
    {
        predictedState = serverState;
        foreach (KeyCode keyCode in pendingMoves)
        {
            predictedState = Move(predictedState, keyCode);
        }
    }

    void OnServerStateChange(PlayerState newState)
    {
        serverState = newState;

        if (pendingMoves != null)
        {
            while (pendingMoves.Count > predictedState.msgNum - serverState.msgNum)
            {
                Debug.Log(pendingMoves.Peek().ToString());
                pendingMoves.Dequeue();
            }
        }
        //UpdatePredictedState();
    }
}

