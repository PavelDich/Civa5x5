using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using UnityEngine.Events;
using Zenject;
using UnityEngine.Networking.Types;
using Mirror.BouncyCastle.Crypto.Generators;
using Mirror.BouncyCastle.Bcpg.OpenPgp;
using Mirror.Examples.MultipleMatch;
using System.Linq;


namespace Minicop.Game.GravityRave
{
    public class NetworkManager : Mirror.NetworkManager
    {
        [Inject]
        public DiContainer diContainer;
        //[SerializeField]
        //private PlayerInfo _playerInfo;
        public bool playerSpawned;
        public static UnityEvent<NetworkIdentity> OnPlayerConnect = new UnityEvent<NetworkIdentity>();
        public static UnityEvent OnPlayerDisconnect = new UnityEvent();
        public static UnityEvent OnClientStarted = new UnityEvent();
        public static UnityEvent OnClientStopped = new UnityEvent();

        public Transform GetRespawn()
        {
            return GetStartPosition();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
        }
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            OnPlayerDisconnect.Invoke();
        }

        public override void OnClientConnect()
        {
            OnClientStarted.Invoke();
            base.OnClientConnect();
        }
        public override void OnClientDisconnect()
        {
            OnClientStopped.Invoke();
            base.OnClientDisconnect();
        }

        public void Create()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            try
            {
                if (!NetworkServer.active)//&&!NetworkClient.isConnected)
                {
                    Debug.Log($"Host started");
                    StartServer();
                }
                else
                {
                    Debug.Log($"You'r are client or server active");
                }
            }
            catch
            {
                Debug.Log($"Critical error on create");
            }
#endif
        }

        public void Connect(string ipAdress)
        {
            try
            {
                if (!NetworkClient.isConnected && !NetworkServer.active && !string.IsNullOrWhiteSpace(ipAdress))
                {
                    //Debug.Log($"Client started");
                    networkAddress = ipAdress;
                    StartClient();
                }
                else
                {
                    //Debug.Log($"Ip adress incorrect");
                }
            }
            catch
            {
                //Debug.Log($"Critical error on connect");
            }
        }

        public void Disconnect()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                //Debug.Log($"Host stopped");
                NetworkManager.singleton.StopClient();
                NetworkManager.singleton.StopHost();
            }
            else
            {
                Debug.Log($"Client stopped");
                NetworkManager.singleton.StopClient();
            }
        }
        public void LoadScene(int Scane)
        {
            SceneManager.LoadScene(Scane);
        }






        public static UnityEvent<NetworkIdentity> OnSubSceneLoad = new UnityEvent<NetworkIdentity>();

        [SerializeField]
        public List<NetworkLevel> ActiveNetworkLevels = new List<NetworkLevel>();
        [SerializeField]
        public List<Scene> ActiveSubScenes = new List<Scene>();


        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            StartCoroutine(OnServerAddPlayerDelayed(conn));
        }
        IEnumerator OnServerAddPlayerDelayed(NetworkConnectionToClient conn)
        {
            yield return new WaitForEndOfFrame();

            base.OnServerAddPlayer(conn);
            OnPlayerConnect.Invoke(conn.identity);
        }



        public override void OnStartServer()
        {
            base.OnStartServer();
        }



        public void LoadSubScene(string scene, System.Action<Scene, NetworkLevel> subScene)
        {
            StartCoroutine(ServerLoadSubScene(scene, subScene));
        }
        [Server]
        private IEnumerator ServerLoadSubScene(string scene, System.Action<Scene, NetworkLevel> subScene)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            yield return new WaitForEndOfFrame();
            yield return SceneManager.LoadSceneAsync(scene, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });
            Scene loadScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
            foreach (GameObject obj in loadScene.GetRootGameObjects())
                if (obj.TryGetComponent<NetworkLevel>(out NetworkLevel networkLevel))
                {
                    AddSubScenes(loadScene, networkLevel);
                    subScene.Invoke(loadScene, networkLevel);
                }
#endif
            yield return null;
        }

        [Server]
        public void AddSubScenes(Scene scene, NetworkLevel networkLevel)
        {
            for (int i = 0; i < ActiveSubScenes.Count; i++)
            {
                if (ActiveNetworkLevels[i]) continue;
                ActiveNetworkLevels[i] = networkLevel;
                ActiveSubScenes[i] = scene;
                networkLevel.SceneId = i;
                return;
            }
            ActiveNetworkLevels.Add(networkLevel);
            ActiveSubScenes.Add(scene);
            networkLevel.SceneId = ActiveNetworkLevels.Count - 1;
        }



        [Server]
        public void ConnectToScene(NetworkIdentity networkIdentity, int id)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            OnSubSceneLoad.Invoke(networkIdentity);
            StartCoroutine(SceneConecting(networkIdentity.connectionToClient, id));
#endif
        }
        [Server]
        private IEnumerator SceneConecting(NetworkConnectionToClient conn, int id)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            yield return new WaitForEndOfFrame();
            conn.Send(new SceneMessage { sceneName = ActiveSubScenes[id].name, sceneOperation = SceneOperation.LoadAdditive });

            SceneManager.MoveGameObjectToScene(conn.identity.gameObject, ActiveSubScenes[id]);
#endif
            yield return null;
        }

        public override void OnStopServer()
        {
            for (int index = 0; index < ActiveSubScenes.Count; index++)
            {
                NetworkServer.SendToAll(new SceneMessage { sceneName = ActiveSubScenes[index].name, sceneOperation = SceneOperation.UnloadAdditive });
            }
            //NetworkServer.SendToAll(new SceneMessage { sceneName = LobbyScenes.Scene, sceneOperation = SceneOperation.UnloadAdditive });
            StartCoroutine(ServerUnloadSubScenes());
        }

        private IEnumerator ServerUnloadSubScenes()
        {
            for (int index = 0; index < ActiveSubScenes.Count; index++)
                if (ActiveSubScenes[index].IsValid())
                    yield return SceneManager.UnloadSceneAsync(ActiveSubScenes[index]);
            ActiveSubScenes.Clear();

            yield return Resources.UnloadUnusedAssets();
        }

        public void UnloadSubScene(Scene scene)
        {
            StartCoroutine(ServerUnloadSubScenes(scene));
        }

        private IEnumerator ServerUnloadSubScenes(Scene scene)
        {
            yield return SceneManager.UnloadSceneAsync(scene);
            yield return Resources.UnloadUnusedAssets();
        }

        public override void OnStopClient()
        {
            if (mode == NetworkManagerMode.Offline)
                StartCoroutine(ClientUnloadSubScenes());
        }

        public void ClientUnloadOfSubScenes()
        {
            StartCoroutine(ClientUnloadSubScenes());
        }

        IEnumerator ClientUnloadSubScenes()
        {
            for (int index = 0; index < SceneManager.sceneCount; index++)
                if (SceneManager.GetSceneAt(index) != SceneManager.GetActiveScene())
                    yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(index));
        }
    }

    public interface IData
    {
        public object GetData();
        public void SetData(object data);
    }
}
