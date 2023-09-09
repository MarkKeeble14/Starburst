using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : PhysicsObject
{
    [SerializeField] private float shrinkSpeed;
    [SerializeField] private Vector2 randomSize;
    [SerializeField] private List<Material> possibleMaterial = new List<Material>();
    [SerializeField] private Vector3 shiftPosBounds = new Vector3(.5f, .5f, .5f);
    [SerializeField] private new Renderer renderer;

    public void Set(Vector3 velocity, Vector3 force)
    {
        transform.position += new Vector3(Random.Range(-shiftPosBounds.x, shiftPosBounds.x),
            Random.Range(-shiftPosBounds.y, shiftPosBounds.y),
            Random.Range(-shiftPosBounds.z, shiftPosBounds.z));

        // Set size randomly
        transform.localScale = transform.localScale + (Vector3.one * Random.Range(randomSize.x, randomSize.y));

        // Set material randomly
        renderer.material = possibleMaterial[Random.Range(0, possibleMaterial.Count)];

        AddForce(force);
        SetVelocity(velocity);

        StartCoroutine(Lifetime());
    }

    private IEnumerator Lifetime()
    {
        // Scale down over lifetime
        while (Vector3.Distance(transform.localScale, Vector3.zero) > .5f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, Time.deltaTime * shrinkSpeed);
            yield return null;
        }

        Destroy(gameObject);
    }
}
