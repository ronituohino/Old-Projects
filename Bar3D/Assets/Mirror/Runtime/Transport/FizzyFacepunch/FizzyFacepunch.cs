using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Mirror.FizzySteam
{
    [HelpURL("https://github.com/Chykary/FizzyFacepunch")]
    public class FizzyFacepunch : Transport
    {
        private const string STEAM_SCHEME = "steam";

        private Client client;
        private Server server;

        private Common activeNode;

        [SerializeField]
        public P2PSend[] Channels = new P2PSend[1] { P2PSend.Reliable };

        [Tooltip("Timeout for connecting in seconds.")]
        public int Timeout = 25;
        [Tooltip("The Steam ID for your application.")]
        public uint SteamAppID = 480;
        [Tooltip("Allow or disallow P2P connections to fall back to being relayed through the Steam servers if a direct connection or NAT-traversal cannot be established.")]
        public bool AllowSteamRelay = true;

        [Header("Info")]
        [Tooltip("This will display your Steam User ID when you start or connect to a server.")]
        public ulong SteamUserID;

        ServerDataMonitor monitor;

        private void Awake()
        {
            monitor = gameObject.AddComponent<ServerDataMonitor>();

            Debug.Assert(Channels != null && Channels.Length > 0, "No channel configured for FizzySteamMirror.");
            try
            {
                if (SteamClient.IsValid)
                {
                    Debug.Log($"Skipping Steam init, {nameof(SteamClient)} already started");
                }
                else
                {
                    SteamClient.Init(SteamAppID, false);
                }
                Invoke(nameof(FetchSteamID), 1f);
            }
            catch (Exception e)
            {
                Debug.LogError($"FizzyFacepunch could not initialise: {e.Message}");
            }
        }

        private void LateUpdate()
        {
            if (enabled)
            {
                SteamClient.RunCallbacks();
                activeNode?.ReceiveData();
            }
        }

        public override bool ClientConnected() => ClientActive() && client.Connected;
        public override void ClientConnect(string address)
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("SteamWorks not initialized. Client could not be started.");
                OnClientDisconnected.Invoke();
                return;
            }

            FetchSteamID();

            if (ServerActive())
            {
                Debug.LogError("Transport already running as server!");
                return;
            }

            if (!ClientActive() || client.Error)
            {
                Debug.Log($"Starting client, target address {address}.");

                SteamNetworking.AllowP2PPacketRelay(AllowSteamRelay);
                client = Client.CreateClient(this, address);
                activeNode = client;
            }
            else
            {
                Debug.LogError("Client already running!");
            }
        }

        public override void ClientConnect(Uri uri)
        {
            if (uri.Scheme != STEAM_SCHEME)
                throw new ArgumentException($"Invalid url {uri}, use {STEAM_SCHEME}://SteamID instead", nameof(uri));

            ClientConnect(uri.Host);
        }

        public override void ClientSend(int channelId, ArraySegment<byte> segment)
        {
            byte[] data = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, data, 0, segment.Count);
            client.Send(data, channelId);
        }

        public override void ClientDisconnect()
        {
            if (ClientActive())
            {
                Shutdown();
            }
        }
        public bool ClientActive() => client != null;


        public override bool ServerActive() => server != null;
        public override void ServerStart()
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("SteamWorks not initialized. Server could not be started.");
                return;
            }

            FetchSteamID();

            if (ClientActive())
            {
                Debug.LogError("Transport already running as client!");
                return;
            }

            if (!ServerActive())
            {
                Debug.Log("Starting server.");
                SteamNetworking.AllowP2PPacketRelay(AllowSteamRelay);
                server = Server.CreateServer(this, NetworkManager.singleton.maxConnections);
                activeNode = server;
            }
            else
            {
                Debug.LogError("Server already started!");
            }
        }

        public override Uri ServerUri()
        {
            var steamBuilder = new UriBuilder
            {
                Scheme = STEAM_SCHEME,
                Host = SteamClient.SteamId.Value.ToString()
            };

            return steamBuilder.Uri;
        }

        public override void ServerSend(int connectionId, int channelId, ArraySegment<byte> segment)
        {
            if (ServerActive())
            {
                byte[] data = new byte[segment.Count];
                Array.Copy(segment.Array, segment.Offset, data, 0, segment.Count);
                server.SendAll(connectionId, data, channelId);

                monitor.AddBytes(segment.Count);
            }
        }

        public override bool ServerDisconnect(int connectionId) => ServerActive() && server.Disconnect(connectionId);
        public override string ServerGetClientAddress(int connectionId) => ServerActive() ? server.ServerGetClientAddress(connectionId) : string.Empty;
        public override void ServerStop()
        {
            if (ServerActive())
            {
                Shutdown();
            }
        }

        public override void Shutdown()
        {
            server?.Shutdown();
            client?.Disconnect();

            server = null;
            client = null;
            activeNode = null;
            Debug.Log("Transport shut down.");
        }

        public override int GetMaxPacketSize(int channelId)
        {
            if (channelId >= Channels.Length)
            {
                Debug.LogError("Channel Id exceeded configured channels! Please configure more channels.");
                return 1200;
            }

            switch (Channels[channelId])
            {
                case P2PSend.Unreliable:
                case P2PSend.UnreliableNoDelay:
                    return 1200;
                case P2PSend.Reliable:
                case P2PSend.ReliableWithBuffering:
                    return 1048576;
                default:
                    throw new NotSupportedException();
            }
        }

        public override bool Available()
        {
            try
            {
                return SteamClient.IsValid;
            }
            catch
            {
                return false;
            }
        }

        private void FetchSteamID()
        {
            if (SteamClient.IsValid)
            {
                SteamUserID = SteamClient.SteamId;
            }
        }

        private void OnDestroy()
        {
            if (activeNode != null)
            {
                Shutdown();
            }
        }
    }



    public class ServerDataMonitor : MonoBehaviour
    {
        public struct DataSent
        {
            public float time;
            public int amount;
        }

        Stack<DataSent> data = new Stack<DataSent>();

        public void AddBytes(int amount)
        {
            DataSent ds = new DataSent();
            ds.time = Time.time;
            ds.amount = amount;

            data.Push(ds);
        }

        float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0f;
                LogAmountOfDataSent();
            }
        }

        void LogAmountOfDataSent()
        {
            float amount = 0;
            Stack<DataSent> newData = new Stack<DataSent>();

            float curTime = Time.time;
            int index = 0;
            foreach (DataSent ds in data)
            {
                if (curTime - ds.time < 1f)
                {
                    amount += ds.amount;
                }
                else
                {
                    break;
                }

                index++;
            }

            for (int i = 0; i < index; i++)
            {
                newData.Push(data.Pop());
            }
            data = newData;

            int numOfOperations = 0;
            while (amount > 1024)
            {
                if (amount > 1024)
                {
                    numOfOperations += 1;
                    amount /= 1024f;
                }
            }

            amount = Mathf.RoundToInt(amount * 100) / 100f;

            string unit = "B/s";
            switch (numOfOperations)
            {
                case 0:
                    break;
                case 1:
                    unit = "kB/s";
                    break;
                case 2:
                    unit = "MB/s";
                    break;
                case 3:
                    unit = "GB/s";
                    break;
                default:
                    unit = "Broken";
                    break;
            }

            print(amount + " " + unit);
        }
    }
}