using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Minicop.Game.GravityRave
{
    public class RoomPanel : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<string> OnNameSet = new UnityEvent<string>();
        [SerializeField]
        private UnityEvent OnPasswordNone = new UnityEvent();
        [SerializeField]
        private UnityEvent OnPasswordSet = new UnityEvent();
        [SerializeField]
        private UnityEvent<string> OnOnlineSet = new UnityEvent<string>();

        public LobbyUiController LobbyUiController;
        public int RoomId;

        public void Connect()
        {
            LobbyUiController.Connect(RoomId);
        }

        public void SetName(string name)
        {
            OnNameSet.Invoke(name);
        }
        public void SetPassword(bool isHavePassword)
        {
            if (isHavePassword) OnPasswordSet.Invoke();
            else OnPasswordNone.Invoke();
        }
        public void SetOnline(string online)
        {
            OnOnlineSet.Invoke(online);
        }
    }
}