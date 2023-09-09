using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateScale : MonoBehaviour
{
    [SerializeField] private float minSize;
    [SerializeField] private float size;
    [SerializeField] private float timePerGrowth;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(minSize, timePerGrowth);
    }

    private bool add = true;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime * (add ? 1 : -1);
        transform.localScale = Vector3.one * (timer / timePerGrowth) * size;
        if (timer > timePerGrowth)
        {
            timer *= -1;
            timer = timePerGrowth;
            add = false;
        }
        else if (timer < minSize)
        {
            timer *= -1;
            timer = minSize;
            add = true;
        }
    }
}
