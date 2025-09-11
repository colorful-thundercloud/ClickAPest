using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item 001", menuName = "Scriptable Objects/Create item")]
[Serializable]
public class ItemData : IngameObjectData
{
    [SerializeField] public GameObject inGamePrefab;
    public int price;
    public int lifetimeSeconds;
    public bool targetsSelf;
    public TargetType targetsWho;
    public ItemType type;
    public SpawnLocation spawnLocation;
    public List<ExtraSpawnedObjects> extraSpawnedObjects;

    [Serializable]
    public enum TargetType
    {
        Pests, Catchers, All
    }

    [Serializable]
    public enum ItemType
    {
        Damaging, Defending, Buff, Debuff, Other
    }

    [Serializable]
    public enum SpawnLocation
    {
        PlayerLocation, PlayerLocationAndPlayerAsParent, MouseCursor, MapCenter
    }

    [Serializable]
    public struct ExtraSpawnedObjects
    {
        [SerializeField] public GameObject spawnThisObject;
        public float delayBeforeThisObjectSpawns;
        public SpawnType spawnType;
        public float spawnThisManyCopiesOfThisObject;

        [Serializable]
        public enum SpawnType
        {
            Once, SpawnExactObjectsNumber, SpawnInfinetelyOnceEveryNSeconds
        }
    }
}
