using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;

// Handles all networking traffic in the game
public class NetworkManagerGame : NetworkManager
{
    [Header("Custom")]

    bool connected = false;
    bool stopping = false;
    uint playerParent = uint.MaxValue;

    new void Start()
    {
        base.Start();

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    //Sent to the server when a client connects
    public struct CreatePlayer : NetworkMessage
    {
        public string playerName;
    }

    //This is sent back when a player is created to indicate which one it is
    public struct PlayerToken : NetworkMessage
    {
        public uint netId;
    }

    //When a client receives this, the client is stopped
    public struct StopPlayerClient : NetworkMessage { }

    [Server]
    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<CreatePlayer>(OnCreatePlayer);
    }

    [Server]
    void OnCreatePlayer(NetworkConnection conn, CreatePlayer createPlayer)
    {
        //Server scene setup when creating host
        if(NetworkServer.connections.Count <= 1)
        {
            playerParent = GlobalReferencesAndSettings.Instance.playersParent.GetComponent<NetworkIdentity>().netId;
        }

        //Create new player
        GameObject g = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, g);

        PlayerControls c = g.GetComponent<PlayerControls>();
        c.hierarchyPosition = new MirrorHierarchyNetworkBehaviour.HierarchyPosition
        (
            playerParent,
            g.transform.position,
            0f,
            g.transform.localScale
        );

        c.playerName = createPlayer.playerName;

        //Send a token back which holds this object
        PlayerToken pt = new PlayerToken();
        pt.netId = c.GetComponent<NetworkIdentity>().netId;
        conn.Send<PlayerToken>(pt);
    }

    [Server]
    public override void OnStopServer()
    {
        NetworkServer.SendToAll<StopPlayerClient>(new StopPlayerClient());
    }

    [Server]
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        NetworkServer.RemovePlayerForConnection(conn, true);
    }



    //Connecting
    [Client]
    public override void OnClientConnect(NetworkConnection conn)
    {
        NetworkClient.RegisterHandler<StopPlayerClient>(ForceCloseByServer);
        NetworkClient.RegisterHandler<PlayerToken>(SetPlayer);

        StartCoroutine(StartGame(conn));
    }

    [Client]
    IEnumerator StartGame(NetworkConnection conn)
    {
        connected = true;
        AsyncOperation async = SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        yield return async;

        StartCoroutine(UnloadAssets(1));

        NetworkClient.Ready();

        CreatePlayer msg = new CreatePlayer();
        msg.playerName = ":)";
        conn.Send<CreatePlayer>(msg);
    }

    [Client]
    //Server send back a token that contains the players netId, subscribe player to local InputManager
    void SetPlayer(PlayerToken pt)
    {
        PlayerControls pc = NetworkIdentity.spawned[pt.netId].GetComponent<PlayerControls>();
        InputManager.Instance.controlledPlayer = pc;
    }

    IEnumerator UnloadAssets(int sceneIndex)
    {
        AsyncOperation async = SceneManager.UnloadSceneAsync(sceneIndex);
        yield return async;

        Resources.UnloadUnusedAssets();
        stopping = false;
    }



    //Disconnecting
    [Client]
    public void ForceCloseByServer(StopPlayerClient msg)
    {
        if (!stopping && connected)
        {
            StopClient();
        }
    }

    //Called on all clients, including the host
    public override void OnStopClient()
    {
        if (!stopping && connected)
        {
            StartCoroutine(CloseGame());
        }
    }

    IEnumerator CloseGame()
    {
        connected = false;
        stopping = true;

        AsyncOperation async = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        yield return async;

        StartCoroutine(UnloadAssets(2));
    }
}
