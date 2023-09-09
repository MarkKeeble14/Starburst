using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Comet : PhysicsObject
{
    [SerializeField] private GameObject spawnOnHit;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip passClip;
    Action onPass;
    Transform ball;

    public void ContactPlayer(MainBall ball)
    {
        Instantiate(spawnOnHit, transform.position, Quaternion.identity);
        AudioManager._Instance.PlayOneShot(hitClip, UnityEngine.Random.Range(0.9f, 1.1f), transform.position);
        ball.StartExplode();

    }

    public void Set(Transform ball, Action onPass)
    {
        this.onPass = onPass;
        this.ball = ball;
        StartCoroutine(CheckIfPassed());
    }

    private IEnumerator CheckIfPassed()
    {
        yield return new WaitUntil(() => velocity != Vector3.zero);

        bool hasPassed = false;
        Vector3 travelDirection = velocity.normalized;
        Vector3 ballStartPos = ball.transform.position;
        Vector3 checkPos = ballStartPos + travelDirection * 10;
        float currentDistance = Vector3.Distance(transform.position, checkPos);
        float lastDistance = currentDistance;

        while (!hasPassed)
        {
            // normalize velocity to find direction of travel
            // check if position is past player position + direction?
            travelDirection = velocity.normalized;
            currentDistance = Vector3.Distance(transform.position, checkPos);
            if (currentDistance > lastDistance)
            {
                hasPassed = true;
            }
            lastDistance = currentDistance;
            yield return null;
        }
        AudioManager._Instance.PlayOneShot(passClip, UnityEngine.Random.Range(0.9f, 1.1f), transform.position);
        onPass();
    }
}
