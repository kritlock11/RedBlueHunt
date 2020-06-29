using System;
using Enums;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [SerializeField] private Material[] _matArr;
    
    private Material _redMat;
    private Material _blueMat;
    private Material _grayMat;
    
    private Vector3 _curMoveDirection;
    private float _speed;
    private float _radius;
    private UnitType _type;

    private Renderer _renderer;
    private Rigidbody _rb;
    
    public Vector3 CurMoveDirection => _curMoveDirection.normalized;

    public UnitType Type
    {
        get => _type;
        private set
        {
            _type = value;
            SetMaterialByType(_type);
        }
    }

    public float Speed
    {
        get => _speed;
        private set => _speed = value;
    }
    public float Radius
    {
        get => _radius;
        private set => _radius = value;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _renderer = GetComponent<Renderer>();
        
        _blueMat = _matArr[0];
        _redMat = _matArr[1];
        _grayMat = _matArr[2];
    }

    public void Init(float radius, float speed, UnitType type)
    {
        Radius = radius;
        Speed = speed;
        Type = type;
    }

    public void SwitchType()
    {
        switch (Type)
        {
            case UnitType.Red:
                SetMaterialByType(UnitType.Blue);
                break;
            case UnitType.Blue:
                SetMaterialByType(UnitType.Red);
                break;
            case UnitType.None:
                SetMaterialByType(UnitType.None);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetMaterialByType(UnitType type)
    {
        _type = type;
        switch (Type)
        {
            case UnitType.Red:
                SetMaterial(_redMat);
                break;
            case UnitType.Blue:
                SetMaterial(_blueMat);
                break;
            case UnitType.None:
                SetMaterial(_grayMat);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetMaterial(Material material) => _renderer.material = material;
    public void SetDir(Vector3 v3) => _curMoveDirection = v3;
    public void SetVel(Vector3 v3) => _rb.velocity = v3;
}