using System.Collections;
using System.Collections.Generic;
using System.Net;
using kcp2k;
using UnityEngine;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class PlatformDependentCompilation : MonoBehaviour
    {
        [Inject]
        public NetworkManager _networkManager;
        [Inject]
        public KcpTransport _kcpTransport;


        private void Start()
        {
            JSONController.Load(ref Data, "NetworkData");
#if UNITY_EDITOR
            CreateServer();
            ConnectServer();
#endif
#if DEVELOPMENT_BUILD
            CreateServer();
#elif !UNITY_EDITOR
            ConnectServer();
#endif
        }

        private void CreateServer()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                Create();
            }
            catch
            {
                //Debug.Log($"Port denied");
            }
#endif
        }
        public void ConnectServer()
        {
            //#if UNITY_EDITOR
            StartCoroutine(WaitConnect());
            //#endif
        }

        public IEnumerator WaitConnect()
        {
            while (true)
            {
                //Debug.Log($"Wait to connect ({Data.IpAdress}:{Data.Port})");
                _networkManager.Connect(Data.IpAdress);
                yield return new WaitForSeconds(0.5f);
            }
        }

        private IEnumerator GetIpAdress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    //ip.ToString();
                    yield return null;
                }
                yield return null;
            }
        }

        public void Create()
        {
            _networkManager.Create();
        }


        public void Disconnect()
        {
            _networkManager.Disconnect();
        }
        public void Exit()
        {
            Application.Quit();
        }

        public static DataStruct Data = new DataStruct
        {
            Port = "49001",
            IpAdress = "localhost",
        };
        [System.Serializable]
        public struct DataStruct
        {
            public string Port;
            public string IpAdress;
        }
    }
}