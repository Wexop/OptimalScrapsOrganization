using System;
using System.Collections.Generic;
using System.Linq;
using LethalLevelLoader;
using UnityEngine;

namespace OptimalScrapsOrganization.Scripts;

public class ScrapsOrganizationManager
{
    public static Transform HangarShipTransform => StartOfRound.Instance.elevatorTransform;
    
    public static Vector3 position = new Vector3(-6.18466759f, 0.103276253f, -5f);
    public static Vector3 maxPosition = new Vector3(9.047635f,0,-5f);
        
    public static Vector3 postionRight = new Vector3(-4.263417f,0,-9.5f);
    public static Vector3 maxPostionRight = new Vector3(3.728537f,0,-9.5f);
    
    public static bool IsScrap(GrabbableObject grabbableObject)
    {
        if (grabbableObject == null) return false;
        if (!IsScrap(grabbableObject.itemProperties)) return false;

        if (grabbableObject.isHeld || !grabbableObject.grabbable)
        {
            return false;
        }

        return true;
    }

    public static bool IsScrap(Item item)
    {
        if (item == null) return false;

        return item.isScrap;
    }
    
    public static bool IsValidScrap(GrabbableObject grabbableObject)
    {
        if (grabbableObject == null) return false;

        if (!IsScrap(grabbableObject)) return false;
        
        return true;
    }
    
    private static List<GrabbableObject> GetValidScrap(IEnumerable<GrabbableObject> grabbableObjects)
    {
        if (grabbableObjects == null) return [];

        return grabbableObjects.Where(x => IsValidScrap(x)).ToList();
    }
    
    public static List<GrabbableObject> GetScrapFromShip()
    {
        if (HangarShipTransform == null) return [];

        return GetValidScrap(HangarShipTransform.GetComponentsInChildren<GrabbableObject>());
    }

    public static void OrganizeShipScraps(OrganizeInformation organizeInformation)
    {
        var scrapsOnShip = GetScrapFromShip();

        var value = organizeInformation.value;
        var organizeBy = organizeInformation.OrganizeBy;

        if (value <= 0) value = 1;

        switch (organizeBy)
        {
            case OrganizeBy.VALUE:
            {
                scrapsOnShip.Sort((x, y) => x.scrapValue.CompareTo(y.scrapValue));
                break;
            }
            case OrganizeBy.NAME:
            {
                scrapsOnShip.Sort((x, y) => x.itemProperties.itemName.CompareTo(y.itemProperties.itemName));
                break;
            }
        }
        
        if(scrapsOnShip.Count == 0) return;
        
        var basePos = position;
        var usePostion = basePos;
        var valueRange = 0;

        var index = -1;
        
        scrapsOnShip.ForEach(scrap =>
        {
            index++;
            if(scrap == null) return;

            if (usePostion.z < maxPostionRight.z)
            {
                usePostion = position;
            }
            if (usePostion.x > maxPosition.x)
            {
                basePos += new Vector3(0,0, -organizeInformation.distanceBetweenObjects);
                usePostion = basePos;
            }
            
            scrap.targetFloorPosition = usePostion;
            scrap.transform.eulerAngles = Vector3.zero + scrap.itemProperties.restingRotation;

            if (organizeBy == OrganizeBy.NAME && index < scrapsOnShip.Count - 1 &&  scrap.itemProperties.itemName == scrapsOnShip[index+1].itemProperties.itemName) return;
            if (organizeBy == OrganizeBy.VALUE && scrap.scrapValue <= valueRange ) return;
            
            
            
            valueRange += value;

            usePostion += new Vector3(organizeInformation.distanceBetweenObjects, 0, 0);

        });
        
    }
    
}