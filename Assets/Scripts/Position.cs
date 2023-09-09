using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "Position", order = 1)]
public class Position : ScriptableObject
{
    public Vector3 Value { get; set; }

}
