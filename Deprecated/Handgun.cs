using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handgun : MonoBehaviour
{
    public static Handgun Instance;

    [Header("Audio")]
    [SerializeField] private AudioSource HandgunAudioSource;
    [SerializeField] private AudioClip ReloadSound;
    [SerializeField] private AudioClip ShootSound;

    [Header("Cosmetics")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private GameObject MuzzleFlash;
    [SerializeField] private ParticleSystem MuzzleFlashParticles;
    [SerializeField] private GameObject ImpactHole;

    [Header("Transform Properties")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] private Transform PlayerRotation;
    [SerializeField] private Transform Orientation;

    [Header("Recoil")]
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;

    [SerializeField] private float snappiness;
    [SerializeField] private float returnSpeed;

    private float timeSinceLastShot;

    private IEnumerator shootCoroutine;

    private bool reloading = false;
    private bool CanFire() => !reloading && timeSinceLastShot > 1f / (rateOfFire / 60f) && !_playerController.sprinting;
    private bool shooting = false;

    [SerializeField] private float rateOfFire = 0.1f;
    [SerializeField] private int currentAmmo = 15;
    [SerializeField] private int clipSize = 15;
    [SerializeField] private float reloadSpeed = 2f;
    float maxDistance = 100f;

    [SerializeField] public KeyCode AttackButton;
    [SerializeField] public KeyCode AimButton;

    public Animator GunAnimator;

    private Vector2 resetPosition;

    [SerializeField] private PlayerControllerScript _playerController;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        _playerController = PlayerControllerScript.Instance;
        resetPosition = new Vector2(CameraControllerScript.Instance.xRotation, CameraControllerScript.Instance.yRotation);
    }

    void Update()
    {
        if (_playerController.paralyzed) return;

        if (!shooting)
        {
            CalculateRecoil();
        }

        if (timeSinceLastShot < 1f)
        {
            shooting = true;
        }
        else
        {
            shooting = false;
        }

        timeSinceLastShot += Time.deltaTime;
    }

    private void LateUpdate()
    {
        if (shooting)
        {
            CalculateRecoil();
        }
    }

    public void CalculateRecoil()
    {
        if (Input.GetKeyDown(AttackButton) && !_playerController.paralyzed)
        {
            Shoot();
        }

        Debug.DrawRay(ShootPosition.position, ShootPosition.forward);
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);

        cameraTransform.localRotation = Quaternion.Euler(currentRotation);
        cameraHolder.localRotation = Quaternion.Euler(currentRotation);

        returnSpeed = 4f;
        recoilX = 8;
        recoilY = 8;
        recoilZ = 12;
    }

    public void Shoot()
    {
        if (currentAmmo > 0)
        {
            if (CanFire())
            {
                Recoil();
            }
        }
        else
        {

        }
    }

    public void Recoil()
    {
        targetRotation -= new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }

    private IEnumerator MuzzleFlashEffect()
    {
        MuzzleFlash.SetActive(true);
        GunAnimator.SetBool("Shoot", true);
        yield return new WaitForSeconds(0.075f);
        MuzzleFlash.SetActive(false);
        GunAnimator.SetBool("Shoot", false);
    }
}
