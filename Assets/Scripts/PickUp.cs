using System;
using System.Collections;
using UnityEngine;

public class PickUp : PhysicsObject
{
    private bool triggered;
    [SerializeField] private float collapseRateIncrease = 1f;
    [SerializeField] private float initialCollapseSpeed = .25f;
    [SerializeField] private float expeditedSpeed = 10f;
    [SerializeField] private StoreInt spawnRange;
    [SerializeField] private float shrinkAtRangeModifier = 3f;
    [SerializeField] private Position playerPosition;
    [SerializeField] private StoreInt numAlive;
    [SerializeField] private GameObject particleOnSpawn;
    [SerializeField] private GameObject[] particleOnCollapse;

    [SerializeField] private AudioClip preClip;
    [SerializeField] private AudioClip preExplodeClip;
    [SerializeField] private AudioClip explodeClip;
    [SerializeField] private float betweenPreAndExplosionTime = 1.5f;

    [SerializeField] private float pickUpRange = 10f;
    [SerializeField] private PickUpTrigger radiusIndicator;
    [SerializeField] private Material arrowMat;
    public Material ArrowMat => arrowMat;

    private float timeForSpawnAnimToReachPeak = .5f;
    private bool spawnParticles = true;
    private GameObject repArrow;

    private new void Update()
    {
        base.Update();
        // Debug.Log(Vector3.Distance(transform.position, playerPosition.Value));
        if (Vector3.Distance(transform.position, playerPosition.Value) > spawnRange.Value * shrinkAtRangeModifier)
        {
            Shrink();
        }
    }

    private void Start()
    {
        Instantiate(particleOnSpawn, transform.position, Quaternion.identity);

        numAlive.Value++;

        // Set Variables
        transform.rotation = UnityEngine.Random.rotation;
        radiusIndicator.Set(pickUpRange);

        StartCoroutine(ShowAfterTime());
    }

    private IEnumerator ShowAfterTime()
    {
        yield return new WaitForSeconds(timeForSpawnAnimToReachPeak);

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }
    }

    public void ExecutePickUp(Action action)
    {
        if (!triggered)
        {
            triggered = true;
            StartCoroutine(PickUpAnimation(action));
        }
    }

    public void Shrink()
    {
        if (!triggered)
        {
            triggered = true;
            spawnParticles = false;
            radiusIndicator.StopAllCoroutines();
            StartCoroutine(ShrinkAnimation());
        }

    }

    private IEnumerator ShrinkAnimation()
    {
        float timer = 1;
        Vector3 goal = Vector3.zero;
        while (transform.localScale != goal)
        {
            timer += Time.deltaTime * collapseRateIncrease;
            transform.localScale = Vector3.MoveTowards(transform.localScale, goal, Time.deltaTime * expeditedSpeed * Mathf.Pow(timer, 2));
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator HideRadiusIndicator()
    {
        while (radiusIndicator.transform.localScale != Vector3.zero)
        {
            radiusIndicator.transform.localScale = Vector3.MoveTowards(radiusIndicator.transform.localScale, Vector3.zero, Time.deltaTime * expeditedSpeed);
            yield return null;
        }
    }

    private IEnumerator PickUpAnimation(Action onEnd)
    {
        StartCoroutine(HideRadiusIndicator());

        float pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        AudioManager._Instance.PlayOneShot(preClip, pitch, transform.position);

        AnimateScale animScale = GetComponent<AnimateScale>();
        if (animScale != null)
            Destroy(animScale);

        radiusIndicator.StopAllCoroutines();

        Vector3 goal = Vector3.one * 0.05f;
        float timer = 1;
        while (transform.localScale != goal)
        {
            timer += Time.deltaTime * collapseRateIncrease;
            transform.localScale = Vector3.MoveTowards(transform.localScale, goal, Time.deltaTime * initialCollapseSpeed * Mathf.Pow(timer, 2));
            yield return null;
        }

        AudioManager._Instance.PlayOneShot(preExplodeClip, pitch, transform.position);

        yield return new WaitForSeconds(betweenPreAndExplosionTime);

        AudioManager._Instance.PlayOneShot(explodeClip, pitch, transform.position);

        onEnd();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;

        numAlive.Value--;

        Destroy(repArrow);

        if (!spawnParticles) return;
        foreach (GameObject obj in particleOnCollapse)
        {
            Instantiate(obj, transform.position, Quaternion.identity);
        }
    }

    public void SetRepArrow(GameObject arrow)
    {
        repArrow = arrow;
    }
}
