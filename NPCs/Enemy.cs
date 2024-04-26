using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public UnitData unitData;
    [SerializeField] private Transform player;
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private NavMeshAgent EnemyNavMeshAgent;

    public GameObject[] ItemsDropped;

    [SerializeField] private AudioSource EnemyAudioSource;
    [SerializeField] private AudioSource TerrorRadius;
    [SerializeField] private AudioClip Footsteps;
    [SerializeField] private AudioClip AlertScream;
    [SerializeField] private AudioClip DamageScream;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioClip Swing;
    [SerializeField] private AudioClip DeathSound;

    [SerializeField] private GameObject playerCharacter;
    [SerializeField] private GameObject destinationGameObject;
    [SerializeField] private GameObject FireTeleportSpell;
    [SerializeField] private GameObject GoreExplosion;
    [SerializeField] private GameObject enemyBody;

    private MeshRenderer meshRenderer;
    public Material newMaterial;

    public Collider EnemyCollider;

    [SerializeField] private ParticleSystem BloodParticles;

    [SerializeField] private GameObject DeathGore;

    [SerializeField]
    private enum EnemyState
    {
        Waiting,
        Chasing,
        Attacking,
        Stunned,
        Dead
    }

    private EnemyState CurrentEnemyState;

    [Range(0, 360)]
    public float viewAngle;

    public float coneRadius = 2.0f;
    public float coneLength = 5.0f;

    private LayerMask hitMask;
    private LayerMask blockedMask;

    public List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();

    private bool dead;
    private bool InRange = false;
    private bool InLineOfSight;
    private bool CanMove = false;
    private Transform _targetPosition;

    private float MeleeRange = 5f;
    private float RangedRange = 5f;

    public bool canBeHarmed;
    public bool stealthed;
    public float cooldown;
    public float soundVolume = 0.5f;
    private int EnemyDamage = 10;
    public float Health = 10f;
    private float stunDuration = 3f;
    private float refreshRate = 0.5f;
    public float NormalMovementSpeed = 9f;
    private float MovementSpeed = 9f;
    private bool isAttacking = false;
    public bool canExplode = false;
    private bool shouldPlaySound = true;

    public bool Initialized { get; private set; }
    public GameObject AlertObject;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        EnemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        EnemyAudioSource = gameObject.GetComponent<AudioSource>();
        enemyAnimator = gameObject.GetComponentInChildren<Animator>();
        EnemyCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        EnemyDamage = unitData.damage;
        Health = unitData.health;
        MovementSpeed = unitData.movementSpeed;
        cooldown = unitData.attackCooldown;
        NormalMovementSpeed = unitData.movementSpeed;
        EnemyNavMeshAgent.speed = MovementSpeed;
        stealthed = unitData.Stealthed;
        RangedRange = unitData.Range;

        if (unitData != null)
        {
            if (stealthed)
            {
                newMaterial = unitData.ShimmerMaterial;

                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].material = newMaterial;
                }
            }
            else
            {
                newMaterial = unitData.EnemyMaterial;

                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].material = newMaterial;
                }
            }
        }

        InvokeRepeating("CheckLoS", 1.0f, 1.0f);
    }

    void Update()
    {
        if (CurrentEnemyState == EnemyState.Dead)
        {
            EnemyCollider.enabled = false;
            StopAllCoroutines();
            return;
        }

        if (CurrentEnemyState == EnemyState.Stunned)
        {
            EnemyNavMeshAgent.speed = 0;
            InRange = false;
            EnemyNavMeshAgent.SetDestination(transform.position);
            StopCoroutine(AttackCoroutine());
            return;
        }
        else
        {
            EnemyNavMeshAgent.speed = NormalMovementSpeed;
        }

        if (_targetPosition != null && CanMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_targetPosition.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 6 * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
        }

        if (Natia.Instance.Dead || PlayerControllerScript.Instance.Dead)
        {
            StopAllCoroutines();
            return;
        }

        DetermineTarget();

        MoveToNewPosition();

        float distanceToTarget = Vector3.Distance(EnemyNavMeshAgent.transform.position, _targetPosition.position);

        if (unitData.AttackType == UnitData.EnemyAttackType.Melee)
        {
            if (distanceToTarget <= MeleeRange)
            {
                InRange = true;
            }
            else
            {
                InRange = false;

                if (CurrentEnemyState != EnemyState.Stunned)
                {
                    CurrentEnemyState = EnemyState.Chasing;
                }
            }

            if (CurrentEnemyState == EnemyState.Chasing)
            {
                Attack();
            }
        }
        else
        {
            if (distanceToTarget <= RangedRange)
            {
                InRange = true;
            }
            else
            {
                InRange = false;

                if (CurrentEnemyState != EnemyState.Stunned)
                {
                    CurrentEnemyState = EnemyState.Chasing;
                }
            }

            if (CurrentEnemyState == EnemyState.Chasing)
            {
                Attack();
            }
        }
    }

    void MoveToNewPosition()
    {
        if (CanMove)
        {
            DetermineTarget();
            EnemyNavMeshAgent.destination = _targetPosition.transform.position;
            EnemyNavMeshAgent.stoppingDistance = unitData.StoppingDistance;
        }
    }

    public void Attack()
    {
        if (InRange && CurrentEnemyState != EnemyState.Attacking && !isAttacking && CanMove)
        {
            switch (unitData.AttackType)
            {
                case UnitData.EnemyAttackType.Melee:
                    StartCoroutine(AttackCoroutine());
                    return;
                case UnitData.EnemyAttackType.Ranged:
                    StartCoroutine(ThrowProjectile());
                    break;
                case UnitData.EnemyAttackType.InstantKill:
                    if (!PlayerControllerScript.Instance.Dead)
                    {
                        TerrorRadius.Stop();
                        UIManager.Instance.PlayJumpScare();
                    }
                    break;
            }
        }
    }

    public void StunEnemy()
    {
        StartCoroutine(StunnedCoroutine());
    }

    private IEnumerator StunnedCoroutine()
    {
        CurrentEnemyState = EnemyState.Stunned;

        if (stealthed)
        {
            newMaterial = unitData.EnemyMaterial;

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].material = newMaterial;
            }

            stealthed = false;
        }
        enemyAnimator.SetBool("Flashed", true);
        yield return new WaitForSeconds(stunDuration);
        enemyAnimator.SetBool("Flashed", false);
        CurrentEnemyState = EnemyState.Chasing;
    }

    public IEnumerator ThrowProjectile()
    {
        isAttacking = true;
        CurrentEnemyState = EnemyState.Attacking;

        while (InRange && InLineOfSight)
        {
            EnemyAudioSource.PlayOneShot(AttackSound, 0.7f);
            enemyAnimator.SetBool("Attack", true);

            yield return new WaitForSeconds(0.5f);

            MovementSpeed = 0;
            EnemyNavMeshAgent.speed = MovementSpeed;

            GameObject Projectile = Instantiate(unitData.ThrownProjectile, transform.position + new Vector3(0, 2, 0), transform.rotation);
            yield return new WaitForSeconds(unitData.attackCooldown);
        }

        MovementSpeed = NormalMovementSpeed;
        EnemyNavMeshAgent.speed = MovementSpeed;
        isAttacking = false;
    }

    public void DetermineTarget()
    {
        if (Vector3.Distance(transform.position, PlayerControllerScript.Instance.transform.position) > Vector3.Distance(transform.position, Natia.Instance.transform.position))
        {
            _targetPosition = Natia.Instance.transform;
        }
        else
        {
            _targetPosition = PlayerControllerScript.Instance.transform;
        }
    }

    public IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        CurrentEnemyState = EnemyState.Attacking;

        while (InRange)
        {
            EnemyAudioSource.PlayOneShot(AttackSound, 0.7f);
            enemyAnimator.SetBool("Attack", true);

            MovementSpeed = 0;
            EnemyNavMeshAgent.speed = MovementSpeed;

            Vector3 coneDirection = transform.forward;

            yield return new WaitForSeconds(0.5f);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, unitData.AttackRadius, coneDirection, coneLength);
            List<GameObject> hitObjects = new List<GameObject>();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject != gameObject)
                {
                    hitObjects.Add(hit.collider.gameObject);
                }
            }

            foreach (GameObject hitObject in hitObjects)
            {
                if (hitObject.CompareTag("Player") && InRange)
                {
                    hitObject.GetComponent<PlayerControllerScript>().TakeDamage(EnemyDamage);
                }
                else if (hitObject.CompareTag("Natia") && InRange)
                {
                    hitObject.GetComponent<Natia>().TakeDamage(EnemyDamage);
                }
            }

            enemyAnimator.SetBool("Attack", false);


            hitObjects.Clear();

            yield return new WaitForSeconds(cooldown);
        }

        MovementSpeed = NormalMovementSpeed;
        EnemyNavMeshAgent.speed = MovementSpeed;
        isAttacking = false;
    }

    public void CheckLoS()
    {
        Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

        if (cone.Length != 0 && !CanMove)
        {
            foreach (var hitCollider in cone)
            {
                var target = hitCollider.GetComponent<PlayerControllerScript>();
                var Natia = hitCollider.GetComponent<Natia>();
                Transform targetTransform = hitCollider.GetComponent<Transform>();
                Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && target || Vector3.Angle(transform.forward, targetDirection) < viewAngle / 2 && Natia)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                    if (!Physics.Raycast(transform.position, targetDirection, distanceToTarget, blockedMask))
                    {
                        InLineOfSight = true;
                        if (unitData.UnitMobilityType == UnitData.UnitType.Demon)
                        {
                            //enemyAnimator.SetBool("Walking", true);
                        }
                        else
                        {
                            EnemyAudioSource.PlayOneShot(AlertScream, 0.5f);
                        }
                        enemyAnimator.SetBool("Awaken", true);

                        StartCoroutine(AwakeCoroutine());
                    }
                    else
                    {
                        InLineOfSight = false;
                    }

                }
            }
        }
    }

    public void TakeDamage(float Damage, bool ShouldExplode)
    {
        if (canBeHarmed)
        {
            Health -= (int)Damage;
            GameObject Blood = Instantiate(BloodParticles.gameObject, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
            Destroy(Blood, 2f);

            if (stealthed)
            {
                newMaterial = unitData.EnemyMaterial;

                for (int i = 0; i < renderers.Count; i++)
                {
                    renderers[i].material = newMaterial;
                }

                stealthed = false;
            }

            if (Health <= 0 && !dead)
            {
                StartCoroutine(DeathCoroutine());

                if (ShouldExplode && canExplode)
                {
                    Explode();
                }
            }
            else
            {
                if (shouldPlaySound)
                {
                    EnemyAudioSource.PlayOneShot(DamageScream, soundVolume);
                    StartCoroutine(SoundCooldown());
                }
            }
        }
    }

    private IEnumerator SoundCooldown()
    {
        shouldPlaySound = false;
        yield return new WaitForSeconds(0.5f);
        shouldPlaySound = true;
    }

    private IEnumerator AwakeCoroutine()
    {
        yield return new WaitForSeconds(1);
        canBeHarmed = true;
        CanMove = true;
        enemyAnimator.SetBool("Walking", true);
        CurrentEnemyState = EnemyState.Chasing;
    }

    private IEnumerator DeathCoroutine()
    {
        dead = true;
        EnemyAudioSource.PlayOneShot(DeathSound, soundVolume);
        CurrentEnemyState = EnemyState.Dead;
        if (DeathGore != null)
        {
            GameObject BloodExplosion = Instantiate(DeathGore, transform.position + new Vector3(0, 2, 0), transform.rotation);
            Destroy(BloodExplosion, 2f);
        }
        if (ItemsDropped != null)
        {
            for (int i = 0; i < ItemsDropped.Length; i++)
            {
                if (ItemsDropped[i] != null)
                {
                    GameObject ItemDrop = Instantiate(ItemsDropped[i], transform.position + new Vector3(0, 5, 0), transform.rotation);
                    ItemDrop.GetComponent<Rigidbody>().AddForce(transform.forward * 5, ForceMode.Impulse);
                }
            }
        }
        enemyAnimator.SetBool("Death", true);
        EnemyAudioSource.clip = null;
        yield return new WaitForSeconds(2);
    }

    public void Explode()
    {
        GameObject GoreExplosionEffect = Instantiate(GoreExplosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
