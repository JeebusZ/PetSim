using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Resource", menuName ="PetVenture/Create/Resource")]
public class Resource : ScriptableObject
{
    public string _name;
    public Sprite _icon;
}
