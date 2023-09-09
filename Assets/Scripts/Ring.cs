using UnityEngine;

public class Ring : PhysicsObject
{
    [SerializeField] private GameObject[] particlesOnDestroy;
    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        foreach (GameObject obj in particlesOnDestroy)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }
    }
}
