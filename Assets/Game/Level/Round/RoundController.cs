using System.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

namespace Minicop.Game.GravityRave
{
    public class RoundController : NetworkBehaviour
    {
        private void Start()
        {
            if (NetworkClient.active)
                ChangeMap(SelectedMap);
        }
        [Inject]
        private NetworkLevel _networkLevel;
        public GridMapLoader GridTerrainMapLoader;
        public Tilegrid GridTerrain;
        public Tilegrid GridUnits;
        [SyncVar]
        public bool IsStarted;
        public Tile BaseUnit;

        [field: SerializeField]
        public Map[] Maps;
        [Serializable]
        public struct Map
        {
            public GridMapVariant gridMapVariant;
            public Transform[] Spawns;
        }

        [SyncVar]
        private int _teamMove;
        public int TeamMove
        {
            get
            {
                return _teamMove;
            }
            set
            {
                if (value >= Teams.Count)
                    _teamMove = 0;
                else
                    _teamMove = value;
            }
        }
        [field: SerializeField]
        public SyncList<Team> Teams = new SyncList<Team>();
        [Serializable]
        public struct Team
        {
            public NetworkIdentity Owner;
            public int MaterialId;
        }
        public Material[] TeamMaterials;

        [SyncVar]
        public int SelectedMap = 0;
        public void ChangeMap(int id) => CmdChangeMap(id);
        [Command(requiresAuthority = false)]
        public void CmdChangeMap(int id)
        {
            if (IsStarted) return;
            SelectedMap = id;
            GridTerrainMapLoader.SrvLoad(Maps[id].gridMapVariant, GridTerrain);
            GridUnits.SrvGenerate(Maps[id].gridMapVariant.Height, Maps[id].gridMapVariant.Width);
        }

        public UnityEvent OnStartRound = new UnityEvent();
        public void StartRound()
        {
            CmdStartRound();
        }
        [Command(requiresAuthority = false)]
        public void CmdStartRound()
        {
            if (IsStarted) return;
            ChangeMap(SelectedMap);

            IsStarted = true;

            List<Transform> spawns = new List<Transform>();
            Maps[SelectedMap].Spawns.CopyTo(spawns);
            for (int i = 0; i < _networkLevel.Connections.Count; i++)
            {
                Teams.Add(new Team()
                {
                    Owner = _networkLevel.Connections[i],
                    MaterialId = i,
                });
                int randomSpawn = Random.Range(0, spawns.Count - 1);
                Unit unit = GridUnits.SrvSetTile(BaseUnit.Id, GridUnits.GetCellFromPosition(spawns[randomSpawn].position)).GetComponent<Unit>();
                spawns.Remove(spawns[randomSpawn]);

                unit.RoundController = this;
                unit.TeamId = i;
                OnStep.AddListener(unit.Step);

                foreach (NetworkIdentity networkIdentity in _networkLevel.Connections)
                    SyncUnitMaterial(networkIdentity.connectionToClient, unit.GetComponent<NetworkIdentity>(),
                    this.GetComponent<NetworkIdentity>(), i);
            }
            OnStartRound.Invoke();
        }
        [TargetRpc]
        void SyncUnitMaterial(NetworkConnectionToClient networkConnection, NetworkIdentity unit, NetworkIdentity roundController, int teamId)
        {
            unit.GetComponent<Unit>().Init(roundController.GetComponent<RoundController>(), teamId);
        }


        [SerializeField]
        private Camera _raycastCamera;
        private Unit _selectedUnit;
        public void Select()
        {
            if (!IsStarted) return;
            Vector3Int cell = GridUnits.GetCellFromClickPosition(_raycastCamera);
            Tile tile = GridUnits.GetTileFromCell(cell);
            if (!tile)
            {
                if (_selectedUnit)
                    _selectedUnit.GetComponent<Unit>().Deselect(cell);
                _selectedUnit = null;
                return;
            }
            Unit unit = tile.GetComponent<Unit>();
            if (NetworkLevel.LocalConnection != Teams[unit.TeamId].Owner) return;
            _selectedUnit = unit;
            _selectedUnit.Select();
        }

        [Client]
        public void Step()
        {
            CmdStep(NetworkLevel.LocalConnection);
        }
        [Command(requiresAuthority = false)]
        public void CmdStep(NetworkIdentity networkIdentity)
        {
            if (!IsStarted) return;
            if (Teams[TeamMove].Owner != networkIdentity) return;
            TeamMove++;
            OnStep.Invoke(networkIdentity);
        }
        public UnityEvent<NetworkIdentity> OnStep = new UnityEvent<NetworkIdentity>();
    }
}