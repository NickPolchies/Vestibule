using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRoom", menuName = "Scriptable/Room")]
public class RoomParameters : ScriptableObject
{
    [Header("Room Stats")]
    public float minWidth;
    public float minLength;
    public float maxWidth;
    public float maxLength;

    [Header("Furnature")]
    public List<Furnature> furnature;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
