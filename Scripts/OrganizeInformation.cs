﻿namespace OptimalScrapsOrganization.Scripts;

public class OrganizeInformation
{
    public OrganizeBy OrganizeBy;
    public OrganizeBy? SecondOrganizeBy = null;
    public int value;
    public float distanceBetweenObjects;
    public bool rotateScraps;
    public float rotationBetweenScraps;
    public string exclusionList;
    public bool orderShopItems;
    public bool orderPlacedItems;
}