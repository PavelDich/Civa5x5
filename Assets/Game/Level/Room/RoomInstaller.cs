using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minicop.Game.GravityRave
{
    public class RoomInstaller : MonoInstaller
    {
        //[SerializeField]
        //private RoundManager _roundManager;
        [SerializeField]
        private RoundController _roundController;
        public override void InstallBindings()
        {
            Container.Bind<RoundController>().FromComponentInHierarchy(_roundController).AsSingle().NonLazy();
            //Container.Bind<NeedObj>().FromComponentInHierarchy(_needObj).AsSingle();
            //Container.Bind<NetworkManager>().FromComponentInHierarchy(_networkManager).AsSingle();
        }
    }
}
