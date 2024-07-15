using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static Block Instance;

    [Header("Logistics")]
    [SerializeField] public KeyCode AttackButton;
    [SerializeField] private PlayerControllerScript _playerController;
    [SerializeField] private PlayerControllerScript player;
    [SerializeField] private AudioSource playerAudioSource;
    [SerializeField] private AudioClip blockedProjectile;
    [SerializeField] private GameObject LightObject;
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private ParticleSystem cosmeticEffect;
    [SerializeField] private Light LightSource;
    private float TargetLightIntensity;

    [SerializeField] private Transform BlockTransform;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float coneRadius = 4;

    Vector3 OffSet = new Vector3(0, 0, 5);
    [Header("Range")]
    [Range(0, 360)]
    public float viewAngle = 180;

    public bool blocking;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        TargetLightIntensity = LightSource.intensity;
        player = gameObject.GetComponent<PlayerControllerScript>();
        playerAudioSource = gameObject.GetComponent<AudioSource>();
        playerRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = LightSource.intensity;
        cosmeticEffect.Stop();

        while (elapsedTime < fadeDuration)
        {
            float currentIntensity = Mathf.Lerp(TargetLightIntensity, 0f, elapsedTime / fadeDuration);

            LightSource.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }
        blocking = false;

        LightSource.intensity = 0f;
    }

    IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        float startIntensity = LightSource.intensity;
        cosmeticEffect.Play();

        while (elapsedTime < fadeDuration)
        {
            float currentIntensity = Mathf.Lerp(0f, TargetLightIntensity, elapsedTime / fadeDuration);

            LightSource.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        LightSource.intensity = TargetLightIntensity;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !blocking)
        {
            blocking = true;
            //StartCoroutine(FadeIn());
            playerAudioSource.PlayOneShot(blockedProjectile);
            player.playerStamina++;
            Collider[] cone = Physics.OverlapSphere(BlockTransform.position, coneRadius);
            StartCoroutine(FadeIn());
            //LightObject.SetActive(true);


            if (cone.Length != 0)
            {
                foreach (var hitCollider in cone)
                {
                    Transform targetTransform = hitCollider.GetComponent<Transform>();
                    Vector3 targetDirection = (targetTransform.position - transform.position).normalized;

                    if (Vector3.Angle(transform.forward, targetDirection) < viewAngle)
                    {
                        if (hitCollider.gameObject.CompareTag("Projectile"))
                        {
                            playerAudioSource.PlayOneShot(blockedProjectile, 0.2f);
                            playerRigidbody.AddForce(-1f * player.Orientation.transform.forward, ForceMode.Impulse);
                            Destroy(hitCollider.gameObject);
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F) && blocking)
        {
            StartCoroutine(FadeOut());
            blocking = false;
        }
    }
}
