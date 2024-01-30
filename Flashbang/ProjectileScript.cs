using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProjectileScript : MonoBehaviour
{
    [SerializeField] public Vector3 direction;
    [SerializeField] public Rigidbody flashbangRigidbody;
    [SerializeField] public float force;
    [SerializeField] public float baseSpeed = 15f;
    [SerializeField] private float fuseTimer = 1.5f;
    [SerializeField] public GameObject Explosion;
    [SerializeField] private GameObject GrenadeObject;
    [SerializeField] public Light FlashbangLight;
    [SerializeField] private AudioClip FlashbangCollision;
    [SerializeField] private AudioClip EarsRinging;
    [SerializeField] private AudioSource FlashbangAudioSource;
    [SerializeField] private Image WhiteScreen;
    public bool Detonation = false;
    private bool hasCollided = false;
    void Start()
    {
        flashbangRigidbody = GetComponent<Rigidbody>();
        flashbangRigidbody.AddForce(PlayerControllerScript.Instance.Orientation.transform.forward * force, ForceMode.Impulse);

        gameObject.layer = LayerMask.NameToLayer("FriendlyProjectiles");

        int playerLayer = LayerMask.NameToLayer("Player");
        int flashbangLayer = LayerMask.NameToLayer("FriendlyProjectiles");

        WhiteScreen = GameObject.FindGameObjectWithTag("FlashScreen").GetComponent<Image>();

        Physics.IgnoreLayerCollision(playerLayer, flashbangLayer);
        SetRayCast();
        StartCoroutine(Fuse());
        StartCoroutine(DetonateTriggerEvent());
    }

    public void Update()
    {
        flashbangRigidbody.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);

        float rotationSpeed = 480.0f;

        if (!hasCollided)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (FlashbangAudioSource != null)
        {
            hasCollided = true;
            FlashbangAudioSource.PlayOneShot(FlashbangCollision, 0.5f);
        }
    }

    public IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseTimer);
        Detonation = true;

        if (Natia.Instance != null)
        {
            float DistanceToNatia = Vector3.Distance(gameObject.transform.position, Natia.Instance.transform.position);

            if (DistanceToNatia < 25 && !DialogueManagerScript.Instance.InProgress)
            {
                DialogueManagerScript.Instance.NatiaFriendlyFireEvent();
            }
        }

        GameObject FlashEffect = Instantiate(Explosion, transform.position, transform.rotation);

        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(PlayerControllerScript.Instance.CinemachineCamera);

        if (GeometryUtility.TestPlanesAABB(cameraPlanes, FlashEffect.GetComponent<Renderer>().bounds))
        {
            Vector3 playerPosition = PlayerControllerScript.Instance.transform.position;
            Vector3 explosionPosition = FlashEffect.transform.position;

            float distanceToExplosion = Vector3.Distance(playerPosition, explosionPosition);
            if (distanceToExplosion < 100.0f)
            {
                Vector3 directionToExplosion = explosionPosition - playerPosition;

                RaycastHit hitInfo;

                if (Physics.Raycast(playerPosition, directionToExplosion.normalized, out hitInfo, distanceToExplosion))
                {

                    if (hitInfo.collider.gameObject != FlashEffect && hitInfo.collider.gameObject != gameObject)
                    {
                        Debug.Log("Hit: " + hitInfo.collider.gameObject.name);
                    }
                    else
                    {
                        float fadeFactor = 1.0f - (distanceToExplosion / 100.0f);

                        fadeFactor *= 2;

                        StartCoroutine(FadeIn(fadeFactor));
                    }
                }
            }
            else
            {
                Debug.Log("Player is too far from the explosion.");
            }
        }

        Destroy(FlashEffect, 2.0f);

        flashbangRigidbody.isKinematic = true;
        GrenadeObject.SetActive(false);
        gameObject.GetComponent<SphereCollider>().enabled = false;

        Destroy(gameObject, 6.0f);
    }

    IEnumerator FadeIn(float timeBlinded)
    {
        AudioManager.GradualMuteAllSounds();
        AudioManager.Instance.PlayFlashbangEffect();
        Color targetColor = WhiteScreen.color;
        targetColor.a = 1f;
        float fadeDuration = 0.4f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            WhiteScreen.color = Color.Lerp(WhiteScreen.color, targetColor, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        WhiteScreen.color = targetColor;
        yield return new WaitForSeconds(timeBlinded);
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        AudioManager.GradualUnmuteAllSounds();

        Color targetColor = WhiteScreen.color;
        targetColor.a = 0f;
        float fadeDuration = 4.0f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            WhiteScreen.color = Color.Lerp(WhiteScreen.color, targetColor, elapsedTime / fadeDuration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        WhiteScreen.color = targetColor;
    }

    IEnumerator DetonateTriggerEvent()
    {
        yield return new WaitForSeconds(fuseTimer - 0.1f);
        Detonation = true;
    }

    public void SetRayCast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
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

        flashbangRigidbody.AddForce(direction * baseSpeed * force, ForceMode.Impulse);
    }
}
