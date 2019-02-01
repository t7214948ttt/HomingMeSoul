﻿using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BA_Studio.StatePattern;
using BA_Studio.UnityLib.GameObjectPool;
using BA_Studio.DataStructure;
using AngerStudio.HomingMeSoul.Core;
using BA_Studio.UnityLib.SingletonLocator;

namespace AngerStudio.HomingMeSoul.Game
{

    public class GameCore : MonoBehaviour
    {
        public static GameCore Instance { get => SingletonBehaviourLocator<GameCore>.Instance; }
        StateMachine<GameCore> stateMachine;
        public GameObject gravityZonePrefab;
        public GameObjectArrayReference gravityZones;
        public GameConfigReference config;
        public IntReference score, sp;
        public HashSet<SupplyDrop>[] pickUpsInZones;

        GameObjectPool<SupplyDrop> dropsPool;

        Dictionary<SupplyDrop, int> zoneIndexMap = new Dictionary<SupplyDrop, int>();
        

        //playerIndex -> pickups
        BiMap<int, HashSet<SupplyDrop>> pickUpInstances;
        float[] zoneWeights;

        public GameObject burstVFXPrefab;

        float poolDepth = 0;

        public GameObject c1,c2,c3;
        public GameObjectReference[] Characters;
        public FloatReference[] CharacterStamina;

        Dictionary<KeyCode, CharacterProperty> Players;

        

        public ScoreBase scoreBase;

        public void CreaterPlayers()
        {
            int i = 0;
            Vector2[] positions = GetSpawnPosition(AppCore.Instance.activePlayers.Count);

            foreach(var player in AppCore.Instance.activePlayers)
            {
                GameObject prefab;

                switch (player.Value.assginedPickupType)
                {
                    case 0:
                    case 1:
                    case 2:
                        prefab = c1;
                        break;
                    case 3:
                    case 4:
                    case 5:
                        prefab = c2;
                        break;
                    case 6:
                    case 7:
                    case 8:
                        prefab = c3;
                        break;
                    default:
                        prefab = c1;
                        break;
                }


                //Initialize character
                GameObject temp = Instantiate(prefab, positions[i], Quaternion.identity);
                Characters[i].Value = temp;

                CharacterProperty character = temp.GetComponent<CharacterProperty>();
                character.typeIndex = player.Value.assginedPickupType;
                character.setColor(player.Value.assignedColor);
                character.m_key = player.Key;
                character.playerIndex = player.Value.UsingPlayerSlot;
                character.Stamina = CharacterStamina[i];
                InitializeCharacter(character);
                

                Players.Add(player.Key, character);
                listPlayerInHome.Add(player.Key);

                i++;
            }
        }

        void InitializeCharacter(CharacterProperty character)
        {
            character.collideLocation = scoreBase.gameObject;
            character.Ready = true;
            character.Stamina.Value = config.Value.staminaChargeNumber;
            character.faceLocation();
        }

        Vector2[] GetSpawnPosition(int playerNumber)
        {
            int radius = 1;
            Vector2[] returnVectors;

            switch(playerNumber)
            {
                case 1:
                    returnVectors = new Vector2[]{new Vector2(0,radius)};
                    break;
                case 2:
                    returnVectors = new Vector2[] { new Vector2(0, radius),new Vector2(0, -radius) };
                    break;
                case 3:
                    returnVectors = new Vector2[] {new Vector2(0, radius), new Vector2(radius*Mathf.Cos(210 * Mathf.Deg2Rad), radius*Mathf.Sin(210*Mathf.Deg2Rad)),new Vector2(radius * Mathf.Cos(330 * Mathf.Deg2Rad), radius * Mathf.Sin(330 * Mathf.Deg2Rad)) };
                    break;
                case 4:
                    returnVectors = new Vector2[] { new Vector2(0, radius),new Vector2(-radius,0),new Vector2(0, -radius),new Vector2(radius,0) };
                
                    break;
                case 5:
                    returnVectors = new Vector2[] { new Vector2(0, radius), new Vector2(radius * Mathf.Cos(162 * Mathf.Deg2Rad), radius * Mathf.Sin(162 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(234 * Mathf.Deg2Rad), radius * Mathf.Sin(234 * Mathf.Deg2Rad)),new Vector2(radius * Mathf.Cos(306 * Mathf.Deg2Rad), radius * Mathf.Sin(306 * Mathf.Deg2Rad)),new Vector2(radius * Mathf.Cos(18 * Mathf.Deg2Rad), radius * Mathf.Sin(18 * Mathf.Deg2Rad)) };
                    break;
                case 6:
                    returnVectors = new Vector2[] { new Vector2(0, radius), new Vector2(radius * Mathf.Cos(150 * Mathf.Deg2Rad), radius * Mathf.Sin(150 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(210 * Mathf.Deg2Rad), radius * Mathf.Sin(210 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(270 * Mathf.Deg2Rad), radius * Mathf.Sin(270 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(330 * Mathf.Deg2Rad), radius * Mathf.Sin(330 * Mathf.Deg2Rad)),new Vector2(radius * Mathf.Cos(30 * Mathf.Deg2Rad), radius * Mathf.Sin(30 * Mathf.Deg2Rad)) };
                   break;
                case 7:
                    returnVectors = new Vector2[] { new Vector2(0, radius), new Vector2(radius * Mathf.Cos(141 * Mathf.Deg2Rad), radius * Mathf.Sin(141 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(192 * Mathf.Deg2Rad), radius * Mathf.Sin(192 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(243 * Mathf.Deg2Rad), radius * Mathf.Sin(243 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(294 * Mathf.Deg2Rad), radius * Mathf.Sin(294 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(345 * Mathf.Deg2Rad), radius * Mathf.Sin(345 * Mathf.Deg2Rad)),new Vector2(radius * Mathf.Cos(39 * Mathf.Deg2Rad), radius * Mathf.Sin(39 * Mathf.Deg2Rad)) };
                    break;
                case 8:
                    returnVectors = new Vector2[] { new Vector2(0, radius), new Vector2(-radius, 0), new Vector2(0, -radius), new Vector2(radius, 0),new Vector2(radius * Mathf.Cos(135 * Mathf.Deg2Rad), radius * Mathf.Sin(135 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(225 * Mathf.Deg2Rad), radius * Mathf.Sin(225 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(315 * Mathf.Deg2Rad), radius * Mathf.Sin(315 * Mathf.Deg2Rad)), new Vector2(radius * Mathf.Cos(45 * Mathf.Deg2Rad), radius * Mathf.Sin(45 * Mathf.Deg2Rad)) };
                    break;
                default:
                    returnVectors = new Vector2[1];
                    break;
            }
            

            return returnVectors;
        }

        public void ReceieveInput()
        {
            foreach(var key in Players.Keys)
            {
                if (SimpleInput.GetKeyDown(key))
                {
                    if (Players[key].Ready)
                    {
                        Shoot(key);
                    }
                }
            }
        }

        public void CharacterGoHome (KeyCode key)
        {
            if (!listPlayerMoving.Contains(key)) return;
            listPlayerMoving.Remove(key);
            listPlayerInHome.Add(key);
        }

        public void EnterLocation(KeyCode key)
        {
            if (!listPlayerMoving.Contains(key)) return;

            listPlayerMoving.Remove(key);
            listPlayerOnLocation.Add(key);
        }

        // public void DecentStamina()
        // {
        //     float decentValue = config.Value.characterStatminaDecayRate * Time.deltaTime;
        //     foreach(var player in Players.Keys)
        //     {
        //         if (Players[player].Stamina.Value > 0)
        //             Players[player].Stamina.Value -= decentValue;
        //         else if (Players[player].Stamina.Value < 0)
        //         {
        //             Players[player].Stamina.Value = 0;
        //             Players[player].setIsDry(true);
        //         }
        //     }
        // }

        void Shoot(KeyCode key)
        {
            Vector3 midPosition = Vector3.zero;
            if (listPlayerOnLocation.Contains(key))
            {
                midPosition = Players[key].collideLocation.transform.position;
                listPlayerOnLocation.Remove(key);
            }
            if (listPlayerInHome.Contains(key))
            {
                midPosition = scoreBase.transform.position;
                listPlayerInHome.Remove(key);
            }
            Players[key].ReturnSupply();
            Players[key].collideLocation = null;

            Players[key].ForwardVector = (Players[key].transform.position - midPosition).normalized * Players[key].Speed;

            Players[key].Ready = false;
            Players[key].canCollide = true;
            Players[key].audio.PlayOneShot(AppCore.Instance.activePlayers[key].assignedActionAudio);
            listPlayerMoving.Add(key);
        }

        List<KeyCode> listPlayerMoving = new List<KeyCode>();
        public void PlayerUpdate()
        {
            foreach(var player in listPlayerMoving)
            {
                // float gravityMagnitude = config.Value.gravityMultiplier / (Players[player].Stamina + 1);
                Vector3 vGravity = Gravity.getGravity(Players[player].transform.position,config.Value.gravityMultiplier);
                
                Players[player].gravityAccelator = vGravity;
                Players[player].PlayerUpdate();

                BorderDectection(player);
            }
        }

        void BorderDectection(KeyCode player)
        {

            if (Vector2.Distance(Players[player].transform.position, scoreBase.transform.position) >= config.Value.worldRidius)
            {
                Vector3 inVector = Players[player].ForwardVector.normalized;
                Vector3 normal = (Players[player].transform.position - scoreBase.transform.position).normalized;
                Vector2 outVector = -1 * (Vector2.Dot(inVector, normal) * normal * 2 - inVector);
                Players[player].ForwardVector = outVector.normalized * Players[player].ForwardVector.magnitude;
            }
        }

        List<KeyCode> listPlayerOnLocation = new List<KeyCode>();
        public void RotatePlayerOnLocation (CharacterProperty c, Vector3 position)
        {
            c.transform.position += c.collideLocation.transform.position - position;

            c.lastLocationPosition = c.collideLocation.transform.position;

            c.transform.RotateAround(c.collideLocation.transform.position, Vector3.forward, config.Value.capturedPickupRevolutionSpeed);
        }

        List<KeyCode> listPlayerInHome = new List<KeyCode>();
        public void RotatePlayerInHome (CharacterProperty c)
        {
            c.Stamina.Value = config.Value.staminaChargeNumber;
            c.transform.RotateAround(scoreBase.transform.position, Vector3.forward, 1f);
        }

        float GetHomeRadius(int peopleNumber)
        {
            if (peopleNumber <= 3) return 0.1f;

            float r = 0.05f;
            for (int n = 3; n < peopleNumber; n++)
                r = r + r * 0.1f * n;

            return r;
        }
        
        public int SuppliesSum
        {
            get
            {
                int t = 0;
                foreach (HashSet<SupplyDrop> h in pickUpsInZones)
                {
                    t += h.Count;
                }
                return t;

            }
        }

        void Awake ()
        {
            SingletonBehaviourLocator<GameCore>.Set(this);
            stateMachine = new StateMachine<GameCore>(this);
            stateMachine.ChangeState(new GamePreparing(stateMachine));

            dropsPool = new GameObjectPool<SupplyDrop>(AppCore.Instance.config.supplyDropPrefab, 40);
            pickUpInstances = new BiMap<int, HashSet<SupplyDrop>>();
            for (int i = 0; i < AppCore.Instance.activePlayers.Count; i++) pickUpInstances.Add(i, new HashSet<SupplyDrop>());

        }

        void Update ()
        {
            stateMachine?.Update();
        }

        // public void Picked (SupplyDrop s)
        // {
        //     s.Occupied = false;
        //     dropsInZones[zoneIndexMap[s]].Remove(s);
        //     pickUpInstances[s.typeIndex].Remove(s);
        //     zoneIndexMap.Remove(s);
        //     dropsPool.ReturnToPool(s);
        // }

        public void PickedBy (SupplyDrop s, int playerIndex)
        {
            s.m_collider.enabled = false;
            s.Occupied = false;
            pickUpsInZones[zoneIndexMap[s]].Remove(s);
            pickUpInstances[playerIndex].Remove(s);
            zoneIndexMap.Remove(s);
            dropsPool.ReturnToPool(s);
        }

        public void Prepare ()
        {            

            gravityZones.Value = new GameObject[config.Value.gravityZoneSteps.Length - 1];
            GameObject g = GameObject.Instantiate(gravityZonePrefab);
            g.transform.localScale = Vector3.one * config.Value.gravityZoneSteps[0];
            
            pickUpsInZones = new HashSet<SupplyDrop>[gravityZones.Value.Length];
            for (int i = 0; i < gravityZones.Value.Length; i++) pickUpsInZones[i] = new HashSet<SupplyDrop>();

            for (int i = 1; i < config.Value.gravityZoneSteps.Length; i++)
            {
                gravityZones.Value[i - 1] = GameObject.Instantiate(gravityZonePrefab);
                gravityZones.Value[i - 1].transform.localScale = Vector3.one * config.Value.gravityZoneSteps[i];                    
            }
            
            Players = new Dictionary<KeyCode, CharacterProperty>();
            if (GameCore.Instance.config.Value.gravityZoneWeight.Length < gravityZones.Value.Length)
            {
                zoneWeights = new float[gravityZones.Value.Length];
                GameCore.Instance.config.Value.gravityZoneWeight.CopyTo(zoneWeights, 0);
                for (int i = GameCore.Instance.config.Value.gravityZoneWeight.Length; i < gravityZones.Value.Length; i++) zoneWeights[i] = 1;
            }
            else zoneWeights = GameCore.Instance.config.Value.gravityZoneWeight;
            
        }


        public int GetLeastPickupTypeIndex ()
        {
            int least = pickUpInstances[AppCore.Instance.orderedPlayers[0].assginedPickupType].Count, resultIndex = 0;
            for (int i = 1; i < AppCore.Instance.orderedPlayers.Count; i++)
            {
                if (pickUpInstances[AppCore.Instance.orderedPlayers[i].assginedPickupType].Count < least)
                {
                    least = pickUpInstances[i].Count;
                    resultIndex = i;
                }
            }
            return resultIndex; 
        }

        public int SpawnSupplyInMostEmptyZone (int typeIndex)
        {
            int emptyZoneIndex = gravityZones.Value.Length - 1, lastLeast = pickUpsInZones[emptyZoneIndex].Count;
            for (int i = emptyZoneIndex - 1; i > 0; i--)
            {
                if (pickUpsInZones[i].Count < lastLeast)
                {
                    lastLeast = pickUpsInZones[i].Count;
                    emptyZoneIndex = i;
                }
            }

            PlaceSupply(emptyZoneIndex, typeIndex);
            return emptyZoneIndex;
        }

        public int SpawnSupplyInRandomZone (int activePickupType)
        {
            float t = Random.Range(0, zoneWeights.Sum());
            for (int i = gravityZones.Value.Length - 1; i > 1 ; i--)
            {
                if (t > zoneWeights[i] && t - zoneWeights[i - 1] < zoneWeights[i])
                {
                    PlaceSupply(i, activePickupType);
                    return i;   
                }
                t -= zoneWeights[i];
            }

            PlaceSupply(1, activePickupType);
            return 1;
        }

        public void PlaceSupply (int zoneIndex, int activePickupType)
        {
            GameObject t = null;
            SupplyDrop d = dropsPool.GetObjectFromPool(null);
            t = d.gameObject;
            d.m_collider.enabled = true;
            d.SetType(activePickupType);
            
            PlaceToOrbit(Random.Range(config.Value.gravityZoneSteps[zoneIndex - 1], config.Value.gravityZoneSteps[zoneIndex]),
            gravityZones.Value[zoneIndex].transform,
            t);

            if (pickUpsInZones[zoneIndex] == null) pickUpsInZones[zoneIndex] = new HashSet<SupplyDrop>();
            pickUpsInZones[zoneIndex].Add(d);

            zoneIndexMap.Add(d, zoneIndex);

            if (pickUpInstances[activePickupType] == null) pickUpInstances[activePickupType] = new HashSet<SupplyDrop>();
            pickUpInstances[activePickupType].Add(d);
        }

        ContactFilter2D filter = new ContactFilter2D () { useTriggers = true };

        Collider2D[] colCache;

        void PlaceToOrbit (float distance, Transform parentZone, GameObject t)
        {
            t.transform.SetParent(parentZone, true);
            t.transform.localPosition = Vector3.zero;
            distance = distance / 2f; // Steps is scale value for zones.
            t.transform.position = parentZone.position + Quaternion.Euler(0, 0, Random.Range(0f, 359.9f)) * Vector2.left * distance;
            for (int i = 0; i < 3; i++)
            {
                if (Physics2D.OverlapCircle(t.transform.position, config.Value.densityBalancingDistance, filter, colCache) > 0) 
                    t.transform.position = parentZone.position + Quaternion.Euler(0, 0, Random.Range(0f, 359.9f)) * Vector2.left * distance;
                else break;
            }
            // GameObject p = GameObject.Instantiate(burstVFXPrefab, t.transform.position, Quaternion.identity);
            // MonoBehaviour.Destroy(p, 5f);
        }



        public TMPro.TextMeshProUGUI countDownText;
        public GameObject FinishUI;
        public TMPro.TextMeshProUGUI Total;
        public TMPro.TextMeshProUGUI[] PanelScore, PanelKey;

        public void ShowFinishUI()
        {
            FinishUI.SetActive(true);
            var score = scoreBase.gameObject.GetComponent<ScoreBase>();
            Total.text = score.scoreR.Value.ToString();
            Dictionary<KeyCode, int> scores = new Dictionary<KeyCode,int>();
            foreach(var player in Players)
                scores.Add(player.Key, player.Value.totalScore);

            List<(int, KeyCode)> Top = new List<(int, KeyCode)>();
            while(Top.Count < 3)
            {
                if (scores.Count <= 0) break;
                KeyCode key = KeyCode.Q;
                int s = -1;

                foreach(var ds in scores)
                {
                    if (ds.Value > s)
                    {
                        key = ds.Key;
                        s = ds.Value;
                    } 
                }
                Top.Add((s,key));
                scores.Remove(key);
            }

            for(int i = 0; i < Top.Count; i++)
            {
                PanelScore[i].text = Top[i].Item1.ToString();
                PanelKey[i].text = Top[i].Item2.ToString();
            }
            
        }
        
    }
}