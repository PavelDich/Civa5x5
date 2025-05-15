using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    public class PlayerProfile : MonoBehaviour
    {
        private void Start()
        {
            JSONController.Load(ref Data, "PlayerProfiles");
        }

        public static List<DataStruct> Data;
        [System.Serializable]
        public struct DataStruct
        {
            public string Name;
            public string Password;
            public int Level;
        }


        private string _name;
        private string Name
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        private string _password;
        private string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }
        public UnityEvent RegisterDenied = new UnityEvent();
        public UnityEvent RegisterAllowed = new UnityEvent();
        public UnityEvent EnterDenied = new UnityEvent();
        public UnityEvent EnterAllowed = new UnityEvent();

        public void Register()
        {
            for (int i = 0; i < Data.Count; i++)
            {
                if (Name == Data[i].Name)
                {
                    RegisterDenied.Invoke();
                    return;
                }
            }
            RegisterAllowed.Invoke();
            JSONController.Save(Data, "PlayerProfiles");
        }
        public void Enter()
        {
            DataStruct data = Data.Find(x => x.Name == Name);
            if (data.Password != Password)
            {
                EnterDenied.Invoke();
                return;
            }
            EnterAllowed.Invoke();
            JSONController.Save(Data, "PlayerProfiles");
        }
    }
}