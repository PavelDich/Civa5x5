using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Lobby : NetworkBehaviour
    {

        [ClientRpc]
        public void RpcRoomAdd(int id) => OnRoomAdd.Invoke(id);
        public UnityEvent<int> OnRoomAdd = new UnityEvent<int>();
        [ClientRpc]
        public void RpcRoomRemove(int id) => OnRoomAdd.Invoke(id);
        public UnityEvent<int> OnRoomRemove = new UnityEvent<int>();
        [ClientRpc]
        public void RpcRoomOnlineChange(int id) => OnRoomAdd.Invoke(id);
        public UnityEvent<int> OnRoomOnlineChange = new UnityEvent<int>();
        [Inject]
        public DiContainer _diContainer;
        [Inject]
        public NetworkManager _networkManager;

        public SyncList<bool> ActiveRooms = new SyncList<bool>();
        public List<Room> Rooms = new List<Room>();
        public List<Scene> RoomScenes = new List<Scene>();
        public SyncList<Room.Data> RoomsData = new SyncList<Room.Data>();
        public SyncList<NetworkLevel.Data> RoomNetworkLevelsData = new SyncList<NetworkLevel.Data>();
        [Scene]
        public string Room;

        public void Connect(int id, string password)
        {
            CmdOpen(NetworkLevel.LocalConnection, id, password);
        }
        [Command(requiresAuthority = false)]
        private void CmdOpen(NetworkIdentity networkIdentity, int id, string password)
        {
            SrvOpen(networkIdentity, id, password);
        }
        [Server]
        private void SrvOpen(NetworkIdentity networkIdentity, int id, string password)
        {
            if (password != null)
                if (password != Rooms[id].Password) return;
            if (Rooms[id].ConnectionCount >= Rooms[id].ConnectionMax) return;

            _networkManager.ConnectToScene(networkIdentity, Rooms[id].GetComponent<NetworkLevel>().SceneId);
            Rooms[id].ConnectionCount = Rooms[id].GetComponent<NetworkLevel>().Connections.Count + 1;
            RoomsData[id] = (Room.Data)Rooms[id].GetData();
        }

        [Server]
        public void SrvLeaveRoom(NetworkIdentity networkIdentity, int id)
        {
            _networkManager.ConnectToScene(networkIdentity, GetComponent<NetworkLevel>().SceneId);
            Rooms[id].ConnectionCount = Rooms[id].GetComponent<NetworkLevel>().Connections.Count - 1;
            RoomsData[id] = (Room.Data)Rooms[id].GetData();

            if (Rooms[id].ConnectionCount <= 0) SrvRemove(id);
        }



        public void Create(string name, string password, int connectionMax)
        {
            CmdCreate(NetworkLevel.LocalConnection, name, password, connectionMax);
        }
        [Command(requiresAuthority = false)]
        private void CmdCreate(NetworkIdentity networkIdentity, string name, string password, int connectionMax)
        {
            for (int i = 0; i < ActiveRooms.Count; i++)
            {
                if (!ActiveRooms[i])
                {
                    _networkManager.LoadSubScene(Room, (scene, networkLevel) =>
                     {
                         ActiveRooms[i] = true;
                         Room room = networkLevel.gameObject.GetComponent<Room>();
                         Rooms[i] = room;
                         RoomScenes[i] = scene;

                         room.Lobby = this;
                         room.Name = name;
                         if (password == "" || password == " ") password = null;
                         room.Password = password;
                         room.ConnectionMax = math.max(connectionMax, 1);
                         room.Id = i;

                         RoomsData[i] = (Room.Data)networkLevel.GetComponent<Room>().GetData();
                         RoomNetworkLevelsData[i] = (NetworkLevel.Data)networkLevel.GetComponent<NetworkLevel>().GetData();

                         SrvOpen(networkIdentity, i, password);
                         RpcRoomAdd(i);
                     });
                    return;
                }
            }
            _networkManager.LoadSubScene(Room, (scene, networkLevel) =>
             {
                 ActiveRooms.Add(true);
                 Room room = networkLevel.gameObject.GetComponent<Room>();
                 Rooms.Add(room);
                 RoomScenes.Add(scene);

                 room.Lobby = this;
                 room.Name = name;
                 room.Password = password;
                 room.ConnectionMax = math.max(connectionMax, 1);
                 room.Id = Rooms.Count - 1;

                 RoomsData.Add((Room.Data)networkLevel.GetComponent<Room>().GetData());
                 RoomNetworkLevelsData.Add((NetworkLevel.Data)networkLevel.GetComponent<NetworkLevel>().GetData());

                 SrvOpen(networkIdentity, Rooms.Count - 1, password);
                 RpcRoomAdd(Rooms.Count - 1);
             });
            return;

        }

        [Server]
        private void SrvRemove(int id)
        {
            ActiveRooms[id] = false;
            Rooms[id] = null;
            _networkManager.UnloadSubScene(RoomScenes[id]);
        }
    }
}