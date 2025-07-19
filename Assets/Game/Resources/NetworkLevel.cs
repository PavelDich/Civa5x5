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
        public UnityEvent<NetworkIdentity> OnConnectionEnter = new UnityEvent<NetworkIdentity>();
        public UnityEvent OnConnectionLeave = new UnityEvent();
        public int SceneId
        {
            get { return _data.SceneId; }
            set { _data.SceneId = value; }
        }

        private void Start()
        {
            if (!NetworkServer.active) return;
            NetworkManager.OnPlayerDisconnect.AddListener(SrvRemovePlayer);
        }

        private void OnEnable()
        {
            if (!NetworkClient.active) return;
            StartCoroutine(WaitSpawnConnection());
        }

        private void OnDisable()
        {
            if (!NetworkClient.active) return;
            CmdRemoveConnection(LocalConnection);
            OnLocalConnectionLeave.Invoke();
        }

        public IEnumerator WaitSpawnConnection()
        {
            yield return new WaitForEndOfFrame();
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                NetworkIdentity networkIdentity = player.GetComponent<NetworkIdentity>();
                if (networkIdentity.isOwned)
                {
                    LocalConnection = networkIdentity;
                    CmdAddConnection(networkIdentity);
                    break;
                }
            }
            OnLocalConnectionEnter.Invoke();
        }
        [Command(requiresAuthority = false)]
        public void CmdAddConnection(NetworkIdentity networkIdentity)
        {
            Connections.Add(networkIdentity);
            foreach (NetworkIdentity connection in Connections)
                RpcConnectionEnter(connection.connectionToClient, networkIdentity);
        }
        [Command(requiresAuthority = false)]
        void CmdRemoveConnection(NetworkIdentity networkIdentity)
        {
            StartCoroutine(Remove());
            IEnumerator Remove()
            {
                yield return new WaitForEndOfFrame();
                Connections.Remove(networkIdentity);
                Connections.RemoveAll(p => p == null);
            }
        }

        [TargetRpc]
        public void RpcConnectionEnter(NetworkConnection networkConnection, NetworkIdentity networkIdentity)
        {
            OnConnectionEnter.Invoke(networkIdentity);
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

        public void Quit()
        {
            Application.Quit();
        }
        #region Network
        [SyncVar]
        [SerializeField]
        private Data _data = new Data();
        [System.Serializable]
        public struct Data
        {
            public int SceneId;
        }
        #endregion


        public object GetData()
        {
            return _data;
        }
        public void SetData(object data)
        {
            _data = (NetworkLevel.Data)data;
        }
    }

    public static class NetworkLevelSerializer
    {
        public static void Write(this NetworkWriter writer, NetworkLevel.Data item)
        {
            writer.WriteInt(item.SceneId);
        }

        public static NetworkLevel.Data Read(this NetworkReader reader)
        {
            return new NetworkLevel.Data
            {
                SceneId = reader.ReadInt(),
            };
        }
    }
}