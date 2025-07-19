using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class ConnectWindow : MonoBehaviour
    {
        private void Awake()
        {
            NetworkManager.OnClientStarted.AddListener(SetDisable);
            NetworkManager.OnClientStopped.AddListener(SetEnable);
        }

        [SerializeField]
        private UnityEvent OnEnable = new UnityEvent();
        public void SetEnable() => OnEnable.Invoke();
        [SerializeField]
        private UnityEvent OnDisable = new UnityEvent();
        public void SetDisable() => OnDisable.Invoke();
    }
}