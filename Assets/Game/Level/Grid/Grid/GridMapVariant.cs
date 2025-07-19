using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Minicop.Game.GravityRave
{
    [CreateAssetMenu(fileName = "New GridMap", menuName = "Game/Grid/MapVariant", order = -1)]
    public class GridMapVariant : ScriptableObject
    {
        public int Height;
        public int Width;
        public string Name = "";
        public List<int> Tiles = new List<int>();
    }
}