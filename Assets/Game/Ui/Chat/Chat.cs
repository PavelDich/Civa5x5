using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    public class Chat : NetworkBehaviour
    {
        public SyncList<string> Messages = new SyncList<string>();


        private List<ChatMessage> SpawnedMassages = new List<ChatMessage>();
        [SerializeField]
        private ChatMessage _chatMessageVatiant;
        [SerializeField]
        private Transform _chatMessageParent;

        public override void OnStartClient()
        {
            Messages.OnAdd += AddMassage;
            Messages.OnInsert += AddMassage;
        }
        public override void OnStopClient()
        {
            Messages.OnAdd -= AddMassage;
            Messages.OnInsert -= AddMassage;
        }

        private void AddMassage(int id)
        {
            ChatMessage chatMessage = Instantiate(_chatMessageVatiant, _chatMessageParent);
            chatMessage.SetMassage(Messages[id]);
            SpawnedMassages.Add(chatMessage);
        }
        private void OnEnable()
        {
            for (int i = 0; i < SpawnedMassages.Count; i++)
                Destroy(SpawnedMassages[i].gameObject);
            SpawnedMassages.Clear();

            for (int i = 0; i < Messages.Count; i++)
                AddMassage(i);
        }

        private string _massage;
        public void SetMassage(string text) => _massage = $"{NetworkLevel.LocalConnection.GetComponent<ConnectionInfo>().Name}: {text}";
        public void SendMassage() => CmdSendMassage(_massage);
        [Command(requiresAuthority = false)]
        public void CmdSendMassage(string text)
        {
            Messages.Add(text);
        }
    }
}