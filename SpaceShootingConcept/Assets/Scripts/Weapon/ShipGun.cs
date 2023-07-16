using IzumiTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShipGun : Weapon
{
    [SerializeField]
    Damage expectedDamage;
    [SerializeField]
    bool _coaxial;
    [SerializeField]
    Transform _xRotateJoint;
    [SerializeField]
    Transform _yRotateJoint;
    [SerializeField]
    float _rotateRequiredTime;
    [SerializeField]
    TestGunCrosshair _crosshairPrefab;
    [SerializeField]
    TestBullet _bulletPrefab;
    [SerializeField]
    Transform _aimOrigin;
    [SerializeField]
    Transform _launchAnchor;
    [SerializeField]
    Vector2 _xRotateRange = new Vector2(-90, 90);
    [SerializeField]
    Vector2 _yRotateRange = new Vector2(-180, 180);
    [SerializeField]
    float _launchVelocity;
    [SerializeField]
    float _range;
    [SerializeField]
    Cooldown _fireCD;
    [SerializeField]
    Cooldown _reloadCD;
    [SerializeField]
    CappedValue _magazineSize;

    public override bool Coaxial => _coaxial;
    public Cooldown FireCD => _fireCD;
    public Cooldown ReloadCD => _reloadCD;
    public CappedValue Magazine => _magazineSize;
    public override Transform LaunchAnchor => _launchAnchor;
    public override float ProjectileAvgVelocity => _launchVelocity;
    public Vector3 TargetAimPosition { get; private set; }
    public bool HasTargetAimPosition { get; private set; }

    public override Damage ExpectedDamage => expectedDamage;
    public TestGunCrosshair GeneratedCrosshair { get; private set; }

    Tweener _xRotateTween, _yRotateTween;
    public Quaternion _targetAimRotation;
    private void Awake()
    {
        Init();
        _xRotateTween = _xRotateJoint.DOLocalRotate(Vector3.zero, _rotateRequiredTime).SetEase(Ease.OutExpo).SetAutoKill(false).SetLink(gameObject);
        _yRotateTween = _yRotateJoint.DOLocalRotate(Vector3.zero, _rotateRequiredTime).SetEase(Ease.OutExpo).SetAutoKill(false).SetLink(gameObject);
    }
    private void LateUpdate()
    {
        FireCD.AddDeltaTime();
        //magazine
        if (!Magazine.IsFull && ReloadCD.AddDeltaTimeAndEat())
        {
            ++Magazine.Value;
        }
        //aim
        if(HasTargetAimPosition)
        {
            Vector3 localTargetAimPos = _aimOrigin.InverseTransformPoint(TargetAimPosition);
            float xzDistance = Mathf.Sqrt(localTargetAimPos.x * localTargetAimPos.x + localTargetAimPos.z * localTargetAimPos.z);
            float clampOverflowX, clampOverflowY;
            _xRotateTween.ChangeEndValue(Vector3.right * ExtendedMath.ClampAndGetOverflow(Mathf.Rad2Deg * Mathf.Atan2(-localTargetAimPos.y, xzDistance), -_xRotateRange.y, -_xRotateRange.x, out clampOverflowX), true).Restart();
            _yRotateTween.ChangeEndValue(Vector3.up * ExtendedMath.ClampAndGetOverflow(Mathf.Rad2Deg * Mathf.Atan2(localTargetAimPos.x, localTargetAimPos.z), _yRotateRange.x, _yRotateRange.y, out clampOverflowY), true).Restart();
            // don't know why this doesn't work
            //targetRotation = Quaternion.LookRotation(TargetAimPosition - _aimOrigin.position, transform.up);
            if (GeneratedCrosshair != null)
            {
                if (!Coaxial && (clampOverflowX != 0 || clampOverflowY != 0))
                {
                    //unstable aim (out of rotate range)
                    GeneratedCrosshair.SetInAimRange(false);
                }
                else
                {
                    GeneratedCrosshair.SetInAimRange(true);
                }
            }
        }
        else
        {
            _xRotateTween.ChangeEndValue(Vector3.zero, true).Restart();
            _yRotateTween.ChangeEndValue(Vector3.zero, true).Restart();
        }
    }
    public override void Init()
    {
        FireCD.Reset();
        ReloadCD.Reset();
        Magazine.Value = 0;
    }
    public override void AimPosition(Vector3 position)
    {
        TargetAimPosition = position;
        HasTargetAimPosition = true;
    }
    public override void Trigger()
    {
        if(!Magazine.IsEmpty && FireCD.Eat())
        {
            --Magazine.Value;
            Fire();
        }
    }
    public override void Fire()
    {
        TestBullet bullet = Instantiate(_bulletPrefab);
        bullet.transform.SetPositionAndRotation(_launchAnchor.position, _launchAnchor.rotation);
        bullet.Rigidbody.position = bullet.transform.position;
        bullet.Rigidbody.velocity = _launchAnchor.forward * _launchVelocity + Unit.Rigidbody.velocity;
        bullet.expectedDamage = expectedDamage;
        bullet.lifetime = _range / _launchVelocity;
        bullet.Init(this);
        //bullet.SetDebugLine(2500);
    }
    public override GameObject GenerateCrosshair()
    {
        TestGunCrosshair crosshair = Instantiate(_crosshairPrefab);
        crosshair.Init(this);
        GeneratedCrosshair = crosshair;
        return crosshair.gameObject;
    }
    public override GameObject GetGeneratedCrossHair()
    {
        return GeneratedCrosshair?.gameObject;
    }
}
