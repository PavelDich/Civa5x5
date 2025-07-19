using System.Collections;
using System.Collections.Generic;
using Minicop.Game.GravityRave;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Alive : NetworkBehaviour, IDamageble, IData
    {
        #region Health
        public UnityEvent OnDeath = new UnityEvent();
        public float HealthMax
        {
            get { return _data.HealthMax; }
            set
            {
                _data.HealthMax = value;
                Health = Health;
            }
        }
        public float Health
        {
            get { return _data.Health; }
            set
            {
                _data.Health = math.clamp(value, 0, HealthMax);
                if (_data.Health == 0) OnDeath.Invoke();
            }
        }

        [Server]
        public void Damage(NetworkIdentity shooter, float damage)
        {
            Health -= damage;
        }
        #endregion



        #region Network
        [SyncVar]
        [SerializeField]
        private Data _data = new Data();
        [System.Serializable]
        public struct Data
        {
            public float HealthMax;
            public float Health;
        }
        #endregion



        public object GetData()
        {
            return _data;
        }
        public void SetData(object data)
        {
            _data = (Alive.Data)data;
        }
    }

    public static class AliveSerializer
    {
        public static void Write(this NetworkWriter writer, Alive.Data item)
        {
            writer.WriteFloat(item.Health);
            writer.WriteFloat(item.HealthMax);
        }

        public static Alive.Data Read(this NetworkReader reader)
        {
            return new Alive.Data
            {
                Health = reader.ReadFloat(),
                HealthMax = reader.ReadFloat(),
            };
        }
    }
}