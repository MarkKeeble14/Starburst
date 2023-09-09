using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoreSetting", menuName = "StoreSetting", order = 1)]
public class StoreSetting : ScriptableObject
{
    [Range(0, 1)]
    [SerializeField] private float mouseSensitivity;
    public float Value
    {
        get
        {
            return mouseSensitivity;
        }
        set
        {
            mouseSensitivity = value;
        }
    }
}
