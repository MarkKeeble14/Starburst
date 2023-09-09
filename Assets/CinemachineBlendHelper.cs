using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineBlendHelper : MonoBehaviour
{
    private CinemachineBrain brain;
    void Start()
    {
        CinemachineCore.GetInputAxis = GetAxisCustom;
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private float GetAxisCustom(string axisName)
    {
        if (brain.IsBlending)
            return 0;
        return Input.GetAxis(axisName);
    }
}
