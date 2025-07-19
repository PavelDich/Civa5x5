using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class Walker : NetworkBehaviour
    {
        [SerializeField]
        private Tile[] Obstacles;
        [SerializeField]
        private Unit _unit;
        public void Start()
        {
            _unit = GetComponent<Unit>();
        }
        public Vector3Int NextStep;
        [Server]
        public void SrvStep(NetworkIdentity owner)
        {
            Unit unit = GetComponent<Unit>();
            if (unit.RoundController.Teams[unit.TeamId].Owner == owner)
                RpcStep(unit.RoundController.Teams[unit.TeamId].Owner.connectionToClient);
        }
        [TargetRpc]
        public void RpcStep(NetworkConnection connection)
        {
            CmdStep(NextStep);
        }
        [Command(requiresAuthority = false)]
        public void CmdStep(Vector3Int position)
        {
            List<int> obstacles = Obstacles.ToList().Select(x => x.Id).ToList();
            int id = _unit.RoundController.GetComponent<RoundController>().GridTerrain.GetTileFromCell(position).Id;
            if (obstacles.Contains(id)) return;
            Tile tile = GetComponent<Tile>();
            tile.Grid.SrvMoveTile(tile.Position, position);
        }

        public void SetStep(Vector3Int position)
        {
            List<int> obstacles = Obstacles.ToList().Select(x => x.Id).ToList();
            Tile tile = _unit.RoundController.GetComponent<RoundController>().GridTerrain.GetTileFromCell(position);
            if (!tile) return;
            if (obstacles.Contains(tile.Id)) return;
            NextStep = position;
        }
    }
}