using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;


namespace Minicop.Game.GravityRave
{
    public class TileVariants : NetworkBehaviour
    {
        protected override void OnValidate()
        {
            Tiles = _tiles.Distinct().ToArray(); ;
            for (int i = 0; i < Tiles.Length; i++)
            {
                if (Tiles[i]) Tiles[i].Id = i;
            }
            _tiles = Tiles;
            base.OnValidate();
        }
        [SerializeField]
        private Tile[] _tiles;
        public static Tile[] Tiles;

        public Tile Spawn(int id)
        {
            Tile tile = Instantiate(Tiles[id], this.transform);
            tile.transform.SetParent(null);
            NetworkServer.Spawn(tile.gameObject);
            NetworkServer.Destroy(this.gameObject);
            return tile;
        }
    }
}