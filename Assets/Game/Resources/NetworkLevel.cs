using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Minicop.Game.GravityRave;
using Mirror;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Events;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class NetworkLevel : NetworkBehaviour
    {

        [Inject]
        public DiContainer _diContainer;
        [Inject]
        public NetworkManager _networkManager;
        [SerializeField]
        private static NetworkIdentity _localConnection;
        [SerializeField]
        private Transform[] _spawns;
        public static NetworkIdentity LocalConnection
        {
            get
            {
                return _localConnection;
            }
            set
            {
                if (_localConnection != null) return;
                _localConnection = value;
                Debug.Log("Local Player Find");
            }
        }
        public List<NetworkIdentity> Connections = new List<NetworkIdentity>();
        public UnityEvent OnLocalConnectionLeave = new UnityEvent();
        public UnityEvent OnLocalConnectionEnter = new UnityEvent();
        public UnityEvent OnConnectionEnter = new UnityEvent();
        public UnityEvent OnConnectionLeave = new UnityEvent();
        [SyncVar]
        public int RoomId = 0;
        [SyncVar]
        public int SceneId = 0;

        private void Start()
        {
            NetworkManager.OnPlayerDisconnect.AddListener(SrvRemovePlayer);
        }

        private void OnEnable()
        {
            StartCoroutine(WaitSpawnPlayer());
            OnLocalConnectionEnter.Invoke();
        }

        private void OnDisable()
        {
            OnLocalConnectionLeave.Invoke();
        }

        public IEnumerator WaitSpawnPlayer()
        {
            yield return new WaitForEndOfFrame();
            FindPlayers();
        }
        public void FindPlayers()
        {
            if (!NetworkClient.active) return;
            CmdFindPlayers();
        }
        [Command(requiresAuthority = false)]
        public void CmdFindPlayers()
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject p in players)
            {
                NetworkIdentity networkIdentity = p.GetComponent<NetworkIdentity>();
                RpcPlayerEnter(networkIdentity.connectionToClient);
                RpcFindLocalPlayers(networkIdentity.connectionToClient, networkIdentity);
            }
        }

        [TargetRpc]
        public void RpcPlayerEnter(NetworkConnectionToClient conn)
        {
            OnConnectionEnter.Invoke();
        }
        [TargetRpc]
        public void RpcFindLocalPlayers(NetworkConnectionToClient conn, NetworkIdentity localPlayer)
        {
            Debug.Log("Find");
            LocalConnection = localPlayer;
        }

        [Server]
        void SrvRemovePlayer()
        {
            StartCoroutine(Remove());
            IEnumerator Remove()
            {
                yield return new WaitForEndOfFrame();
                Connections.RemoveAll(p => p == null);
            }
        }

        [Server]
        public void SrvRemovePlayer(NetworkIdentity networkIdentity)
        {
            NetworkIdentity player = Connections.Find(player => player.connectionToClient == networkIdentity.connectionToClient);
            Connections.Remove(player);
            Destroy(player.gameObject);
            NetworkServer.Destroy(player.gameObject);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}