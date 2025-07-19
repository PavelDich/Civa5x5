using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    public class ChatMessage : MonoBehaviour
    {
        public void SetMassage(string text) => OnMassageSet.Invoke(text);
        [SerializeField]
        public UnityEvent<string> OnMassageSet = new UnityEvent<string>();
    }
}