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
    public class Room : NetworkBehaviour, IData
    {
        public Lobby Lobby;
        [Inject]
        public DiContainer _diContainer;
        [Inject]
        public NetworkManager _networkManager;
        public string Name
        {
            get { return _data.Name; }
            set { _data.Name = value; }
        }
        public string Password
        {
            get { return _data.Password; }
            set { _data.Password = value; }
        }
        public int Id
        {
            get { return _data.Id; }
            set { _data.Id = value; }
        }
        public int ConnectionCount
        {
            get { return _data.ConnectionCount; }
            set { _data.ConnectionCount = value; }
        }
        public int ConnectionMax
        {
            get { return _data.ConnectionMax; }
            set { _data.ConnectionMax = value; }
        }


        public void LeaveRoom()
        {
            CmdLeaveRoom(NetworkLevel.LocalConnection);
        }
        [Command(requiresAuthority = false)]
        public void CmdLeaveRoom(NetworkIdentity networkIdentity)
        {
            Lobby.SrvLeaveRoom(networkIdentity, Id);
        }

        #region Network
        [SyncVar]
        [SerializeField]
        private Data _data = new Data();
        [System.Serializable]
        public struct Data
        {
            public int ConnectionCount;
            public int ConnectionMax;
            public int Id;
            public string Name;
            public string Password;
        }
        #endregion


        public object GetData()
        {
            return _data;
        }
        public void SetData(object data)
        {
            _data = (Room.Data)data;
        }
        public void SetData(Data data)
        {
            _data = data;
        }
    }

    public static class RoomSerializer
    {
        public static void Write(this NetworkWriter writer, Room.Data item)
        {
            writer.WriteInt(item.ConnectionCount);
            writer.WriteInt(item.ConnectionMax);
            writer.WriteInt(item.Id);
            writer.WriteString(item.Name);
            writer.WriteString(item.Password);
        }

        public static Room.Data Read(this NetworkReader reader)
        {
            return new Room.Data
            {
                ConnectionCount = reader.ReadInt(),
                ConnectionMax = reader.ReadInt(),
                Id = reader.ReadInt(),
                Name = reader.ReadString(),
                Password = reader.ReadString(),
            };
        }
    }
}