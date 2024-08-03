using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minable : ScriptableObject
{
    public string _name;
    public float _health;
    public long _minGather, _maxGather;
    public GameObject _prefab;
}
