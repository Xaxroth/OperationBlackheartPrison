using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyClass : MonoBehaviour
{
    public Vector3 fallDirection = new Vector3(0, -2, 0);
    public Vector3 jumpDirection = new Vector3(0, -2, 0);

    public Transform player;
    public Animator enemyAnimator;
    public GameObject playerCharacter;
    public AudioSource EnemyAudioSource;
    public AudioClip BellRinging;
    public AudioClip EnemyAlert;
    public GameObject destinationGameObject;
    public GameObject FireTeleportSpell;
    public NavMeshAgent EnemyNavMeshAgent;
    public GameObject enemyBody;

    public SphereCollider HeadShotHitBox;

    public Collider enemyHitbox;

    public GameObject teleportingFire;
    public GameObject dingDongBell;

    public GameObject[] EnemyWaypoints;
    public int currentWaypoint = 0;
    public float waypointRadius = 1;
    public AudioClip[] DeathSounds;
    public bool pursuing = false;
    public bool enemyDead = false;
    public bool spotted = false;
    public bool teleporting = false;
    public bool inLineOfSight = false;
    public bool throwingFireBalls = false;
    public bool interrupted = false;
    public bool lookingForPlayer = false;
    public bool canBeAffectedByPhysics = false;
    public bool resetEnemy = false;
    public bool pathCreated;
    public bool prematureHurt = false;

    public int zPos;
    public int xPos;

    public static int timesBellRung = 0;

    Quaternion targetRotation;
    Vector3 rotationDirection;
    private float rotationSpeed = 0.03f;

    public GameObject[] LookDirections;

    [SerializeField] private int enemyCurrentHealth = 300;
    [SerializeField] private int enemyMaxHealth = 300;
    [SerializeField] private int speed = 10;
    public bool alert = false;
    public bool enemyOnGround = false;
    [SerializeField] private float enemyHeight = 2f;
    [SerializeField] private float enemyJumpHeight = 1f;

    //MONSTER ABILITIES//
    [SerializeField] private GameObject Fireball;
    [SerializeField] private Transform Shootposition;

    [SerializeField] private Transform playerPreviousPosition;

        [Range(0, 360)]
    public float viewAngle;

    public float coneRadius = 8;
    public LayerMask hitMask;
    public LayerMask blockedMask;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckLoS", 1f, 1f);
        InvokeRepeating("LineOfSight", 1f, 1f);
        EnemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();
        enemyHitbox = gameObject.GetComponent<CapsuleCollider>();
        canBeAffectedByPhysics = true;
        teleportingFire.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (!enemyDead)
        {
            enemyCurrentHealth -= damage;

            if (teleporting == false && !enemyDead && alert == true && enemyCurrentHealth > 0f)
            {
                StartCoroutine(Teleport());
            }

            if (!interrupted && alert == true)
            {
                interrupted = true;
            }

            if (alert == false)
            {
                prematureHurt = true;
                alert = true;
                StartCoroutine(ChasePlayer());
            }

            if (enemyCurrentHealth <= 0f)
            {
                enemyHitbox.enabled = false;
                StopAllCoroutines();
                StartCoroutine(DeathCoroutine());

                EnemyNavMeshAgent.speed = 0;
                enemyDead = true;
            }

        }
    }

    private IEnumerator DeathCoroutine()
    {
        enemyAnimator.SetBool("Death", true);
        EnemyAudioSource.clip = DeathSounds[Random.Range(0, DeathSounds.Length)];
        EnemyAudioSource.PlayOneShot(EnemyAudioSource.clip, 1.2f);
        yield return new WaitForSeconds(2.5f);
        GameObject teleportCircleDeath = Instantiate(FireTeleportSpell, transform.position - new Vector3(0, 2f, 0), FireTeleportSpell.transform.rotation);
        CollectorHunted.coroutineStarted = false;
        Destroy(gameObject);
    }

    void Update()
    {
        if (enemyDead)
        {
            return;
        }

        if (enemyDead == true)
        {
            speed = 0;
            EnemyNavMeshAgent.destination = transform.position;
        }

        if (spotted == true)
        {
            EnemyNavMeshAgent.speed = 9;
            alert = true;
            EnemyNavMeshAgent.destination = player.transform.position;
            spotted = true;
            speed = 2;
            pursuing = true;
            StartCoroutine(RingAlarmBell());
            StopCoroutine(ChasePlayer());
            prematureHurt = false;
        }


        if (interrupted == true && !teleporting)
        {
            dingDongBell.SetActive(false);
            enemyAnimator.SetBool("Walk", false);
            enemyAnimator.SetBool("Alert", true);
            enemyAnimator.SetBool("Combat", true);
        }

        if (EnemyNavMeshAgent.velocity == new Vector3(0, 0, 0))
        {
            enemyAnimator.SetBool("Walk", false);
        }
        else
        {
            if (pursuing == false)
            {
                enemyAnimator.SetBool("Walk", true);
            }
        }

        if (throwingFireBalls == false && interrupted == true)
        {
            throwingFireBalls = true;

            StartCoroutine(ThrowFireBall());
        }

        SearchingArea();
    }

    private IEnumerator ResetEnemyRoutine()
    {
        int timesTeleported = 0;

        yield return new WaitForSeconds(2);

        if (timesTeleported < 3)
        {
            if (inLineOfSight == false)
            {
                StartCoroutine(Teleport());
                timesTeleported++;
            }
            else
            {
                StartCoroutine(ThrowFireBall());
            }
        }
        else
        {
            StopAllCoroutines();
            alert = false;
        }

    }

    private void FixedUpdate()
    {
        PatrolRoute();
    }

    private void LineOfSight()
    {
        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();

                if (canBeAffectedByPhysics == true)
                {
                    Rigidbody targetRigidbodies = hitCollider.GetComponent<Rigidbody>();
                }


                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask))
                    {
                        if (alert == false)
                        {
                            StartCoroutine(ChasePlayer());
                        }
                        else
                        {

                        }
                    }

                }
                else
                {
                    
                }
            }
        }
    }

    public void CheckLoS()
    {
        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();
                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask))
                    {
                        inLineOfSight = true;
                    }
                    else
                    {
                        inLineOfSight = false;
                    }

                }
            }
        }
    }

    private void SearchingArea()
    {
        if (lookingForPlayer == true && inLineOfSight == true)
        {
            lookingForPlayer = false;
            StopCoroutine(LookForPlayer());
        }
    }

    private IEnumerator LookForPlayer()
    {
        pathCreated = false;
        EnemyNavMeshAgent.destination = transform.position + new Vector3(Random.Range(0, 5), 0, Random.Range(0, 5));

        lookingForPlayer = true;

        yield return new WaitForSeconds(60);

        lookingForPlayer = false;
        Debug.Log(lookingForPlayer);
    }

    public IEnumerator ThrowFireBall()
    {
        if (enemyDead == false)
        {
            enemyAnimator.speed = 1.4f;
            enemyAnimator.SetBool("Attack", true);
            yield return new WaitForSeconds(0.68f);
            enemyAnimator.speed = 1;
            enemyAnimator.SetBool("Attack", false);

            if (inLineOfSight == false && pursuing == true && !teleporting)
            {
                throwingFireBalls = false;

                if (lookingForPlayer == false)
                {
                    StartCoroutine(LookForPlayer());
                    lookingForPlayer = true;
                }
            }
            else
            {
                if (teleporting == false)
                {
                    enemyAnimator.SetBool("Attack", true);
                    GameObject currentJumpProjectile = Instantiate(Fireball, Shootposition.transform.position, Shootposition.rotation);
                    throwingFireBalls = true;
                    StartCoroutine(ThrowFireBall());
                }
            }

        }
      
    }

    public IEnumerator Teleport()
    {
        enemyHitbox.enabled = false;
        teleporting = true;

        enemyAnimator.SetBool("Attack", false);
        EnemyNavMeshAgent.speed = 0;

        yield return new WaitForSeconds(0.5f);

        enemyBody.SetActive(false);
        teleportingFire.SetActive(true);

        xPos = Random.Range(5, 20);
        zPos = Random.Range(5, 20);

        GameObject teleportCircle = Instantiate(FireTeleportSpell, transform.position - new Vector3(0, 2f, 0), FireTeleportSpell.transform.rotation);

        yield return new WaitForSeconds(0.5f);

        GameObject destination = Instantiate(destinationGameObject, transform.position + new Vector3(xPos, -2f, zPos), destinationGameObject.transform.rotation);

        EnemyNavMeshAgent.acceleration = 250;
        EnemyNavMeshAgent.speed = 300;
        EnemyNavMeshAgent.stoppingDistance = 0;

        EnemyNavMeshAgent.destination = destination.transform.position;

        yield return new WaitForSeconds(1f);

        EnemyNavMeshAgent.acceleration = 8;
        EnemyNavMeshAgent.speed = 9;
        EnemyNavMeshAgent.stoppingDistance = 10;

        EnemyNavMeshAgent.destination = player.transform.position;

        enemyBody.SetActive(true);
        teleportingFire.SetActive(false);

        enemyAnimator.SetBool("Attack", true);

        GameObject endTeleportCircle = Instantiate(FireTeleportSpell, transform.position - new Vector3(0, 2f, 0), FireTeleportSpell.transform.rotation);

        gameObject.transform.position = endTeleportCircle.transform.position;

        enemyHitbox.enabled = true;

        teleporting = false;

        if (pursuing == true && throwingFireBalls == false)
        {
            StartCoroutine(ThrowFireBall());
        }
    }

    public void EnemyAttracted()
    {
        coneRadius = 100;
        StartCoroutine(ChasePlayer());
    }

    private IEnumerator ChasePlayer()
    {
        alert = true;
        EnemyAudioSource.PlayOneShot(EnemyAlert);
        EnemyNavMeshAgent.speed = 0;

        Vector3 LookAtPosition = player.position;
        LookAtPosition.y = transform.position.y;
        transform.LookAt(LookAtPosition);

        yield return new WaitForSeconds(1.5f);

        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();

                if (canBeAffectedByPhysics == true)
                {
                    Rigidbody targetRigidbodies = hitCollider.GetComponent<Rigidbody>();
                }                    


                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask) || prematureHurt == true)
                    {
                        spotted = true;
                        speed = 2;
                        pursuing = true;
                        enemyAnimator.SetBool("Alert", true);
                        prematureHurt = false;
                        yield return new WaitForSeconds(0.7f);
                        StartCoroutine(RingAlarmBell());
                        StopCoroutine(ChasePlayer());
                    }

                }
                else
                {
                    speed = 2;
                }
            }
        }

        if (!spotted)
        {
            EnemyAudioSource.PlayOneShot(BellRinging);
            alert = false;
            StopCoroutine(ChasePlayer());
        }

        StopCoroutine(ChasePlayer());
    }

    private IEnumerator RingAlarmBell()
    {
        if (!interrupted && timesBellRung < 3)
        {
            enemyAnimator.SetBool("Alert", true);
            timesBellRung++;
            EnemyAudioSource.PlayOneShot(BellRinging, 1f);
            yield return new WaitForSeconds(1f);
            StartCoroutine(RingAlarmBell());
        }
        else
        {
            if (throwingFireBalls == false)
            {
                EnemyNavMeshAgent.speed = 9;
                interrupted = true;
                enemyAnimator.SetBool("Alert", true);
                enemyAnimator.SetBool("Combat", true);
                StartCoroutine(ThrowFireBall());
                yield return new WaitForSeconds(0.5f);
                enemyAnimator.SetBool("Walk", false);
                StopCoroutine(RingAlarmBell());
            }
        }
    }
   
    private void GroundCheck()
    {
        enemyOnGround = Physics.Raycast(transform.position, Vector3.down, enemyHeight / 2f + 0.05f);
    }

    private void PatrolRoute()
    {
        if (pursuing == false && alert == false)
        {
            if (Vector3.Distance(EnemyWaypoints[currentWaypoint].transform.position, transform.position) < waypointRadius)
            {
                currentWaypoint++;

                if (currentWaypoint >= EnemyWaypoints.Length)
                {
                    currentWaypoint = 0;
                }
            }

            EnemyNavMeshAgent.destination = EnemyWaypoints[currentWaypoint].transform.position;

            rotationDirection = (EnemyWaypoints[currentWaypoint].transform.position - transform.position).normalized;
        }
    }

}
