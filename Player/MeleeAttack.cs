using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [Header("Logistics")]
    [SerializeField] public KeyCode AttackButton;
    [SerializeField] private PlayerControllerScript _playerController;
    [SerializeField] private PlayerControllerScript player;

    private List<Enemy> hitEnemies = new List<Enemy>();
    private List<Enemy> othersHit = new List<Enemy>();

    [Header("Animation")]
    public Animator meleeAnimator;
    public Animator cameraAnimator;

    [Header("Cosmetics")]
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private ParticleSystem chainsawBlood;
    [SerializeField] private ParticleSystem screenBloodSplat;
    [SerializeField] private ParticleSystem WeaponTrail;
    [SerializeField] private AudioClip quickSwingSound;
    [SerializeField] private AudioClip angryGrunt;
    [SerializeField] private AudioClip bloodSplatSound;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private GameObject bloodBurst;
    [SerializeField] private Rigidbody playerRigidbody;

    public GameObject ImpactEffect;

    [SerializeField] private ParticleSystem AdrenalineParticles;
    [SerializeField] private ParticleSystem enemyHitParticles;

    [SerializeField] private AudioClip[] ChainsawSweepSounds;

    public LayerMask hitMask;
    public LayerMask blockedMask;

    [Header("Damage and Attributes")]

    [SerializeField] private int baseDamage = 25;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float coneRadius = 6;
    [SerializeField] private float LungeAttackForceModifier = 1.5f;

    [Header("Range")]
    [Range(0, 360)]
    public float viewAngle = 60;

    [Header("Conditions")]
    private int attackAnimationNumber;
    private float chargingLunge;
    private bool quickAttack;
    private bool lungeAttack;
    private bool vulnerable = true;
    private bool hit = true;
    public bool swinging = false;
    public bool canSwing = true;

    void Start()
    {
        player = gameObject.GetComponent<PlayerControllerScript>();
        playerAudioSource = gameObject.GetComponent<AudioSource>();
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (PlayerControllerScript.Instance.paralyzed)
        {
            return;
        }
        if (PlayerControllerScript.Instance != null && !PlayerControllerScript.Instance.Incapacitated)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                swinging = true;
            }
            else
            {
                swinging = false;
            }

            if (Input.GetMouseButtonDown(0) && canSwing)
            {
                StartCoroutine(CameraAnimation());
                StartCoroutine(MeleeQuickSwing());
            }

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                StopCoroutine(MeleeQuickSwing());
                StopCoroutine(MeleePowerSwing());
            }
        }
    }

    private void MeleeSwing()
    {
        if (swinging)
        {
            Collider[] cone = Physics.OverlapSphere(transform.position, coneRadius);

            Ray ray = new Ray(PlayerControllerScript.Instance.Orientation.transform.position + new Vector3(0, 2.5f, 0), PlayerControllerScript.Instance.Orientation.transform.forward);

            RaycastHit hit;
            float raycastDistance = 5f;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.ImpactSounds[Random.Range(0, AudioManager.Instance.ImpactSounds.Length)], 0.75f);
                GameObject impactEffect = Instantiate(ImpactEffect, hit.point, Quaternion.identity);
                Destroy(impactEffect, 3f);
            }

            if (cone.Length != 0)
            {
                foreach (var hitCollider in cone)
                {
                    if (hitCollider.gameObject.CompareTag("Enemy"))
                    {
                        Enemy enemy = hitCollider.GetComponent<Enemy>();
                        if (!hitEnemies.Contains(enemy))
                        {
                            enemy.TakeDamage(baseDamage, false);
                            hitEnemies.Add(enemy);

                            PlayerControllerScript.Instance.playerStamina += baseDamage;

                            Vector3 targetDirection = (enemy.transform.position - transform.position).normalized;
                            //AudioManager.Instance.PlaySound(AudioManager.Instance.GoreHitSounds[Random.Range(0, AudioManager.Instance.GoreHitSounds.Length)], 0.3f);
                        }
                    }

                    if (hitCollider.gameObject.CompareTag("Natia"))
                    {
                        Natia natia = hitCollider.GetComponent<Natia>();

                        if (natia != null)
                        {
                            natia.TakeDamage(baseDamage);
                        }
                    }
                }
            }
        }
    }

    IEnumerator MeleeQuickSwing()
    {
        while (swinging)
        {
            if (attackAnimationNumber >= 2)
            {
                attackAnimationNumber = 0;
            }

            playerAudioSource.clip = ChainsawSweepSounds[Random.Range(0, ChainsawSweepSounds.Length)];
            playerAudioSource.PlayOneShot(playerAudioSource.clip, 0.7f);

            StartCoroutine(CameraAnimation());

            attackAnimationNumber++;
            canSwing = false;

            yield return new WaitForSeconds(0.2f);

            MeleeSwing();

            yield return new WaitForSeconds(attackCooldown);

            hitEnemies.Clear();

            canSwing = true;
        }
    }

    IEnumerator MeleePowerSwing()
    {
        while (swinging)
        {
            attackAnimationNumber = 3;

            playerAudioSource.clip = ChainsawSweepSounds[Random.Range(0, ChainsawSweepSounds.Length)];
            playerAudioSource.PlayOneShot(playerAudioSource.clip, 0.7f);

            StartCoroutine(CameraAnimation());

            attackAnimationNumber++;

            canSwing = false;

            yield return new WaitForSeconds(0.1f);

            MeleeSwing();

            yield return new WaitForSeconds(attackCooldown * 3);

            hitEnemies.Clear();

            canSwing = true;
        }
    }

    private IEnumerator CameraAnimation()
    {
        meleeAnimator.SetInteger("AnimationNumber", attackAnimationNumber);

        if (attackAnimationNumber == 0)
        {
            meleeAnimator.SetBool("RightSwing", true);
            yield return new WaitForSeconds(0.1f);
            meleeAnimator.SetBool("RightSwing", false);
        }
        if (attackAnimationNumber == 1)
        {
            meleeAnimator.SetBool("LeftSwing", true);
            yield return new WaitForSeconds(0.1f);
            meleeAnimator.SetBool("LeftSwing", false);
        }
        if (attackAnimationNumber == 2)
        {
            meleeAnimator.SetBool("RightSwing", true);
            yield return new WaitForSeconds(0.1f);
            meleeAnimator.SetBool("RightSwing", false);
        }
        if (attackAnimationNumber == 3)
        {
            meleeAnimator.SetBool("HeavySwing", true);
            yield return new WaitForSeconds(0.1f);
            meleeAnimator.SetBool("HeavySwing", false);
        }

    }
}
