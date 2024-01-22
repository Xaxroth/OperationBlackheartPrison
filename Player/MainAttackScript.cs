using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainAttackScript : MonoBehaviour
{
    private PlayerControllerScript PlayerInstance;
    public List<GameObject> projectilePool { get; private set; }
    private List<GameObject> Projectiles = new List<GameObject>();
    [SerializeField] private int ProjectilePoolAmount = 30;

    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioClip Reload;
    [SerializeField] private GameObject ProjectilePrefab;
    [SerializeField] private GameObject ShootEffect;
    [SerializeField] private Transform ShootPosition;
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
        projectilePool = new List<GameObject>();

        for (int i = 0; i < numberOfBulletsPerShot * 4; i++)
        {
            GameObject projectile = Instantiate(ProjectilePrefab, gameObject.transform.position, gameObject.transform.rotation);
            DontDestroyOnLoad(projectile);
            projectile.SetActive(false);
            projectilePool.Add(projectile);
        }
    }

    private void PrimaryFire()
    {
        if (Input.GetMouseButtonDown(0) && canFire )
        {
            PlayerControllerScript.Instance.casting = true;
            StartCoroutine(FireShotgun());
        }
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
                PlayerControllerScript.Instance.casting = false;
            }

            yield return new WaitForSeconds(rateOfFire);
            canFire = true;
        }
    }

    private IEnumerator ShotgunVFX()
    {
        ShootEffect.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        ShootEffect.SetActive(false);
    }

    private GameObject GetPooledProjectile()
    {
        // WHY IS THIS CAUSING PROBLEMS

        //for (int i = 0; i < projectilePool.Count; i++)
        //{
        //    if (!projectilePool[i].activeInHierarchy)
        //    {
        //        return projectilePool[i];
        //    }
        //}

        GameObject newProjectile = Instantiate(ProjectilePrefab);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);

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
