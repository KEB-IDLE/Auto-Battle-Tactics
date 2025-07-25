using UnityEngine;
using System.Linq;
using UnityEngine.AI;
using System;

public class MoveComponent : MonoBehaviour, IMoveNotifier, IOrientable
{
    public event Action OnMove;

    Transform coreTransform;
    NavMeshAgent _agent;
    IAttackable _attacker;
    IDamageable _health;
    ITeamProvider _teamProvider;
    private float moveSpeed;
    bool _isMoving;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _attacker = GetComponent<IAttackable>();
        _health = GetComponent<IDamageable>();
        _teamProvider = GetComponent<ITeamProvider>();
    }

    private void Start()
    {
        coreTransform = CoreRegistry.Instance.GetEnemyCore(_teamProvider.Team);
    }

    void Update()
    {
        if(_attacker.IsAttacking() || !_health.IsAlive())
        {
            if(_isMoving) 
                _isMoving = false;
            _agent.isStopped = true;
            _agent.ResetPath();
            return;
        }
        if (_agent.isStopped)
            _agent.isStopped = false;

        var target = _attacker.DetectTarget();
        if (target != null && target.IsAlive() && CanSee(target))
        {
            var mb = target as MonoBehaviour;
            _agent.SetDestination(mb.transform.position);
            LookAtTarget(target);
        }
        else
            _agent.SetDestination(coreTransform.position);

        if (!_isMoving)
        {
            _isMoving = true;
            OnMove?.Invoke();
        }
    }

    public void Initialize(EntityData data)
    {
        moveSpeed = data.moveSpeed;
        _agent.speed = moveSpeed;
        _agent.acceleration = moveSpeed;
        _agent.autoBraking = false;
        _isMoving = false;

    }
    public void LookAtTarget(IDamageable target)
    {
        var mb = target as MonoBehaviour;
        if (mb != null)
        {
            Vector3 targetPos = mb.transform.position;
            targetPos.y = transform.position.y; // y 고정(수평 회전)
            transform.LookAt(targetPos);
        }
    }

    private bool CanSee(IDamageable target)
    {
        var attackComponent = _attacker as AttackComponent;
        if (attackComponent == null || target == null)
            return false;

        // 공격 타입별로 가시성 판정 커스텀 가능
        // 예: 마법사는 무조건 true, 원거리/근접은 Raycast
        if (attackComponent.isMagic)
            return true;

        var firePoint = attackComponent.firePoint != null ? attackComponent.firePoint : attackComponent.transform;
        var targetTransform = (target as MonoBehaviour)?.transform;

        if (firePoint == null || targetTransform == null)
            return false;

        return attackComponent.IsTargetVisible(firePoint, targetTransform);
    }

}
