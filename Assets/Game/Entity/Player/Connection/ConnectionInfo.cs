using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    public class ConnectionInfo : NetworkBehaviour, IData
    {

        public string Name
        {
            get { return _data.Name; }
            set { _data.Name = value; }
        }
        public int Level
        {
            get { return _data.Level; }
            set { _data.Level = value; }
        }
        public int Status
        {
            get { return _data.Status; }
            set { _data.Status = value; }
        }

        #region Network
        [SyncVar]
        [SerializeField]
        private Data _data = new Data();
        [System.Serializable]
        public struct Data
        {
            public string Name;
            public int Level;
            public int Status;
        }
        #endregion


        public object GetData()
        {
            return _data;
        }
        public void SetData(object data)
        {
            _data = (ConnectionInfo.Data)data;
        }
        public void SetData(Data data)
        {
            _data = data;
        }
    }

    public static class ConnectionInfoSerializer
    {
        public static void Write(this NetworkWriter writer, ConnectionInfo.Data item)
        {
            writer.WriteString(item.Name);
            writer.WriteInt(item.Level);
            writer.WriteInt(item.Status);
        }

        public static ConnectionInfo.Data Read(this NetworkReader reader)
        {
            return new ConnectionInfo.Data
            {
                Name = reader.ReadString(),
                Level = reader.ReadInt(),
                Status = reader.ReadInt(),
            };
        }
    }
}