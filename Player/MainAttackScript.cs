using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAttackScript : MonoBehaviour
{
    private PlayerControllerScript PlayerInstance;
    public List<GameObject> projectilePool = new List<GameObject>();
    public List<GameObject> explosivePool = new List<GameObject>();
    private List<GameObject> Projectiles = new List<GameObject>();
    [SerializeField] private int ProjectilePoolAmount = 30;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip Reload;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private GameObject ExplosionPrefab;
    [SerializeField] private GameObject ShootEffect;
    [SerializeField] private Transform ShootPosition;
    [SerializeField] public Light ProjectilePointLight;
    public ParticleSystem MuzzleFlash;
    [SerializeField] private Text ammoDisplay;
    [SerializeField] private Transform Orientation;

    private bool reloading = false;
    private bool canFire = true;

    [SerializeField] private bool firing = false;
    [SerializeField] private int amountOfProjectilesPerShot = 10;
    [SerializeField] private int numberOfBulletsPerShot = 1;
    [SerializeField] private float rateOfFire = 0.6f;
    [SerializeField] private int playerAmmo = 12;
    [SerializeField] private float reloadSpeed = 2f;
    [SerializeField] private int playerMaxAmmo = 12;
    [SerializeField] private int projectileForce = 25;

    void Start()
    {
        InitializeProjectilePool();
        ammoDisplay.text = playerAmmo.ToString();
        AudioSource = gameObject.GetComponent<AudioSource>();
        PlayerInstance = PlayerControllerScript.Instance;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            firing = true;
        }
        else
        {
            firing = false;
        }

        ShootPosition.transform.rotation = Orientation.transform.rotation;

        PrimaryFire();

        CheckReload();
    }

    private void InitializeProjectilePool()
    {
        for (int i = 0; i < numberOfBulletsPerShot * 30; i++)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }

        for (int i = 0; i < numberOfBulletsPerShot * 30; i++)
        {
            GameObject projectile = Instantiate(ExplosionPrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            explosivePool.Add(projectile);
        }
    }

    private void PrimaryFire()
    {
        if (Input.GetMouseButtonDown(0) && canFire )
        {
            StartCoroutine(FireShotgun());
        }
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        float startIntensity = 35;

        while (elapsedTime < 0.35f)
        {
            float currentIntensity = Mathf.Lerp(startIntensity, 0f, elapsedTime / 0.35f);

            ProjectilePointLight.intensity = currentIntensity;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        ProjectilePointLight.intensity = 0f;
    }

    private IEnumerator FireShotgun()
    {
        while (firing && playerAmmo > 0)
        {
            canFire = false;
            StartCoroutine(ShotgunVFX());
            AudioManager.Instance.PlaySound(AudioManager.Instance.ReleaseEnergy, 1.0f);

            playerAmmo--;

            for (int i = 0; i < amountOfProjectilesPerShot; i++)
            {
                GameObject currentProjectile = GetPooledProjectile();

                currentProjectile.GetComponent<MainAttackProjectile>().Activate();
                currentProjectile.transform.position = ShootPosition.transform.position + new Vector3(0, 3f, 0);
                currentProjectile.transform.rotation = ShootPosition.transform.rotation;

                currentProjectile.GetComponent<MainAttackProjectile>().power = projectileForce;
                currentProjectile.GetComponent<MainAttackProjectile>().SetRayCast();
                Physics.IgnoreCollision(currentProjectile.GetComponent<Collider>(), GetComponent<Collider>());
            }

            yield return new WaitForSeconds(rateOfFire);
            canFire = true;
        }
    }

    private IEnumerator ShotgunVFX()
    {
        ShootEffect.SetActive(true);
        MuzzleFlash.Play();
        StartCoroutine(FadeOut());
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", true);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", true);
        yield return new WaitForSeconds(0.2f);
        MuzzleFlash.Stop();
        PlayerControllerScript.Instance.playerAnimator.SetBool("Fire", false);
        PlayerControllerScript.Instance.CameraAnimator.SetBool("ShotgunRecoil", false);
        ShootEffect.SetActive(false);
    }

    private GameObject GetPooledProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }

        GameObject newProjectile = Instantiate(ProjectilePrefab);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);

        return newProjectile;
    }

    public GameObject GetPooledExplosion()
    {
        for (int i = 0; i < explosivePool.Count; i++)
        {
            if (!explosivePool[i].activeInHierarchy)
            {
                return explosivePool[i];
            }
        }

        GameObject newProjectile = Instantiate(ProjectilePrefab);
        explosivePool.Add(newProjectile);

        return newProjectile;
    }

    private void CheckReload()
    {
        if (playerAmmo <= 0 && reloading == false || Input.GetKeyDown(KeyCode.R) && reloading == false && playerAmmo < playerMaxAmmo)
        {
            StartCoroutine(ReloadWeapon());
            return;
        }
    }

    private IEnumerator ReloadWeapon()
    {
        AudioSource.PlayOneShot(Reload, 0.6f);
        canFire = false;
        reloading = true;
        firing = false;
        yield return new WaitForSeconds(reloadSpeed);
        playerAmmo = playerMaxAmmo;
        ammoDisplay.text = playerAmmo.ToString();
        canFire = true;
        reloading = false;
    }
}
