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
    private bool _reelingIn;

    public static int amountOfChains = 0;
    public static int maxAmountOfChains = 3;

    public Transform caster;
    public Transform castPosition;
    public Transform target;

    [SerializeField] private LineRenderer _hookLine;

    [SerializeField] private Vector3 _targetPosition;

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

        if (_reelingIn)
        {
            //PlayerCon.playerSpeed = 6f;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (_canHook && other.CompareTag("Enemy"))
        {
            StartCoroutine(HookCoroutine());
            Collision(other.transform);
        }
        else if (_canHook && other.CompareTag("Environment"))
        {
            GameObject hitExplosion = Instantiate(_impactEffect, transform.position, transform.rotation);
            stopRange = 2;
            returnspeed = 60;
            Destroy(hitExplosion, 2f);
            _canHook = false;
            _hookHit = true;
        }
    }

    private void Collision(Transform hitTarget)
    {
        speed = returnspeed;
        _hookHit = true;

        if (hitTarget)
        {
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
            if (Input.GetMouseButtonDown(1))
            {
                DestroyHook();
            }
        }

        if (caster)
        {
            _hookLine.SetPosition(0, ThrowHook.Instance.HookTransform.position);
            _hookLine.SetPosition(1, transform.position);

            if (_hookHit)
            {
                transform.LookAt(ThrowHook.Instance.HookTransform.position);

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
                    _canHook = false;
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

        yield return new WaitForSeconds(1);

        speed = 80;
        returnspeed = 160;

        yield return new WaitForSeconds(1);

        DestroyHook();
    }

    private void DestroyHook()
    {
        _reelingIn = false;
        //_player.playerSpeed = _player.playerNormalSpeed;
        amountOfChains--;
        ThrowHook.Instance.HookWeapon.SetActive(true);
        Destroy(gameObject);
    }

}
