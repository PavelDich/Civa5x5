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
    public class Room : NetworkBehaviour
    {

        [Inject]
        public DiContainer _diContainer;
        [Inject]
        public NetworkManager _networkManager;
        [SyncVar]
        public bool IsFree;
        [SyncVar]
        public int RoomId = 0;
        [SyncVar]
        public int SceneId = 0;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [SyncVar]
        private string _name;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        [SyncVar]
        private string _password;


        public void LeaveRoom()
        {
            CmdLeaveRoom(NetworkLevel.LocalConnection);
        }
        [Command(requiresAuthority = false)]
        public void CmdLeaveRoom(NetworkIdentity networkIdentity)
        {
            _networkManager.LeaveOfRoom(networkIdentity);
        }

        /*
        public void Open(int id)
        {
            CmdOpen(LocalConnection, id);
        }
        [Command(requiresAuthority = false)]
        public void CmdOpen(NetworkIdentity networkIdentity, int id)
        {
            Debug.Log($"LoadRoom ({id})");
            _networkManager.ConnectToRoom(networkIdentity, id);
        }
                public void LeaveRoom()
                {
                    CmdLeaveRoom(NetworkLevel.LocalConnection);
                }
                [Command(requiresAuthority = false)]
                public void CmdLeaveRoom(NetworkIdentity networkIdentity)
                {
                    SrvRemovePlayer(networkIdentity);
                    _networkManager.LeaveOfRoom(networkIdentity);
                }

                [Server]
                public void SrvRemovePlayer(NetworkIdentity networkIdentity)
                {
                    NetworkIdentity player = Players.Find(player => player.connectionToClient == networkIdentity.connectionToClient);
                    Players.Remove(player);
                    Destroy(player.gameObject);
                    NetworkServer.Destroy(player.gameObject);
                }
                */
    }
}