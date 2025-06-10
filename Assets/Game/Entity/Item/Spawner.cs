using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

namespace Minicop.Game.GravityRave
{
    public class Spawner : NetworkBehaviour
    {
        [SerializeField]
        private SpawnType _spawnType;
        public enum SpawnType
        {
            Points,
            Radius,
            RadiusOnNavMash,
        }
        [SerializeField]
        private Transform[] _spawnPoints;
        public float Radius;
        [Inject]
        public DiContainer _diContainer;
        [SerializeField]
        private Variant[] Variants;
        [Serializable]
        public struct Variant
        {
            public NetworkIdentity Object;
            public int MaxCount;
            public int MinCount;
        }
        [SyncVar]
        public SyncList<NetworkIdentity> SpawnedObjects;
        public UnityEvent<NetworkIdentity> OnObjectSpawned = new UnityEvent<NetworkIdentity>();
        //Add event class OnDestroy
        public UnityEvent<NetworkIdentity> OnObjectDestroy = new UnityEvent<NetworkIdentity>();


        public void Spawn(NetworkIdentity owner)
        {
            CmdSpawn(owner);
            [Command(requiresAuthority = false)]
            void CmdSpawn(NetworkIdentity owner)
            {
                SrvSpawn(owner);
            }
        }
        [Server]
        public void SrvSpawn(NetworkIdentity owner)
        {
            for (int i = 0; i < Variants.Length; i++)
            {
                int gen = Random.Range(Variants[i].MinCount, Variants[i].MaxCount);
                for (int j = 0; j < gen; j++)
                {
                    Vector3 spawn = Vector3.zero;
                    switch (_spawnType)
                    {
                        case SpawnType.Points:
                            spawn = RandomPoints(Radius);
                            break;
                        case SpawnType.Radius:
                            spawn = RandomRadius(Radius);
                            break;
                        case SpawnType.RadiusOnNavMash:
                            spawn = RandomNavSphere(Radius);
                            break;
                        default:
                            break;
                    }
                    GameObject go = _diContainer.InstantiatePrefab(Variants[i].Object, spawn, Quaternion.identity, this.transform);
                    go.transform.SetParent(null);
                    if (owner) NetworkServer.Spawn(go, owner.connectionToClient);
                    else NetworkServer.Spawn(go);
                    SpawnedObjects.Add(go.GetComponent<NetworkIdentity>());
                    OnObjectSpawned.Invoke(go.GetComponent<NetworkIdentity>());
                }
            }
        }

        public Vector3 RandomNavSphere(float distance)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            randomDirection += _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, distance, -1);
            return navHit.position;
        }

        public Vector3 RandomRadius(float distance)
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
            randomDirection += _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
            return randomDirection;
        }
        public Vector3 RandomPoints(float distance)
        {
            return _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;
        }

        /*
                public IEnumerator WaitSpawnPlayer()
                {
                    yield return new WaitForEndOfFrame();
                    FindPlayers();
                    SpawnPlayer();
                }
                public void FindPlayers()
                {
                    if (!NetworkClient.active) return;
                    CmdFindPlayers();
                }
                [Command(requiresAuthority = false)]
                public void CmdFindPlayers()
                {
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject p in players)
                    {
                        NetworkIdentity networkIdentity = p.GetComponent<NetworkIdentity>();
                        RpcPlayerEnter(networkIdentity.connectionToClient);
                        RpcFindLocalPlayers(networkIdentity.connectionToClient, networkIdentity);
                    }
                }
                [Client]
                public void SpawnPlayer()
                {
                    if (NetworkClient.active)
                    {
                        CmdSpawnPlayer(LocalConnection, SceneId);
                    }
                }
                [Command(requiresAuthority = false)]
                private void CmdSpawnPlayer(NetworkIdentity owner, int id)
                {
                    Vector3 spawn = _spawns[Random.Range(0, _spawns.Length - 1)].position;
                    NetworkIdentity go = _diContainer.InstantiatePrefab(Player, spawn, Quaternion.identity, null).GetComponent<NetworkIdentity>();
                    SceneManager.MoveGameObjectToScene(go.gameObject, _networkManager.ActiveRooms[id]);

                    NetworkServer.Spawn(go.gameObject, owner.connectionToClient);
                    Players.Add(go);
                }
                [TargetRpc]
                private void RpcSpawnPlayer(NetworkConnection conn, NetworkIdentity player)
                {
                    OnPlayerSpawned.Invoke(player);
                }
                */
    }
}