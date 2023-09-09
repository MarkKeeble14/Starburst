using System.Collections;
using UnityEngine;

public class PickUpTrigger : MonoBehaviour
{
    private PickUp pickUp;
    public PickUp PickUp
    {
        get
        {
            if (pickUp == null)
                pickUp = GetComponentInParent<PickUp>();
            return pickUp;
        }
    }

    [SerializeField] private float expandSpeed = 10f;

    public void Set(float radius)
    {
        StartCoroutine(SetRadius(radius));
    }

    private IEnumerator SetRadius(float radius)
    {
        Vector3 goal = Vector3.one * radius;

        while (transform.localScale != goal)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, goal, Time.deltaTime * expandSpeed);
            yield return null;
        }
    }
}
