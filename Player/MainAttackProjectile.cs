using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MainAttackProjectile : MonoBehaviour
{
    [Header("Logistics")]
    [SerializeField] public Rigidbody ProjectileRigidbody;
    [SerializeField] public ParticleSystem ProjectileParticleSystem;
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private GameObject MuzzleFlashPrefab;
    [SerializeField] private GameObject damageTarget;
    [SerializeField] private GameObject arrowExplosionPrefab;
    [SerializeField] public GameObject arrowHead;
    [SerializeField] public GameObject player;
    [SerializeField] public Vector3 fallDirection = new Vector3(0, -2, 0);
    [SerializeField] public GameObject BloodSplat;
    [SerializeField] public Light ProjectilePointLight;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] public Vector3 direction;
    public static int IgnoreRaycastLayer;

    [Header("Conditions")]
    private bool prematureExplosion = false;
    private bool onGround = false;
    private bool bouncer = false;
    private bool fuseStarted = false;
    private bool detonationOccured = false;
    private bool inPlayerRange = false;
    private bool knifeBounce = true;
    public bool randomSpread = false;
    public bool shouldFall = false;

    [Header("Stats")]
    [SerializeField] private int grenadeDamage = 10;
    public float LifeTime = 5f;
    public float power = 0.5f;
    private float grenadeHeight = 2;
    private float fallSpeed = 0.1f;
    private float fuseTimer = 0.3f;
    [SerializeField] private float projectileSpeed = 50f;
    private float explosionRadius = 8;
    private float explosionForce = 0;
    public float spreadAngleRange = 3f; // Adjust this value to control the spread angle
    public float originalIntensity = 1f;

    void Start()
    {
        originalIntensity = ProjectilePointLight.intensity;
        player = GameObject.FindGameObjectWithTag("Player");
        ProjectileRigidbody = gameObject.GetComponent<Rigidbody>();
        DontDestroyOnLoad(this);
    }

    public void Activate()
    {
        ProjectileParticleSystem.Play();
        ProjectilePointLight.intensity = originalIntensity; 
        detonationOccured = false;
        fuseStarted = false;
        ProjectileRigidbody.isKinematic = false;
        gameObject.SetActive(true);
        Invoke("Deactivate", 1.0f);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetRayCast()
    {
        Vector3 screenCenter = new Vector3(0.5f * Screen.width, 0.5f * Screen.height, 0);

        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit) && hit.distance > 10f)
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(40);
        }

        direction = targetPoint - transform.position;

        Debug.DrawRay(ray.origin, direction.normalized * (hit.distance > 10f ? hit.distance : 40), Color.red, 2f);

        float spreadAngleX = Random.Range(-spreadAngleRange, spreadAngleRange);
        float spreadAngleY = Random.Range(-spreadAngleRange, spreadAngleRange);

        Quaternion spreadRotation = Quaternion.Euler(spreadAngleX, spreadAngleY, 0f);
        Vector3 spreadDirection = spreadRotation * direction.normalized;

        ProjectileRigidbody.AddForce(spreadDirection * projectileSpeed * power, ForceMode.Impulse);
    }

    void Update()
    {
        if (shouldFall)
        {
            onGround = Physics.Raycast(transform.position, Vector3.down, grenadeHeight / 10f);
            ProjectileRigidbody.AddForce(fallDirection * fallSpeed * 0.3f, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && detonationOccured == false || other.CompareTag("Natia") && detonationOccured == false)
        {
            ProjectileRigidbody.isKinematic = true;
            ProjectileParticleSystem.Stop();
            StartCoroutine(FadeOut());
            GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider near in colliders)
            {

                Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

                if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
                {
                    targetRigidbodies.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                }
            }

            var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
            {
                var enemy = hitCollider.GetComponent<Enemy>();
                var friendly = hitCollider.GetComponent<Natia>();
                if (enemy)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);

                    var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

                    if (enemy)
                    {
                        enemy.TakeDamage(grenadeDamage, true);
                    }
                }

                if (friendly)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);

                    var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

                    friendly.TakeDamage(grenadeDamage);
                }
            }
        }

        if (other.tag == "Environment" && fuseStarted == false)
        {
            ProjectileRigidbody.isKinematic = true;
            ProjectileParticleSystem.Stop();
            StartCoroutine(FadeOut());
            GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider near in colliders)
            {
                Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

                if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
                {
                    targetRigidbodies.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                }
            }
            fuseStarted = true;
        }

        if (other.tag == "Player")
        {
            inPlayerRange = true;
        }

    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = ProjectilePointLight.intensity;

        while (elapsedTime < fadeDuration)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / fadeDuration);

            ProjectilePointLight.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ProjectilePointLight.intensity = 0f;
    }

    public IEnumerator DetonationSequence()
    {
        if (fuseStarted == false)
        {
            fuseStarted = true;

            yield return new WaitForSeconds(fuseTimer);

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider near in colliders)
            {
                Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

                if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
                {
                    targetRigidbodies.AddExplosionForce(explosionForce / 2, transform.position, explosionRadius, 1f, ForceMode.Impulse);
                }
            }

            var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (var hitCollider in hitColliders)
            {
                var enemy = hitCollider.GetComponent<EnemyClass>();
                if (enemy)
                {
                    var closestPoint = hitCollider.ClosestPoint(transform.position);
                    var distance = Vector3.Distance(closestPoint, transform.position);

                    var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

                    enemy.TakeDamage(grenadeDamage / 3);

                }
            }

            GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Deactivate();
        }

    }
}
