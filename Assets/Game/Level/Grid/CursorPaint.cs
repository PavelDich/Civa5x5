using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

namespace Minicop.Game.GravityRave
{
    public class CursorPaint : NetworkBehaviour
    {
        public GridMapVariant gridMapVariant;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private Tilegrid _grid;
        [SerializeField]
        private GridMapLoader _gridMapLoader;

        [SerializeField]
        private Tile _selectedTile;
        [SerializeField]
        private Tilegrid _paintGrid;

        public UnityEvent<Camera> OnClick = new UnityEvent<Camera>();

#if UNITY_EDITOR
        private void Update()
        {
            if (!NetworkClient.active) return;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                CmdSetTile(_selectedTile.Id, _grid.GetCellFromClickPosition(_camera));
            }

            if (Input.GetKeyDown(KeyCode.Equals))
            {
                if (_selectedTile.Id + 1 >= TileVariants.Tiles.Length)
                    _selectedTile = TileVariants.Tiles[0];
                else
                    _selectedTile = TileVariants.Tiles[_selectedTile.Id + 1];
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                if (_selectedTile.Id - 1 < 0)
                    _selectedTile = TileVariants.Tiles[TileVariants.Tiles.Length - 1];
                else
                    _selectedTile = TileVariants.Tiles[_selectedTile.Id - 1];
            }

            if (Input.GetKey(KeyCode.Z))
            {
                CmdSave();
            }


            if (Input.GetKey(KeyCode.C))
            {
                CmdLoad();
            }


            if (Input.GetKey(KeyCode.Space))
            {
                CmdGenerate(10, 10);
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdGenerate(int x, int y) => _grid.SrvGenerate(x, y);
        [Command(requiresAuthority = false)]
        public void CmdLoad() => _gridMapLoader.SrvLoad(gridMapVariant,_paintGrid);
        [Command(requiresAuthority = false)]
        public void CmdSave() => _gridMapLoader.SrvSave(gridMapVariant,_paintGrid);
        [Command(requiresAuthority = false)]
        private void CmdSetTile(int tileId, Vector3Int position)
        {
            _grid.SrvSetTile(tileId, position);
        }
#endif
    }
}