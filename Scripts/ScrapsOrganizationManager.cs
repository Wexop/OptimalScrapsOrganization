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
        var organizeBy = organizeInformation.OrganizeBy == OrganizeBy.LOCKER ? organizeInformation.SecondOrganizeBy : organizeInformation.OrganizeBy;

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
        
        var inititalPosition = position;
        var basePos = inititalPosition;
        var usePostion = basePos;
        var valueRange = 0;
        var rotation = 0f;
        var maxPosZ = maxPostionRight.z;
        var maxPosX = maxPosition.x;
        
        List<Collider> colliders = new List<Collider>();
        

        if (organizeInformation.OrganizeBy == OrganizeBy.LOCKER)
        {
            List<PlaceableObjectsSurface> placeableObjectsSurfaces =
                HangarShipTransform.GetComponentsInChildren<PlaceableObjectsSurface>().ToList();

            if (placeableObjectsSurfaces.Count > 0)
            {
                
                placeableObjectsSurfaces.ForEach(p =>
                {
                    if (p.placeableBounds) colliders.Add(p.placeableBounds);
                });
            }
        }

        while (valueRange < scrapsOnShip[0].scrapValue)
        {
            valueRange += value;
        }

        var index = -1;
        int lockerIndex = 0;
        int maxLockerIndex = colliders.Count - 1;

        if (colliders.Count > 0)
        {
            Debug.Log($"PLACES COUNT {colliders.Count}");
            colliders.Sort((x, y) => x.transform.position.y.CompareTo(y.transform.position.y));
            inititalPosition = usePostion = basePos = colliders[lockerIndex].transform.localPosition - new Vector3(colliders[lockerIndex].bounds.extents.x,0, colliders[lockerIndex].bounds.size.z);
            maxPosX = inititalPosition.x + colliders[lockerIndex].bounds.size.x * 2;
            organizeInformation.distanceBetweenObjects = 1f;
        }
        
        scrapsOnShip.ForEach(scrap =>
        {
            index++;
            if(scrap == null) return;
            if(!organizeInformation.orderPlacedItems && scrap.transform.parent != HangarShipTransform) return;

            if (usePostion.z < maxPosZ)
            {
                usePostion = inititalPosition;
            }
            if (usePostion.x > maxPosX)
            {

                if (organizeInformation.OrganizeBy == OrganizeBy.LOCKER)
                {
                    if (lockerIndex + 1 <= maxLockerIndex) lockerIndex++;
                    else lockerIndex = 0;
                    basePos = colliders[lockerIndex].transform.localPosition - new Vector3(colliders[lockerIndex].bounds.size.x,0, colliders[lockerIndex].bounds.size.z);
                    maxPosX = basePos.x + colliders[lockerIndex].bounds.size.x * 2;
                    
                    Debug.Log($"NEW LOCKER INDEX {lockerIndex} POS {basePos}");

                }
                else
                {
                    basePos += new Vector3(0,0, -organizeInformation.distanceBetweenObjects);
                    
                }
                
                usePostion = basePos;
            }

            var positionWithOffset = usePostion + Vector3.up * scrap.itemProperties.verticalOffset;
            
            scrap.targetFloorPosition = positionWithOffset;

            scrap.transform.eulerAngles = Vector3.zero + scrap.itemProperties.restingRotation;
            scrap.transform.parent = HangarShipTransform;
            
            if (organizeInformation.OrganizeBy == OrganizeBy.LOCKER)
            {
                var parentObject = colliders.Count > 0 ? colliders[lockerIndex].transform.parent : HangarShipTransform;
                PlayerPhysicsRegion componentInChildren = parentObject.GetComponentInChildren<PlayerPhysicsRegion>();
                if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null && componentInChildren.allowDroppingItems)
                    parentObject = componentInChildren.physicsTransform;
                scrap.transform.SetParent(parentObject, true);
                scrap.startFallingPosition = scrap.transform.localPosition;
                scrap.transform.localPosition = positionWithOffset;
                scrap.targetFloorPosition = positionWithOffset;
                scrap.EnablePhysics(true);
                scrap.EnableItemMeshes(true);
            }
            
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