using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Lobby : NetworkBehaviour
    {
        [Inject]
        public NetworkManager _networkManager;
        [Inject]
        public NetworkLevel _networkLevel;
        public GameObject UI;
        public void Open(int id)
        {
            UI.SetActive(false);
            CmdOpen(NetworkLevel.LocalConnection, id);
        }
        private void Start()
        {
            UI.SetActive(true);
        }
        [Command(requiresAuthority = false)]
        public void CmdOpen(NetworkIdentity networkIdentity, int id)
        {
            _networkManager.ConnectToRoom(networkIdentity, id);
        }
    }
}