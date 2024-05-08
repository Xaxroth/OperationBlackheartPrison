using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HookScript : MonoBehaviour
{
    [SerializeField] private string[] hookableObjects;
    [SerializeField] private float speed, returnspeed, range, stopRange;

    [SerializeField] private GameObject _impactEffect;

    private bool _hookHit;
    private bool _canHook;
    private bool _draggingSpear;
    private bool _reelingIn;

    public static int amountOfChains = 0;
    public static int maxAmountOfChains = 3;
    public int HookDamage = 30;

    public Transform caster;
    public Transform castPosition;
    public Transform target;

    [SerializeField] private LineRenderer _hookLine;

    [SerializeField] private Vector3 _targetPosition;

    public GameObject SpearObject;

    //[SerializeField] private CharacterControllerScript _player;
    //[SerializeField] private CinemachineShakeExtension _cinemachineShake;

    void Start()
    {
        _hookLine = gameObject.GetComponentInChildren<LineRenderer>();
        amountOfChains++;
        returnspeed = 80;
        ThrowHook.Instance.HookWeapon.SetActive(false);
    }

    void Update()
    {
        _targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + stopRange);

        Physics.IgnoreLayerCollision(7, 8);

        StartThrowHook();

        if (_hookHit && Input.GetMouseButtonDown(1))
        {
            speed = 80;
            returnspeed = 160;
            Debug.Log("Maybe a bullet tomorrow will find me");
            Rigidbody rb = PlayerControllerScript.Instance.GetComponent<Rigidbody>();
            _draggingSpear = true;
            _reelingIn = true;
            Vector3 direction = (SpearObject.transform.position - PlayerControllerScript.Instance.PlayerBody.transform.position).normalized;

            // Apply force in the direction
            rb.AddForce(direction * 25, ForceMode.Impulse);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_canHook)
        {
            if (other.CompareTag("Enemy"))
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(HookDamage, false);
                StartCoroutine(HookCoroutine());
                Collision(other.transform);
            }
            else if (other.CompareTag("Environment"))
            {
                GameObject hitExplosion = Instantiate(_impactEffect, transform.position, transform.rotation);
                stopRange = 2;
                returnspeed = 0;
                speed = 0;
                Destroy(hitExplosion, 2f);
                _canHook = false;
                _hookHit = true;
            }
        }
    }

    private void Collision(Transform hitTarget)
    {
        speed = returnspeed;
        _hookHit = true;

        if (hitTarget)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.position = hitTarget.position + new Vector3(0, 3, 0);
            target = hitTarget;
            GameObject hitExplosion = Instantiate(_impactEffect, target.position + new Vector3(0, 3, 0), target.rotation);
            Destroy(hitExplosion, 2f);
        }
    }

    private void StartThrowHook()
    {
        if (amountOfChains >= maxAmountOfChains)
        {
            amountOfChains = maxAmountOfChains;
        }

        if (_reelingIn)
        {
            //if (Input.GetMouseButtonDown(1))
            //{
            //    DestroyHook();
            //}
        }

        if (caster)
        {
            _hookLine.SetPosition(0, ThrowHook.Instance.HookTransform.position);
            _hookLine.SetPosition(1, transform.position);

            if (_hookHit)
            {
                if (_reelingIn)
                {
                    transform.LookAt(ThrowHook.Instance.HookTransform.position);
                }

                var dist = Vector3.Distance(transform.position, ThrowHook.Instance.HookTransform.position);

                if (dist < stopRange)
                {
                    DestroyHook();
                }
            }
            else
            {
                var dist = Vector3.Distance(transform.position, ThrowHook.Instance.HookTransform.position);

                if (dist > range)
                {
                    transform.LookAt(ThrowHook.Instance.HookTransform.position);
                    _canHook = false;
                    speed = 80;
                    returnspeed = 160;
                    Collision(null);
                }
                else
                {
                    _canHook = true;
                }
            }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            if (target)
            {
                target.transform.position = transform.position - new Vector3(0, 3, 0);
            }
        }
        else
        {
            DestroyHook();
        }
    }

    private IEnumerator HookCoroutine()
    {
        //CinemachineShakeExtension.Instance.ShakeCamera(1f, 0.3f, 4f);

        _reelingIn = true;
        _canHook = false;

        stopRange = 7;
        returnspeed = 0;
        speed = 0;
        //_player.playerSpeed = 6f;

        yield return new WaitForSeconds(5);

        DestroyHook();
    }

    private void DestroyHook()
    {
        _reelingIn = false;
        amountOfChains--;
        ThrowHook.Instance.HookWeapon.SetActive(true);
        ThrowHook.Instance.canThrow = true;
        Destroy(gameObject);
    }

}
