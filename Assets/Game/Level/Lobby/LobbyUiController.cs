using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


namespace Minicop.Game.GravityRave
{
    public class LobbyUiController : MonoBehaviour
    {
        private void Start()
        {
            Refresh(0);
            Lobby.OnRoomAdd.AddListener(Refresh);
            Lobby.OnRoomRemove.AddListener(Refresh);
            Lobby.OnRoomOnlineChange.AddListener(Refresh);
        }

        public void Refresh(int id)
        {
            for (int i = 0; i < _panels.Count; i++)
                Destroy(_panels[i].gameObject);
            _panels.Clear();
            for (int i = 0; i < Lobby.ActiveRooms.Count; i++)
                if (Lobby.ActiveRooms[i]) AddPanel(i);
        }


        [SerializeField]
        private Lobby Lobby;
        [SerializeField]
        private List<RoomPanel> _panels = new List<RoomPanel>();
        [SerializeField]
        private RoomPanel _panelVariant;
        [SerializeField]
        private Transform _panelsParent;
        public void AddPanel(int id)
        {
            try
            {
                RoomPanel lobbyPanel = Instantiate(_panelVariant, _panelsParent);
                lobbyPanel.LobbyUiController = this;
                lobbyPanel.SetName(Lobby.RoomsData[id].Name);
                lobbyPanel.SetOnline($"{Lobby.RoomsData[id].ConnectionCount}/{Lobby.RoomsData[id].ConnectionMax}");
                lobbyPanel.SetPassword(Lobby.RoomsData[id].Password != "" && Lobby.RoomsData[id].Password != null);
                lobbyPanel.RoomId = Lobby.RoomsData[id].Id;
                _panels.Add(lobbyPanel);
            }
            catch
            {
                return;
            }
        }


        private string _name;
        public void SetName(string name) => _name = name;
        private string _password;
        public void SetPassword(string password) => _password = password;
        private string _passwordConnect;
        public void SetPasswordConnect(string password) => _passwordConnect = password;
        private int _connectionMax;
        public void SetConnectionMax(string connectionMax) => _connectionMax = int.Parse(connectionMax);

        public void Create()
        {
            Lobby.Create(_name, _password, _connectionMax);
        }
        public void Connect(int id)
        {
            Lobby.Connect(id, _passwordConnect);
        }
    }
}
