using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    public class Unit : NetworkBehaviour
    {
        public void OnDestroy()
        {
            _roundController.OnStep.RemoveListener(Step);
        }

        [SyncVar]
        public int TeamId;
        private RoundController _roundController;
        public RoundController RoundController
        {
            get
            {
                return _roundController;
            }
            set
            {
                _roundController = value;
            }
        }
        public UnityEvent<Material> OnMaterialChange = new UnityEvent<Material>();

        [Client]
        public void Init(RoundController roundController, int teamId)
        {
            RoundController = roundController;
            OnMaterialChange.Invoke(RoundController.TeamMaterials[teamId]);
        }

        public UnityEvent OnSelect = new UnityEvent();
        public void Select()
        {
            OnSelect.Invoke();
        }


        public UnityEvent<Vector3Int> OnDeselect = new UnityEvent<Vector3Int>();
        public void Deselect(Vector3Int position)
        {
            OnDeselect.Invoke(position);
        }


        public UnityEvent<NetworkIdentity> OnStep = new UnityEvent<NetworkIdentity>();
        public void Step(NetworkIdentity owner)
        {
            OnStep.Invoke(owner);
        }
    }
}