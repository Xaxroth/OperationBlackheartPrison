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
    [SerializeField] public GameObject player;
    [SerializeField] public GameObject BloodSplat;
    [SerializeField] public Vector3 fallDirection = new Vector3(0, -2, 0);
    [SerializeField] public Light ProjectilePointLight;
    [SerializeField] public Vector3 direction;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private LayerMask layerMask;
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
    [SerializeField] private int Damage = 10;
    [SerializeField] private int grenadeDamage = 10;
    [SerializeField] private float projectileSpeed = 50f;
    public float LifeTime = 5f;
    public float power = 0.5f;
    private float grenadeHeight = 2;
    private float fallSpeed = 0.1f;
    private float fuseTimer = 0.3f;
    private float explosionRadius = 8;
    private float explosionForce = 0;
    public float spreadAngleRange = 3f;
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
        //Invoke("Deactivate", 2.0f);
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

        Debug.DrawRay(ray.origin, direction.normalized * (hit.distance > 10f ? hit.distance : 80), Color.red, 2f);

        float spreadAngleX = Random.Range(-spreadAngleRange, spreadAngleRange);
        float spreadAngleY = Random.Range(-spreadAngleRange, spreadAngleRange);

        Quaternion spreadRotation = Quaternion.Euler(spreadAngleX, spreadAngleY, 0f);
        Vector3 spreadDirection = spreadRotation * direction.normalized;

        Ray spreadRay = new Ray(transform.position, spreadDirection);
        RaycastHit spreadHit;

        if (Physics.Raycast(spreadRay, out spreadHit, layerMask))
        {
            if (spreadHit.collider.gameObject.CompareTag("Enemy"))
            {
                Enemy spreadDamageable = spreadHit.collider.gameObject.GetComponent<Enemy>();

                if (spreadDamageable != null)
                {
                    spreadDamageable.TakeDamage(Damage, true);
                }
            }
            
            if (spreadHit.collider.gameObject.CompareTag("Natia"))
            {
                Natia natia = spreadHit.collider.gameObject.GetComponent<Natia>();

                if (natia != null)
                {
                    natia.TakeDamage(Damage);
                }
            }
        }

        Debug.DrawRay(ray.origin, spreadDirection.normalized * (hit.distance > 10f ? hit.distance : 80), Color.red, 2f);

        GameObject explosion = PlayerControllerScript.Instance.GetComponent<MainAttackScript>().GetPooledExplosion();

        if (explosion != null)
        {
            explosion.SetActive(true);
            explosion.GetComponentInChildren<ParticleSystem>().Play();
            explosion.transform.position = spreadHit.point;
        }

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

    #region DEPRECATED

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Enemy") && detonationOccured == false || other.CompareTag("Natia") && detonationOccured == false)
    //    {
    //        ProjectileRigidbody.isKinematic = true;
    //        ProjectileParticleSystem.Stop();
    //        StartCoroutine(FadeOut());
    //        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    //        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

    //        foreach (Collider near in colliders)
    //        {

    //            Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

    //            if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
    //            {
    //                targetRigidbodies.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
    //            }
    //        }

    //        var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
    //        foreach (var hitCollider in hitColliders)
    //        {
    //            var enemy = hitCollider.GetComponent<Enemy>();
    //            var friendly = hitCollider.GetComponent<Natia>();
    //            if (enemy)
    //            {
    //                var closestPoint = hitCollider.ClosestPoint(transform.position);
    //                var distance = Vector3.Distance(closestPoint, transform.position);

    //                var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

    //                if (enemy)
    //                {
    //                    enemy.TakeDamage(grenadeDamage, true);
    //                }
    //            }

    //            if (friendly)
    //            {
    //                var closestPoint = hitCollider.ClosestPoint(transform.position);
    //                var distance = Vector3.Distance(closestPoint, transform.position);

    //                var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

    //                friendly.TakeDamage(grenadeDamage);
    //            }
    //        }
    //    }

    //    if (other.tag == "Environment" && fuseStarted == false)
    //    {
    //        ProjectileRigidbody.isKinematic = true;
    //        ProjectileParticleSystem.Stop();
    //        StartCoroutine(FadeOut());
    //        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    //        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

    //        foreach (Collider near in colliders)
    //        {
    //            Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

    //            if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
    //            {
    //                targetRigidbodies.AddExplosionForce(explosionForce, transform.position, explosionRadius, 1f, ForceMode.Impulse);
    //            }
    //        }
    //        fuseStarted = true;
    //    }

    //    if (other.tag == "Player")
    //    {
    //        inPlayerRange = true;
    //    }

    //}
    //public IEnumerator DetonationSequence()
    //{
    //    if (fuseStarted == false)
    //    {
    //        fuseStarted = true;

    //        yield return new WaitForSeconds(fuseTimer);

    //        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

    //        foreach (Collider near in colliders)
    //        {
    //            Rigidbody targetRigidbodies = near.GetComponent<Rigidbody>();

    //            if (targetRigidbodies != null && targetRigidbodies.gameObject.tag != "Projectile")
    //            {
    //                targetRigidbodies.AddExplosionForce(explosionForce / 2, transform.position, explosionRadius, 1f, ForceMode.Impulse);
    //            }
    //        }

    //        var hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
    //        foreach (var hitCollider in hitColliders)
    //        {
    //            var enemy = hitCollider.GetComponent<EnemyClass>();
    //            if (enemy)
    //            {
    //                var closestPoint = hitCollider.ClosestPoint(transform.position);
    //                var distance = Vector3.Distance(closestPoint, transform.position);

    //                var damage = Mathf.InverseLerp(explosionRadius, 0, distance);

    //                enemy.TakeDamage(grenadeDamage / 3);

    //            }
    //        }

    //        GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
    //        Deactivate();
    //    }

    //}

    #endregion
}
