using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFurnature", menuName = "Scriptable/Furnature")]
public class Furnature : ScriptableObject
{
    public GameObject prefab;
    public float length;
    public float width;
    public float height;
}
