using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Regestration : NetworkBehaviour
    {
        private void Start()
        {
            JSONController.Load(ref UserData, "UserRegestrationData");
            OnLoginLoad.Invoke(UserData.Login);
            OnEmailLoad.Invoke(UserData.Email);
            OnPasswordLoad.Invoke(UserData.Password);

            _databaseHandler.OnRegisterDenied.AddListener(OnRegisterDenied.Invoke);
            _databaseHandler.OnRegisterAllowed.AddListener(Accept);
            _databaseHandler.OnEnterDenied.AddListener(OnEnterDenied.Invoke);
            _databaseHandler.OnEnterAllowed.AddListener(Accept);

        }
        private void Accept(NetworkIdentity networkIdentity)
        {
            //NetworkLevel.LocalConnection.GetComponent<ConnectionInfo>().Name = Login;
            OnAccept.Invoke(networkIdentity);
        }
        public UnityEvent<NetworkIdentity> OnRegisterDenied = new UnityEvent<NetworkIdentity>();
        public UnityEvent<NetworkIdentity> OnEnterDenied = new UnityEvent<NetworkIdentity>();
        public UnityEvent<NetworkIdentity> OnAccept = new UnityEvent<NetworkIdentity>();

        public UnityEvent<string> OnLoginLoad = new UnityEvent<string>();
        public UnityEvent<string> OnEmailLoad = new UnityEvent<string>();
        public UnityEvent<string> OnPasswordLoad = new UnityEvent<string>();
        [Inject]
        public NetworkManager _networkManager;
        [Inject]
        public NetworkLevel _networkLevel;
        [Inject]
        public DatabaseHandler _databaseHandler;


        public string Email
        {
            get { return UserData.Email; }
            set { UserData.Email = value; }
        }
        public string Login
        {
            get { return UserData.Login; }
            set { UserData.Login = value; }
        }
        public string Password
        {
            get { return UserData.Password; }
            set { UserData.Password = value; }
        }

        public void Registration()
        {
            JSONController.Save(UserData, "UserRegestrationData");

            CmdRegistration(NetworkLevel.LocalConnection, Email, Password, Login);
        }
        [Command(requiresAuthority = false)]
        void CmdRegistration(NetworkIdentity networkIdentity, string email, string password, string login)
        {
            _databaseHandler.SaveUser(networkIdentity, email, password, login);
        }
        public void Enter()
        {
            JSONController.Save(UserData, "UserRegestrationData");

            CmdEnter(NetworkLevel.LocalConnection, Email, Password, Login);
        }
        [Command(requiresAuthority = false)]
        void CmdEnter(NetworkIdentity networkIdentity, string email, string password, string login)
        {
            _databaseHandler.CheckUser(networkIdentity, email, password, login);
        }


        public static UserDataStruct UserData = new UserDataStruct();
        [System.Serializable]
        public struct UserDataStruct
        {
            public string Email;
            public string Password;
            public string Login;
        }
    }
}