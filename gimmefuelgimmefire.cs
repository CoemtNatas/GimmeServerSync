using HarmonyLib;
using UnityEngine;

namespace GimmeServerSync;

public class gimmefuelgimmefire
{
     [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
        static class gimmefuel_coe
        {
            static void Postfix(ZNetScene __instance)
            {
                if (__instance == null)
                {
                    return;
                }

                FuelControl(__instance, "piece_groundtorch_wood", GimmeServerSyncPlugin.torch1.Value);
                FuelControl(__instance, "piece_groundtorch_blue", GimmeServerSyncPlugin.torch2.Value);
                FuelControl(__instance, "piece_groundtorch_green", GimmeServerSyncPlugin.torch3.Value);
                FuelControl(__instance, "piece_groundtorch", GimmeServerSyncPlugin.torch4.Value);
                FuelControl(__instance, "fire_pit", GimmeServerSyncPlugin.torch5.Value);
                FuelControl(__instance, "fire_pit_iron", GimmeServerSyncPlugin.torch6.Value);
                FuelControl(__instance, "fire_pit_hildir", GimmeServerSyncPlugin.torch7.Value);
                FuelControl(__instance, "fire_pit_haldor", GimmeServerSyncPlugin.torch8.Value);
                FuelControl(__instance, "hearth", GimmeServerSyncPlugin.torch9.Value);
                FuelControl(__instance, "bonfire", GimmeServerSyncPlugin.torch10.Value);
                FuelControl(__instance, "piece_brazierfloor01", GimmeServerSyncPlugin.torch11.Value);
                FuelControl(__instance, "piece_brazierfloor02", GimmeServerSyncPlugin.torch12.Value);
                FuelControl(__instance, "piece_brazierceiling01", GimmeServerSyncPlugin.torch13.Value);
                FuelControl(__instance, "piece_walltorch",GimmeServerSyncPlugin. torch14.Value);

                FuelControlSmelter(__instance, "smelter", GimmeServerSyncPlugin.smelt1.Value, GimmeServerSyncPlugin.smeltOre1.Value);
                FuelControlSmelter(__instance, "charcoal_kiln", 0, GimmeServerSyncPlugin.smelt2.Value);
                FuelControlSmelter(__instance, "blastfurnace", GimmeServerSyncPlugin.smelt3.Value, GimmeServerSyncPlugin.smeltOre2.Value);
                FuelControlSmelter(__instance, "Armory_TW", GimmeServerSyncPlugin.smelt4.Value, GimmeServerSyncPlugin.smeltOre3.Value);
                FuelControlSmelter(__instance, "piece_spinningwheel", 0, GimmeServerSyncPlugin.smeltOre5.Value);
                FuelControlSmelter(__instance, "windmill", 0, GimmeServerSyncPlugin.smeltOre6.Value);
            }

            private static void FuelControl(ZNetScene instance, string go, float maxFuel)
            {
                if (instance != null)
                {
                    GameObject piece = instance.GetPrefab(go);
                    if (piece == null)
                    {
                        return;
                    }

                    if (piece)
                    {
                        var fire = piece.GetComponent<Fireplace>();
                        if (fire)
                        {
                            fire.m_maxFuel = maxFuel;
                        }
                    }
                }
            }

            private static void FuelControlSmelter(ZNetScene instance, string go, int maxFuel, int maxOre)
            {
                if (instance != null)
                {
                    GameObject piece = instance.GetPrefab(go);
                    if (piece)
                    {
                        if (piece == null)
                        {
                            return;
                        }

                        var fire = piece.GetComponent<Smelter>();
                        if (fire)
                        {
                            fire.m_maxFuel = maxFuel;
                            fire.m_maxOre = maxOre;

                        }
                    }
                }
            }
        }
}