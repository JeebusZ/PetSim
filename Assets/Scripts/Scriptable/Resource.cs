using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    COPPER,
    SILVER,
    GOLD,
    PLATINUM,
    DIAMOND
}

[CreateAssetMenu(fileName ="New Resource", menuName ="PetVenture/Create/Resource")]
public class Resource : ScriptableObject
{
    public string _name;
    public Sprite _icon;
    public ResourceType _resourceType;
}
