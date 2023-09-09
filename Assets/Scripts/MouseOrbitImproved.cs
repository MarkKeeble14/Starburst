using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{

    public Transform target;

    public float minMouseSpeed = 40f;
    public float maxMouseSpeed = 120.0f;
    [SerializeField] private float yAxisMouseModifier = 1.5f;
    [SerializeField] private float currentMouseSpeed;
    private Vector3 targetPosition;
    [SerializeField] private float moveSpeed;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;
    public float distanceMin = 10f;
    public float distanceMax = 20f;
    private float distance;

    private Rigidbody rb;
    private float x = 0.0f;
    private float y = 0.0f;

    private List<Vector3> travelPoints = new List<Vector3>();


    public void SetSensitivity(float percent)
    {
        float dif = maxMouseSpeed - minMouseSpeed;
        currentMouseSpeed = minMouseSpeed + (dif * percent);
    }

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        rb = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rb != null)
        {
            rb.freezeRotation = true;
        }
    }

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void LateUpdate()
    {
        if (target)
        {
            x += Input.GetAxis("Mouse X") * currentMouseSpeed * distance * 0.02f;
            y -= Input.GetAxis("Mouse Y") * currentMouseSpeed * 0.02f * yAxisMouseModifier;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            targetPosition = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}