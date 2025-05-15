using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minicop.Game.GravityRave
{
    [CreateAssetMenu(fileName = "MenuField", menuName = "Game/Menu", order = 1)]
    [System.Serializable]
    public class MenuField : ScriptableObject
    {
        public Field field;
        [System.Serializable]
        public struct Field
        {
            public Game game;
            [System.Serializable]
            public struct Game
            {
                public string IpAdress;
                public string PortAdress;
            }
            public Settings settings;
            [System.Serializable]
            public struct Settings
            {
                public Player player;
                [System.Serializable]
                public struct Player
                {
                    public string Name;
                }
            }
        }
    }
}