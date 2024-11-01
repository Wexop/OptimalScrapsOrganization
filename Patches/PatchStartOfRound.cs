using HarmonyLib;
using OptimalScrapsOrganization.Scripts;

namespace OptimalScrapsOrganization.Patches;

[HarmonyPatch(typeof(StartOfRound))]
public class PatchStartOfRound
{
    [HarmonyPatch("AutoSaveShipData")]
    [HarmonyPrefix]
    private static void ShipHasLeftPostfix(StartOfRound __instance)
    {
        if (__instance.IsServer && OptimalScrapsOrganizationPlugin.instance.autoReorderOnLeftMoon.Value)
        {
            OrganizeInformation organizeInformation = new OrganizeInformation();
            organizeInformation.OrganizeBy = OptimalScrapsOrganizationPlugin.instance.defaultReorderType.Value;
            organizeInformation.value = OptimalScrapsOrganizationPlugin.instance.organiseDefaultValueRange.Value;
            organizeInformation.distanceBetweenObjects = OptimalScrapsOrganizationPlugin.instance.distanceBewteenObjects.Value;
            NetworkOrganization.OrganizeScrapsClientRpc(organizeInformation);
        }
    }
}