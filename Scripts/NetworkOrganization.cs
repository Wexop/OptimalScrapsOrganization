using StaticNetcodeLib;
using Unity.Netcode;

namespace OptimalScrapsOrganization.Scripts;

[StaticNetcode]
public class NetworkOrganization
{
    [ServerRpc]
    public static void OrganizeScrapsServerRpc(OrganizeInformation organizeInformation)
    {
        OrganizeScrapsClientRpc(organizeInformation);
    }
    
    [ClientRpc]
    public static void OrganizeScrapsClientRpc(OrganizeInformation organizeInformation)
    {
        ScrapsOrganizationManager.OrganizeShipScraps(organizeInformation);
    }
    
}