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
    
    public static bool IsScrap(GrabbableObject grabbableObject, OrganizeInformation organizeInformation)
    {
        if (grabbableObject == null) return false;
        if (!IsScrap(grabbableObject.itemProperties, organizeInformation)) return false;

        if (grabbableObject.isHeld || !grabbableObject.grabbable || IsExcluded(grabbableObject, organizeInformation))
        {
            return false;
        }

        return true;
    }

    public static bool IsExcluded(GrabbableObject grabbableObject, OrganizeInformation organizeInformation)
    {
        var result = false;
        if(organizeInformation.exclusionList == "") return false;
        
        organizeInformation.exclusionList.Split(",").ToList().ForEach(name =>
        {
            if(organizeInformation.exclusionList == "")
            {
                result = false;
            }
            else if(OptimalScrapsOrganizationPlugin.instance.StringContain(grabbableObject.itemProperties.itemName, name.ToString())) result = true;
        });

        return result;
    }

    public static bool IsScrap(Item item, OrganizeInformation organizeInformation)
    {
        if (item == null) return false;

        return organizeInformation.orderShopItems || item.isScrap;
    }
    
    public static bool IsValidScrap(GrabbableObject grabbableObject, OrganizeInformation organizeInformation)
    {
        if (grabbableObject == null) return false;

        if (!IsScrap(grabbableObject, organizeInformation)) return false;
        
        return true;
    }
    
    private static List<GrabbableObject> GetValidScrap(IEnumerable<GrabbableObject> grabbableObjects, OrganizeInformation organizeInformation)
    {
        if (grabbableObjects == null) return [];

        return grabbableObjects.Where(x => IsValidScrap(x, organizeInformation)).ToList();
    }
    
    public static List<GrabbableObject> GetScrapFromShip(OrganizeInformation organizeInformation)
    {
        if (HangarShipTransform == null) return [];

        return GetValidScrap(HangarShipTransform.GetComponentsInChildren<GrabbableObject>(), organizeInformation);
    }

    public static void OrganizeShipScraps(OrganizeInformation organizeInformation)
    {
        var scrapsOnShip = GetScrapFromShip(organizeInformation);

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
            case OrganizeBy.TYPE:
            {
                scrapsOnShip.Sort((x, y) => x.itemProperties.twoHanded.CompareTo(y.itemProperties.twoHanded));
                break;
            }
        }
        
        if(scrapsOnShip.Count == 0) return;
        
        var basePos = position;
        var usePostion = basePos;
        var valueRange = 0;
        var rotation = 0f;

        while (valueRange < scrapsOnShip[0].scrapValue)
        {
            valueRange += value;
        }

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
            if (organizeInformation.rotateScraps)
            {
                scrap.transform.eulerAngles += new Vector3(0, rotation, 0);
                rotation += organizeInformation.rotationBetweenScraps;
            }

            if (organizeBy == OrganizeBy.NAME && index < scrapsOnShip.Count - 1 &&  scrap.itemProperties.itemName == scrapsOnShip[index+1].itemProperties.itemName) return;
            if (organizeBy == OrganizeBy.VALUE && index < scrapsOnShip.Count - 1  && scrapsOnShip[index + 1].scrapValue <= valueRange ) return;
            if (organizeBy == OrganizeBy.TYPE && index < scrapsOnShip.Count - 1  && scrapsOnShip[index + 1].itemProperties.twoHanded == scrap.itemProperties.twoHanded ) return;

            if (index < scrapsOnShip.Count - 1 )
            {
                while (valueRange < scrapsOnShip[index + 1].scrapValue)
                {
                    valueRange += value;
                }
            }

            usePostion += new Vector3(organizeInformation.distanceBetweenObjects, 0, 0);
            rotation = 0;

        });
        
    }
    
}