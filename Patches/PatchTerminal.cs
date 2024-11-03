using HarmonyLib;
using OptimalScrapsOrganization.Scripts;
using UnityEngine;

namespace OptimalScrapsOrganization.Patches;

[HarmonyPatch(typeof(Terminal))]
public class PatchTerminal
{
    
    [HarmonyPatch("ParsePlayerSentence")]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    private static bool ParsePlayerSentencePatch(Terminal __instance, ref TerminalNode  __result)
    {
        string[] array = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded).Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        
        if (array[0].ToLower().Contains("reorder") || array[0].ToLower().Contains("reoder"))
        {

            if (array.Length > 1 && array[1].ToLower().Contains("help"))
            {
                __result = CreateTerminalNode("Commands :\n\n- reorder\n\n- reorder <valueRange>\n\n- reorder name\n\n- reorder type\n\n");
                return false;
            }

            OrganizeBy organizeBy = OptimalScrapsOrganizationPlugin.instance.defaultReorderType.Value;
            var value = OptimalScrapsOrganizationPlugin.instance.organiseDefaultValueRange.Value;
            
            if(array.Length > 1 && array[1].ToLower().Contains("name")) organizeBy = OrganizeBy.NAME;
            else if(array.Length > 1 && array[1].ToLower().Contains("type")) organizeBy = OrganizeBy.TYPE;
            else if (array.Length > 1)
            {
                organizeBy = OrganizeBy.VALUE;
                int.TryParse(array[1], out value);
            }

            OrganizeInformation organizeInformation = new OrganizeInformation();
            organizeInformation.OrganizeBy = organizeBy;
            organizeInformation.value = value;
            organizeInformation.distanceBetweenObjects = OptimalScrapsOrganizationPlugin.instance.distanceBetweenScraps.Value;
            organizeInformation.rotateScraps = OptimalScrapsOrganizationPlugin.instance.rotateScraps.Value;
            organizeInformation.rotationBetweenScraps = OptimalScrapsOrganizationPlugin.instance.rotationBetweenScraps.Value;
            organizeInformation.exclusionList = OptimalScrapsOrganizationPlugin.instance.exclusionList.Value;
            organizeInformation.orderShopItems = OptimalScrapsOrganizationPlugin.instance.orderShopItems.Value;
            
            NetworkOrganization.OrganizeScrapsServerRpc(organizeInformation);
            __result = CreateTerminalNode("Done !");
            return false;
        }
        return true;
    }
    
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Last)]
    private static void StartPatch(ref TerminalNodesList ___terminalNodes)
    {
        
        if(OptimalScrapsOrganizationPlugin.instance.hasPatchedStartTerminal) return;
        
        int index = 1;
        string defaultMessage = ___terminalNodes.specialNodes[index].displayText;

        string message = defaultMessage +=
            $"\n[{OptimalScrapsOrganizationPlugin.NAME}]\nType \"reorder help\" for a list of commands.\n";

        ___terminalNodes.specialNodes[index].displayText = message;
        
        index = 13;
        defaultMessage = ___terminalNodes.specialNodes[index].displayText;

        message = defaultMessage +=
            $"\n\n>REORDER HELP\nTo see the list of {OptimalScrapsOrganizationPlugin.NAME} commands.\n";

        ___terminalNodes.specialNodes[index].displayText = message;

        OptimalScrapsOrganizationPlugin.instance.hasPatchedStartTerminal = true;

    }
    
    public static TerminalNode CreateTerminalNode(string message, bool clearPreviousText = true, int maxChar = 50)
    {
        TerminalNode terminalNode = ScriptableObject.CreateInstance<TerminalNode>();

        terminalNode.displayText = message;
        terminalNode.clearPreviousText = clearPreviousText;
        terminalNode.maxCharactersToType = maxChar;

        return terminalNode;
    }
    
}