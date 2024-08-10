using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "PetVenture/Create/Lootable")]
public class Minable : ScriptableObject
{
    public string _name;
    public string _oreTag;
    public string _lootDropCoinTag;
    public string _lootDropBagTag;
    public string _lootDropDiamondTag;
    public long _health;
    public float _bonus;
    public long _minGather, _maxGather;
    public int _minDiamondDrop, _maxDiamondDrop;
    public int _maxPetForGathering;
}
