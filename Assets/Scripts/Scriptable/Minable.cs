using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "PetVenture/Create/Lootable")]
public class Minable : ScriptableObject
{
    public string _name;
    public string _tag;
    public float _health;
    public float _bonus;
    public long _minGather, _maxGather;
    public GameObject _prefab;
}
