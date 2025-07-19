using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using ModestTree;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    public class GridMapLoader : NetworkBehaviour
    {
        [Server]
        public void SrvLoad(GridMapVariant gridMapVariant, Tilegrid grid)
        {
            grid.SrvGenerate(gridMapVariant.Height, gridMapVariant.Width);

            for (int x = 0; x < gridMapVariant.Width; x++)
            {
                for (int y = 0; y < gridMapVariant.Height; y++)
                    if (gridMapVariant.Tiles[x * gridMapVariant.Height + y] != -1)
                    {
                        grid.SrvSetTile(gridMapVariant.Tiles[x * gridMapVariant.Height + y],
                            new Vector3Int(x, y, 0) - new Vector3Int(
                                gridMapVariant.Width / 2,
                                 gridMapVariant.Height / 2,
                                0));
                    }
            }
        }
        [Server]
        public void SrvSave(GridMapVariant gridMapVariant, Tilegrid grid)
        {
            if (grid.Tiles.Count > 0)
                if (grid.Tiles[0].Count > 0)
                {
                    gridMapVariant.Height = grid.Tiles[0].Count;
                    gridMapVariant.Width = grid.Tiles.Count;
                    gridMapVariant.Tiles.Clear();
                    for (int x = 0; x < grid.Tiles.Count; x++)
                    {
                        for (int y = 0; y < grid.Tiles[x].Count; y++)
                            if (grid.Tiles[x][y])
                            {
                                gridMapVariant.Tiles.Add(grid.Tiles[x][y].Id);
                            }
                            else
                            {
                                gridMapVariant.Tiles.Add(-1);
                            }
                    }
                }
        }
    }
}