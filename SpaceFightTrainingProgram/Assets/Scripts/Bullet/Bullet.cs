using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public abstract class Bullet : MonoBehaviour
{
    [SerializeField]
    GameObject _spawnObjOnHit;
    [SerializeField]
    Transform _detachOnDestory;

    public Rigidbody Rigidbody { get; private set; }
    public Vector3 SpawnPosition { get; private set; }
    public float SpawnTime { get; private set; }
    public Unit LaunchUnit => LaunchWeaon?.Unit;
    public Weapon LaunchWeaon { get; private set; }
    [HideInInspector]
    public Damage expectedDamage;
    [HideInInspector]
    public float lifetime;
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }
    public virtual void Init(Weapon weapon)
    {
        LaunchWeaon = weapon;
        SpawnPosition = transform.position;
        SpawnTime = Time.timeSinceLevelLoad;
    }
    public virtual bool HitCheck(GameObject target)
    {
        if (target.TryGetComponent(out UnitDamageCollider parts))
        {
            var feedback = parts.BulletHit(this, expectedDamage);
            LaunchWeaon.OnHit.Invoke(feedback);
            return feedback.isHit;
        }
        return false;
    }
    public virtual void OnHit(Vector3 point)
    {
        if (_spawnObjOnHit != null)
            Instantiate(_spawnObjOnHit).transform.SetPositionAndRotation(point, transform.rotation);
        Destroy();
    }
    protected virtual void Update()
    {
        if (Time.timeSinceLevelLoad - SpawnTime > lifetime)
        {
            Destroy();
        }
        foreach (var hitInfo in Physics.RaycastAll(transform.position, transform.forward, Rigidbody.velocity.magnitude * Time.fixedDeltaTime))
        {
            if (HitCheck(hitInfo.collider.gameObject))
            {
                OnHit(hitInfo.point);
                break;
            }
        }
    }
    public virtual void Destroy()
    {
        if (_detachOnDestory != null)
        {
            _detachOnDestory.transform.parent = transform.parent;
            foreach (var particle in _detachOnDestory.GetComponentsInChildren<ParticleSystem>())
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }
        Destroy(gameObject);
    }
}
