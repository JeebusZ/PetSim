using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PetTiers
{
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGEND,
    MYTHIC
}

[CreateAssetMenu(fileName = "New Resource", menuName = "PetVenture/Create/Pet")]
public class Pet : ScriptableObject
{
    public string _name;
    public Color _color;
    public float _power;
    public float _attackSpeed;
    public float _upgradeCost;
    public PetTiers _tier;
}
