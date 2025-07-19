using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Minicop.Game.GravityRave
{
    public class Tile : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        public int Id;
        [HideInInspector]
        [SerializeField]
        public Tilegrid Grid;
        [HideInInspector]
        [SerializeField]
        public Vector3Int Position;
    }
}