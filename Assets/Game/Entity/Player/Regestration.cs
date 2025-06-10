using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Regestration : NetworkBehaviour
    {
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

        private void Start()
        {
            JSONController.Load(ref UserData, "UserRegestrationData");
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