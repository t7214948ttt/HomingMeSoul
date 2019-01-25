﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AngerStudio.HomingMeSoul.Game
{

    public class GameCore : MonoBehaviour
    {
        public GameObject gravityZonePrefab;
        public GameObjectArrayReference gravityZones;

        public GameConfigReference config;

        public int[] suppliesCountOfZones;

        public void Prepare ()
        {
            suppliesCountOfZones = new int[config.Value.gravityZoneSteps.Length - 1];
        }

        public int SpawnSupplyInMostEmptyZone (SupplyType type, int supplyLevel)
        {
            int emptyZoneIndex = 0, lastLeast = 100;
            for (int i = 0; i < config.Value.gravityZoneSteps.Length - 1; i++)
            {
                if (suppliesCountOfZones[i] > lastLeast)
                {
                    lastLeast = suppliesCountOfZones[i];
                    emptyZoneIndex = i;
                }                
            }

            PlaceSupply(emptyZoneIndex, type, supplyLevel);
            return emptyZoneIndex;            
        }

        public int SpawnSupplyInRandomZone (SupplyType type, int supplyLevel)
        {
            int t = Random.Range(1, gravityZones.Value.Length);
            PlaceSupply(t, gravityZones.Value[t].transform, type, supplyLevel);
            return t;
        }

        public void PlaceSupply (int zoneIndex, SupplyType type, int supplyLevel)
        {
            PlaceSupply(Random.Range(config.Value.gravityZoneSteps[zoneIndex - 1], config.Value.gravityZoneSteps[zoneIndex]), gravityZones.Value[zoneIndex].transform, type, supplyLevel);
        }

        public void PlaceSupply (float distance, Transform parentZone, SupplyType type, int supplyLevel)
        {
            GameObject t = null;
            switch (type)
            {
                case SupplyType.Money:
                    t = GameObject.Instantiate(config.Value.moneySupplyPrefabs[supplyLevel]);
                    break;
                case SupplyType.Book:
                    t = GameObject.Instantiate(config.Value.bookSupplyPrefabs[supplyLevel]);
                    break;
                case SupplyType.Food:
                    t = GameObject.Instantiate(config.Value.foodSupplyPrefabs[supplyLevel]);
                    break;
            }
            
            t.transform.SetParent(parentZone);
            t.transform.localPosition = Vector3.zero;
            t.transform.position = parentZone.position + Quaternion.Euler(0, 0, Random.Range(0f, 359.9f)) * Vector2.left * distance;
        }
    }
}