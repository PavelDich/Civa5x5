using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using Zenject;
using Random = UnityEngine.Random;

namespace Minicop.Game.GravityRave
{
    public class Tilegrid : NetworkBehaviour
    {
        private void Awake()
        {
            if (NetworkServer.active) SrvGenerate(_gridHeight, _gridWidth);
            if (NetworkClient.active)
                Generate(_gridHeight, _gridWidth);
        }

        [Server]
        public void SrvClear()
        {
            _gridHeight = 0;
            _gridWidth = 0;
            for (int x = 0; x < Tiles.Count; x++)
            {
                for (int y = 0; y < Tiles[x].Count; y++)
                {
                    if (Tiles[x][y]) NetworkServer.Destroy(Tiles[x][y].gameObject);
                }
            }
            Tiles.Clear();
            for (int i = 0; i < _networkLevel.Connections.Count; i++)
                RpcClear(_networkLevel.Connections[i].connectionToClient);
        }
        [TargetRpc]
        private void RpcClear(NetworkConnection connection)
        {
            Tiles.Clear();
        }


        [Server]
        public void SrvGenerate(int height, int width)
        {
            SrvClear();
            _gridHeight = height;
            _gridWidth = width;
            Generate(height, width);
            for (int i = 0; i < _networkLevel.Connections.Count; i++)
                RpcGenerate(_networkLevel.Connections[i].connectionToClient, height, width);
        }
        [TargetRpc]
        private void RpcGenerate(NetworkConnection connection, int height, int width)
        {
            Generate(height, width);
        }
        private void Generate(int height, int width)
        {
            for (int x = 0; x < width; x++)
            {
                Tiles.Add(new List<Tile>());
                for (int y = 0; y < height; y++)
                {
                    Tiles[x].Add(null);
                }
            }
        }


        private void Start()
        {
            if (NetworkClient.active) SyncTilegrid();
        }



        public UnityEvent<Tile> OnTileSet = new UnityEvent<Tile>();

        [Inject]
        public DiContainer _diContainer;
        [SerializeField]
        private TileVariants _tileVariants;
        [Inject]
        private NetworkLevel _networkLevel;
        [SerializeField]
        private float _detectClickDistance = 100f;
        [SerializeField]
        private LayerMask _gridLayer;

        [SerializeField]
        private Tilemap _grid;
        [SerializeField]
        public List<List<Tile>> Tiles = new List<List<Tile>>();
        [SyncVar]
        [SerializeField]
        [Range(1, 999)]
        private int _gridHeight;
        [SyncVar]
        [SerializeField]
        [Range(1, 999)]
        private int _gridWidth;

        public void SyncTilegrid() => CmdSyncTilegrid(NetworkLevel.LocalConnection);
        [Command(requiresAuthority = false)]
        public void CmdSyncTilegrid(NetworkIdentity conn)
        {
            for (int x = 0; x < Tiles.Count; x++)
            {
                for (int y = 0; y < Tiles[x].Count; y++)
                {
                    NetworkIdentity networkIdentity = null;
                    if (Tiles[x][y]) networkIdentity = Tiles[x][y].GetComponent<NetworkIdentity>();
                    RpcSetTile(conn.connectionToClient, networkIdentity, new Vector3Int(x, y, 0) - new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0));
                }
            }
        }


        [Server]
        public Tile SrvSetTile(int tileId, Vector3Int position)
        {
            Vector3Int tileCell = position + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);

            if (tileCell.x < 0 || tileCell.x >= Tiles.Count) return null;
            if (tileCell.y < 0 || tileCell.y >= Tiles[tileCell.x].Count) return null;

            if (Tiles[tileCell.x][tileCell.y])
            {
                NetworkServer.Destroy(Tiles[tileCell.x][tileCell.y].gameObject);
            }

            if (TileVariants.Tiles[tileId])
            {
                TileVariants tileVariants = _diContainer.InstantiatePrefab(_tileVariants, this.transform).GetComponent<TileVariants>();
                NetworkServer.Spawn(tileVariants.gameObject);
                Tile tile = tileVariants.Spawn(tileId);

                //tile.transform.rotation = Quaternion.Euler(0, 60 * Random.Range(0, 5), 0);
                Tiles[tileCell.x][tileCell.y] = tile;
                //tile.transform.position = _grid.GetCellCenterWorld(position);
                tile.Grid = this;
                tile.Position = position;
                OnTileSet.Invoke(tile);

                for (int i = 0; i < _networkLevel.Connections.Count; i++)
                    RpcSetTile(_networkLevel.Connections[i].connectionToClient, tile.GetComponent<NetworkIdentity>(), position);
                return tile;
            }
            else
                for (int i = 0; i < _networkLevel.Connections.Count; i++)
                    RpcSetTile(_networkLevel.Connections[i].connectionToClient, null, position);
            return null;
        }
        [TargetRpc]
        public void RpcSetTile(NetworkConnection connection, NetworkIdentity tileIdentity, Vector3Int position)
        {
            Vector3Int tileCell = position + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileCell.x < 0 || tileCell.x >= Tiles.Count) return;
            if (tileCell.y < 0 || tileCell.y >= Tiles[tileCell.x].Count) return;

            if (!tileIdentity) return;
            Tile tile = tileIdentity.GetComponent<Tile>();
            tile.transform.rotation = Quaternion.Euler(0, 60 * Random.Range(0, 5), 0);
            Tiles[tileCell.x][tileCell.y] = tile;
            tile.transform.position = _grid.GetCellCenterWorld(position);

            tile.Grid = this;
            tile.Position = position;
        }


        [Server]
        public void SrvMoveTile(Vector3Int position, Vector3Int targetPosition)
        {
            if (GetTileFromCell(targetPosition)) return;

            Vector3Int tileCell = position + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileCell.x < 0 || tileCell.x >= Tiles.Count) return;
            if (tileCell.y < 0 || tileCell.y >= Tiles[tileCell.x].Count) return;

            Vector3Int tileTargetCell = targetPosition + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileTargetCell.x < 0 || tileTargetCell.x >= Tiles.Count) return;
            if (tileTargetCell.y < 0 || tileTargetCell.y >= Tiles[tileCell.x].Count) return;

            Tile tile = GetTileFromCell(position);

            Tiles[tileTargetCell.x][tileTargetCell.y] = Tiles[tileCell.x][tileCell.y];
            Tiles[tileTargetCell.x][tileTargetCell.y].Position = targetPosition;

            Tiles[tileCell.x][tileCell.y] = null;

            for (int i = 0; i < _networkLevel.Connections.Count; i++)
                RpcMoveTile(_networkLevel.Connections[i].connectionToClient, tile.GetComponent<NetworkIdentity>(), position, targetPosition);
        }
        /*
        [Server]
        public void SrvMoveTile(Tile tile, Vector3Int targetPosition)
        {
            for (int i = 0; i < _networkLevel.Connections.Count; i++)
                RpcSetTile(_networkLevel.Connections[i].connectionToClient, tile.GetComponent<NetworkIdentity>(), targetPosition);
        }*/
        [TargetRpc]
        public void RpcMoveTile(NetworkConnection connection, NetworkIdentity tileIdentity, Vector3Int position, Vector3Int targetPosition)
        {
            Vector3Int tileCell = position + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileCell.x < 0 || tileCell.x >= Tiles.Count) return;
            if (tileCell.y < 0 || tileCell.y >= Tiles[tileCell.x].Count) return;

            Vector3Int tileTargetCell = targetPosition + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileTargetCell.x < 0 || tileTargetCell.x >= Tiles.Count) return;
            if (tileTargetCell.y < 0 || tileTargetCell.y >= Tiles[tileCell.x].Count) return;

            Tiles[tileTargetCell.x][tileTargetCell.y] = Tiles[tileCell.x][tileCell.y];
            Tiles[tileTargetCell.x][tileTargetCell.y].Position = targetPosition;

            Tiles[tileTargetCell.x][tileTargetCell.y].transform.rotation = Quaternion.Euler(0, 60 * Random.Range(0, 5), 0);
            Tiles[tileTargetCell.x][tileTargetCell.y].transform.position = _grid.GetCellCenterWorld(targetPosition);

            Tiles[tileCell.x][tileCell.y] = null;

        }





        public Vector3Int GetCellFromPosition(Vector3 position)
        {
            return _grid.WorldToCell(position);
        }

        public Vector3Int GetCellFromClickPosition(Camera camera)
        {
            RaycastHit raycastHit;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, _detectClickDistance, _gridLayer))
            {
                return _grid.WorldToCell(raycastHit.point);
            }
            return _grid.WorldToCell(ray.GetPoint(_detectClickDistance));
        }

        public Tile GetTileFromCell(Vector3Int position)
        {
            Vector3Int tileCell = position + new Vector3Int(_gridWidth / 2, _gridHeight / 2, 0);
            if (tileCell.x < 0 || tileCell.x >= Tiles.Count) return null;
            if (tileCell.y < 0 || tileCell.y >= Tiles[tileCell.x].Count) return null;
            return Tiles[tileCell.x][tileCell.y];
        }
        public Vector3Int GetPositionTile()
        {
            return Vector3Int.zero;
        }
    }
}