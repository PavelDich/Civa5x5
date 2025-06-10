using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Lobby : NetworkBehaviour
    {
        [Inject]
        public DiContainer _diContainer;
        [Inject]
        public NetworkManager _networkManager;
        public UnityEvent<NetworkIdentity> OnRoomAdd = new UnityEvent<NetworkIdentity>();
        //Add event class OnDestroy
        public UnityEvent<NetworkIdentity> OnRoomRemove = new UnityEvent<NetworkIdentity>();
        public void Connect(int id, string password)
        {
            CmdOpen(NetworkLevel.LocalConnection, id, password);
        }
        [Command(requiresAuthority = false)]
        public void CmdOpen(NetworkIdentity networkIdentity, int id, string password)
        {
            Debug.Log($"LoadRoom ({id})");
            _networkManager.ConnectToRoom(networkIdentity, id);
        }
    }
}