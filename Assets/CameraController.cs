using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform follow;
    [SerializeField] private float followOffset;
    private Vector3 targetPos;
    [SerializeField] private float moveSpeed;

    public float minMouseSpeed = 40f;
    public float maxMouseSpeed = 120.0f;
    [SerializeField] private float yAxisMouseModifier = 1.5f;
    [SerializeField] private float currentMouseSpeed;

    public void SetSensitivity(float percent)
    {
        float dif = maxMouseSpeed - minMouseSpeed;
        currentMouseSpeed = minMouseSpeed + (dif * percent);
    }

    private Vector2 lastMousePos;

    // Update is called once per frame
    void Update()
    {
        Vector2 curMousePos = Input.mousePosition;
        Vector2 mouseDelta = curMousePos - lastMousePos;

        // Position
        targetPos = follow.position + (follow.transform.forward * followOffset);
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

        lastMousePos = curMousePos;
    }
}
