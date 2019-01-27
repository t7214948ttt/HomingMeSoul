using UnityEngine;

namespace AngerStudio.HomingMeSoul.Game
{

    [System.Serializable]
    public class GameConfig
    {
        public FloatReference characterStatminaDecayRate, supplyRevolutionSpeed, gravityMultiplier, staminaChargeNumber, speedMultiplier, worldRidius;

        public float[] gravityZoneSteps = {5, 10, 15, 20, 25};

        public Sprite[] forbiddenPickupTypes;

        public float treasureChance = 0.05f;

        public float rimRewardFactor = 1.01f;

        public float badGuySpawnChancePerMinute = 0.1f;

        public bool building_Exchange, building_Upgrade, building_Casino;

        public float[] gravityZonesRevolutionSpeeds = { 3f, 5f, 8f, 12f, 16f, 20f, 25f, 30f, 35f, 40f, 48f, 56f };

        public int maxSupplyDrops = 100, minSupplyDrops = 40;

        public float densityBalancingDistance = 7f;

        public float passiveSPGainDelayInSconds = 5f;

    }
}