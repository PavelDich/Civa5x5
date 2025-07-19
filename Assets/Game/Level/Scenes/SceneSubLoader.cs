using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Search;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class SceneSubLoader : MonoBehaviour
    {
        public bool IsConnectOnStart;
        [Scene]
        public string Scene;
        [Inject]
        private NetworkManager _networkManager;
        private NetworkLevel _scene;

        private void Start()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (!NetworkServer.active) return;
            _networkManager.LoadSubScene(Scene, (resultscene, resultNetworkLevel) =>
            {
                _scene = resultNetworkLevel;
            });
            if (IsConnectOnStart) NetworkManager.OnPlayerConnect.AddListener(Connect);
#endif
        }
        [Server]
        public void Connect(NetworkIdentity player)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _networkManager.ConnectToScene(player, _scene.SceneId);
#endif
        }
    }
}