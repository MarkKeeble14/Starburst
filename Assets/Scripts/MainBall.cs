using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using Cinemachine;

public class MainBall : PhysicsObject
{
    [Header("Game")]
    [SerializeField] private bool gameActive;

    [Header("Physics Stats")]
    public float flameSpeed;
    public float cometSpeed;
    public float reduceAccelerationSpeed;
    [SerializeField] private float minAccelerationToStopBrake = 0.01f;
    [SerializeField] private float minAccelerationToBrake = 1f;

    [Header("Comet Stats")]
    [SerializeField] private float timebeforeFirstComets;
    [SerializeField] private float timeBtweenComets;
    [SerializeField] private float timeBtweenCometsReductionRate = 0.01f;
    [SerializeField] private float minTimeBetweenComet = 2f;
    [SerializeField] private float cometSpawnAtDistance;
    [SerializeField] private float cometSizeBoost = 0;
    [SerializeField] private float cometSizeBoostGrow = .34f;

    [Header("Pick Up Stats")]
    [SerializeField] private int numPickUpsAllowedAtOnce = 1;
    [SerializeField] private int pickUpsBeforeAllowMorePickUpsAtOnce = 2;
    [SerializeField] private StoreInt pickUpSpawnAtDistance;
    [SerializeField] private int pickUpSpawnDistanceGrowth = 10;
    [SerializeField] float timeBetweenPickUpSpawns = 2f;
    private float timeBetweenPickUpSpawnsTimer;
    [SerializeField] private LayerMask pickUpLayer;
    private List<PickUp> canPickUp = new List<PickUp>();
    [SerializeField] private StoreInt pickUpsAlive;
    private int pickUpGrowGoal;

    [Header("Ring Stats")]
    [SerializeField] private Vector2 minMaxRingFloatSpeed = new Vector2(50, 150);
    [SerializeField] private float timeBtweenDestroys = 1f;

    [Header("Smoke Stats")]
    [SerializeField] private int numSmokePerFlame = 3;

    [Header("Prefabs")]
    [SerializeField] private Flame flamePrefab;
    [SerializeField] private Flame antiFlamePrefab;
    [SerializeField] private Comet cometPrefab;
    [SerializeField] private Smoke smokePrefab;
    [SerializeField] private PickUp[] pickUpPrefab;
    [SerializeField] private ArrowPointer arrowPointerPrefab;
    [SerializeField] private ArrowPointer dangerPointerPrefab;

    [Header("Materials")]
    [SerializeField] private Material cometMaterial;

    [Header("References")]
    [SerializeField] private Ring[] rings;
    [SerializeField] private TextMeshProUGUI startGameText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI pickUpText;
    [SerializeField] private TextMeshProUGUI velocityText;
    [SerializeField] private TextMeshProUGUI accelerationText;
    [SerializeField] private TextMeshProUGUI pickUpInstructText;
    [SerializeField] private TextMeshProUGUI nextCometText;
    [SerializeField] private GameObject stopVelocityParticle;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private ParticleSizeInfo[] particlesOnDestroy;
    [SerializeField] private Position playerPosition;
    [SerializeField] private Animator startTextAnim;

    private bool paused;
    private bool lockMovement;
    private bool gameOvered;
    private float timer;
    public float SurvivalTimer => timer;
    private int pickedUp;
    public int PickedUp => pickedUp;
    private bool skipTimer = true;

    [Header("Audio")]
    [SerializeField] private AudioClip breakClip;
    [SerializeField] private AudioClip breakClip2;
    [SerializeField] private AudioClip explodeClip;
    [SerializeField] private AudioClip ringExplodeClip;
    [SerializeField] private AudioSource rocketSource;
    [SerializeField] private AudioClip spawnCometClip;
    [SerializeField] private AudioClip spawnPickUpClip;

    [Header("Alien Clips")]
    [SerializeField] private AudioClip[] alienClips;
    [SerializeField] private Vector2 minMaxAlienClipTimer;
    [SerializeField] private Vector2 minMaxTimeAfterStarExplodeToPlayClip;
    [SerializeField] private Vector2 chanceToPlayClip;
    private float alienClipTimer;
    private AudioClip lastAlienClip;
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private CinemachineVirtualCamera gameOverVCam;
    [SerializeField] private CinemachineVirtualCamera endGameVCam;
    [SerializeField] private CinemachineBrain cinemachineBrain;


    private void Start()
    {
        alienClipTimer = UnityEngine.Random.Range(minMaxAlienClipTimer.x, minMaxAlienClipTimer.y);
        pickUpGrowGoal += pickUpsBeforeAllowMorePickUpsAtOnce;

        // Reset
        pickUpsAlive.Reset();
        pickUpSpawnAtDistance.Reset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMaskHelper.IsInLayerMask(other.gameObject, pickUpLayer))
        {
            CanPickUp(other.GetComponent<PickUpTrigger>().PickUp, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (LayerMaskHelper.IsInLayerMask(other.gameObject, pickUpLayer))
        {
            CanPickUp(other.GetComponent<PickUpTrigger>().PickUp, false);
        }
    }

    private new void Update()
    {
        // Update Position
        playerPosition.Value = transform.position;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Resume();
            else if (!paused) Pause();
        }

        base.Update();

        if (gameOvered) return;

        if (gameActive)
        {
            timeBetweenPickUpSpawnsTimer -= Time.deltaTime;
            if (skipTimer)
            {
                for (int i = 0; i < numPickUpsAllowedAtOnce; i++)
                {
                    SpawnPickUp();
                }
                skipTimer = false;
            }
            else
            {
                if (pickUpsAlive.Value < numPickUpsAllowedAtOnce && timeBetweenPickUpSpawnsTimer <= 0)
                {
                    SpawnPickUp();
                }
            }
        }

        alienClipTimer -= Time.deltaTime;
        if (alienClipTimer <= 0)
        {
            AudioManager._Instance.PlayOneShotFromAlienClipChannel(GetAlienClip());
            alienClipTimer = UnityEngine.Random.Range(minMaxAlienClipTimer.x, minMaxAlienClipTimer.y);
        }

        if (gameActive)
        {
            timeBtweenComets -= Time.deltaTime * timeBtweenCometsReductionRate;
        }

        // Pick Up
        if (canPickUp.Count > 0)
        {
            pickUpInstructText.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                foreach (PickUp pickUp in canPickUp)
                {
                    PickUp(pickUp);
                }
            }
        }
        else
        {
            pickUpInstructText.gameObject.SetActive(false);
        }

        if (lockMovement) return;

        // Move
        Movement();

        if (Input.GetKey(KeyCode.LeftShift))
            DropSpeed(true);

        if (Input.GetKeyDown(KeyCode.Space) && !gameActive)
            StartGame();
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        velocityText.text = "Velocity: " + velocity.ToString();
        accelerationText.text = "Acceleration: " + acceleration.ToString();
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;

        // Spawn Particles
        foreach (ParticleSizeInfo info in particlesOnDestroy)
        {
            Instantiate(info.Particle, transform.position, Quaternion.identity).transform.localScale *= info.ScaleModifier;
        }
        FindObjectOfType<GameOverCanvas>().GameOver();
    }

    private IEnumerator Game()
    {
        gameActive = true;
        StartCoroutine(SpawnComets(true));

        pickUpText.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        nextCometText.gameObject.SetActive(true);
        startGameText.gameObject.SetActive(false);

        pickUpText.text = "Stars Collapsed: " + pickedUp.ToString();

        startTextAnim.CrossFade("AppearThenFade", 0, 0);

        while (true)
        {
            timer += Time.deltaTime;
            timerText.text = "Seconds Survived: " + Mathf.RoundToInt(timer).ToString();
            yield return null;
        }
    }


    private void StartGame()
    {
        StartCoroutine(Game());
        DropSpeed(false);
    }

    public void StartExplode()
    {
        gameOvered = true;
        // Destroy Things
        ArrowPointer[] arrows = FindObjectsOfType<ArrowPointer>();
        foreach (ArrowPointer arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }

        for (int i = 0; i < rings.Length; i++)
        {
            Ring current = rings[i];
            current.transform.parent = null;
            current.SetVelocity(velocity);
            current.AddForce(UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(minMaxRingFloatSpeed.x, minMaxRingFloatSpeed.y));
        }
        StartCoroutine(DestroySequence());
    }

    private IEnumerator DestroySequence()
    {
        vCam.gameObject.SetActive(false);
        gameOverVCam.gameObject.SetActive(true);


        for (int i = 0; i < rings.Length; i++)
        {
            yield return new WaitForSeconds(timeBtweenDestroys);
            AudioManager._Instance.PlayOneShot(ringExplodeClip, UnityEngine.Random.Range(0.6f, 1f), rings[i].transform.position);
            Destroy(rings[i].gameObject);
        }

        yield return new WaitForSeconds(timeBtweenDestroys);
        AudioManager._Instance.PlayOneShot(explodeClip, UnityEngine.Random.Range(0.9f, 1.3f), transform.position);

        EnableEndGameVCam();

        Destroy(gameObject);
    }

    private void EnableEndGameVCam()
    {
        gameOverVCam.gameObject.SetActive(false);
        endGameVCam.gameObject.SetActive(true);
    }

    private void SpawnPickUp()
    {

        // Spawn Pick Up
        PickUp spawnedPickUp = Instantiate(pickUpPrefab[UnityEngine.Random.Range(0, pickUpPrefab.Length)],
            transform.position + (UnityEngine.Random.onUnitSphere * pickUpSpawnAtDistance.Value), Quaternion.identity);
        AudioManager._Instance.PlayOneShot(spawnPickUpClip, UnityEngine.Random.Range(0.7f, 1.3f), spawnedPickUp.transform.position);

        // Spawn Arrow
        ArrowPointer spawnedStarArrow = Instantiate(arrowPointerPrefab, transform.position, Quaternion.identity);
        spawnedStarArrow.SetPointAt(transform, spawnedPickUp.transform, spawnedPickUp.ArrowMat);
        spawnedPickUp.SetRepArrow(spawnedStarArrow.gameObject);

        timeBetweenPickUpSpawnsTimer = timeBetweenPickUpSpawns;
    }

    public void PickUp(PickUp pickUp)
    {
        DropSpeed(false);
        pickUp.ExecutePickUp(() =>
        {
            pickUpText.text = "Stars Collapsed: " + (++pickedUp).ToString();

            if (canPickUp.Contains(pickUp))
                canPickUp.Remove(pickUp);

            if (pickedUp >= pickUpGrowGoal)
            {
                numPickUpsAllowedAtOnce += 1;
                pickUpGrowGoal += pickUpsBeforeAllowMorePickUpsAtOnce;
            }
            pickUpSpawnAtDistance.Value += pickUpSpawnDistanceGrowth;

            timeBetweenPickUpSpawnsTimer = timeBetweenPickUpSpawns;

            // Chance to play clip

            if (UnityEngine.Random.Range(chanceToPlayClip.x, chanceToPlayClip.y) < chanceToPlayClip.x)
            {
                PlayAlienClipAfterTime(UnityEngine.Random.Range(minMaxTimeAfterStarExplodeToPlayClip.x, minMaxTimeAfterStarExplodeToPlayClip.y));
            }

            cometSizeBoost += cometSizeBoostGrow;
        });
    }

    private IEnumerator PlayAlienClipAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        AudioManager._Instance.PlayOneShotFromAlienClipChannel(GetAlienClip());
    }

    public void CanPickUp(PickUp pickUp, bool can)
    {
        if (can)
        {
            if (canPickUp.Contains(pickUp)) return;
            canPickUp.Add(pickUp);
        }
        else
        {
            if (!canPickUp.Contains(pickUp)) return;
            canPickUp.Remove(pickUp);
        }
    }

    private IEnumerator SpawnComets(bool first)
    {
        float waitDuration = first ? timebeforeFirstComets : (timeBtweenComets > minTimeBetweenComet ? timeBtweenComets : minTimeBetweenComet);

        for (float i = 0; i < waitDuration; i += Time.deltaTime)
        {
            nextCometText.text = "Next Comet in: " + Mathf.RoundToInt(waitDuration - i).ToString();
            yield return null;
        }

        SpawnComet();

        StartCoroutine(SpawnComets(false));
    }

    private void SpawnComet()
    {
        Comet comet = Instantiate(cometPrefab, transform.position + (UnityEngine.Random.onUnitSphere * cometSpawnAtDistance), Quaternion.identity);
        AudioManager._Instance.PlayOneShot(spawnCometClip, UnityEngine.Random.Range(0.7f, 1.3f), comet.transform.position);
        comet.transform.localScale *= cometSizeBoost;
        ArrowPointer spawned = Instantiate(dangerPointerPrefab, transform.position, Quaternion.identity);
        spawned.SetPointAt(transform, comet.transform, cometMaterial);
        comet.Set(transform, () =>
        {
            if (spawned != null)
                Destroy(spawned.gameObject);
        });

        Vector3 direction = transform.position - comet.transform.position;
        // Add force to comet
        Vector3 force = (direction * cometSpeed);
        comet.AddForce(force);

    }

    private void Movement()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            direction -= Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Camera.main.transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Camera.main.transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction -= Camera.main.transform.right;
        }

        if (direction != Vector3.zero)
        {
            rocketSource.enabled = true;
            // Spawn Flame
            Flame flame = Instantiate(flamePrefab, transform.position, Quaternion.identity);
            // Add force to flame
            Vector3 force = direction * flameSpeed;
            flame.Set(velocity, force, this);
            SpawnSmoke(velocity, force, numSmokePerFlame);
        }
        else if (!lockMovement)
        {
            rocketSource.enabled = false;
        }
    }

    private void SpawnSmoke(Vector3 velocity, Vector3 force, int numToSpawn)
    {
        for (int i = 0; i < numToSpawn; i++)
        {
            Smoke spawned = Instantiate(smokePrefab, transform.position, Quaternion.identity);
            spawned.Set(velocity, force);
        }
    }

    private void DropSpeed(bool checkMinAcceleration)
    {
        if (lockMovement) return;
        StartCoroutine(ExecuteDropSpeed(checkMinAcceleration));
    }


    private IEnumerator ExecuteDropSpeed(bool checkMinAcceleration)
    {
        bool wentThrough = false;
        float x = Mathf.Abs(acceleration.x);
        float y = Mathf.Abs(acceleration.y);
        float z = Mathf.Abs(acceleration.z);

        if (checkMinAcceleration && (x < minAccelerationToBrake && y < minAccelerationToBrake && z < minAccelerationToBrake)) yield break;

        float startVolume = rocketSource.volume;

        Flame flame;
        while (x > minAccelerationToStopBrake || y > minAccelerationToStopBrake || z > minAccelerationToStopBrake)
        {
            if (wentThrough == false)
            {
                lockMovement = true;

                // Audio
                AudioManager._Instance.PlayOneShot(breakClip, UnityEngine.Random.Range(0.9f, 1.1f), transform.position);
                wentThrough = true;
            }

            x = Mathf.Abs(acceleration.x);
            y = Mathf.Abs(acceleration.y);
            z = Mathf.Abs(acceleration.z);

            // Audio
            rocketSource.enabled = true;
            float vol = 0;
            // X Direction
            if (x > minAccelerationToStopBrake)
            {
                // Spawn Flame
                flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
                Vector3 force = acceleration.x * reduceAccelerationSpeed * Vector3.right;
                // Add force to flame
                flame.Set(velocity, force, this);
                SpawnSmoke(velocity, force, Mathf.CeilToInt(numSmokePerFlame / 3f));
                vol += startVolume / 3;
            }

            if (y > minAccelerationToStopBrake)
            {
                // Y Direction
                // Spawn Flame
                flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
                Vector3 force = acceleration.y * reduceAccelerationSpeed * Vector3.up;
                // Add force to flame
                flame.Set(velocity, force, this);
                SpawnSmoke(velocity, force, Mathf.CeilToInt(numSmokePerFlame / 3f));
                vol += startVolume / 3;
            }

            if (z > minAccelerationToStopBrake)
            {
                // Z Direction
                // Spawn Flame
                flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
                Vector3 force = acceleration.z * reduceAccelerationSpeed * Vector3.forward;
                // Add force to flame
                flame.Set(velocity, force, this);
                SpawnSmoke(velocity, force, Mathf.CeilToInt(numSmokePerFlame / 3f));
                vol += startVolume / 3;
            }

            rocketSource.volume = vol;

            yield return null;
        }

        if (!wentThrough) yield break;

        // Audio
        rocketSource.volume = startVolume;
        rocketSource.enabled = false;

        // Set velocity to smaller number in same direction
        SetVelocity(-velocity.normalized);

        // Cancel out remaining force to cause acceleration to drop to 0
        // X
        // Spawn Flame
        flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
        // Add force to flame
        flame.Set(velocity, forceSum.x * Vector3.right, this);

        // Y
        // Spawn Flame
        flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
        // Add force to flame
        flame.Set(velocity, forceSum.y * Vector3.up, this);

        // Z
        // Spawn Flame
        flame = Instantiate(antiFlamePrefab, transform.position, Quaternion.identity);
        // Add force to flame
        flame.Set(velocity, forceSum.z * Vector3.forward, this);

        AudioManager._Instance.PlayOneShot(breakClip2, UnityEngine.Random.Range(0.8f, 1.2f), transform.position);

        // Particle
        Instantiate(stopVelocityParticle, transform.position, Quaternion.identity);

        // Allow player to move again
        lockMovement = false;
    }

    private AudioClip GetAlienClip()
    {
        // Code is to prevent the same clip from appearing twice in a row
        if (lastAlienClip != null)
        {
            AudioClip alienClip = lastAlienClip;
            while (alienClip == lastAlienClip)
            {
                alienClip = alienClips[UnityEngine.Random.Range(0, alienClips.Length)];
            }
            lastAlienClip = alienClip;
            return alienClip;
        }
        else
        {
            return alienClips[UnityEngine.Random.Range(0, alienClips.Length)];
        }
    }

    private void Pause()
    {
        paused = true;
        Time.timeScale = 0;
        PauseCamera();
        pauseCanvas.SetActive(true);
    }

    private void Resume()
    {
        paused = false;
        Time.timeScale = 1;
        ResumeCamera();
        pauseCanvas.SetActive(false);
    }

    private void PauseCamera()
    {
        cinemachineBrain.enabled = false;
    }

    private void ResumeCamera()
    {
        cinemachineBrain.enabled = true;
    }

}
