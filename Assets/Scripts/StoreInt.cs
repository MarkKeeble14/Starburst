using System;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreInt", menuName = "StoreInt")]
public class StoreInt : ScriptableObject
{
    public int Value { get; set; }
    [SerializeField] private int defaultValue;

    public void Reset()
    {
        Value = defaultValue;
    }
}
