using UnityEngine;
using UnityEngine.AI;

public class SpeedController : MonoBehaviour
{
    private NavMeshAgent agent;
    [HideInInspector] public float baseSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) baseSpeed = agent.speed;
    }

    // 라운드 종료 후 사망 → 다음 라운드에 재활성화될 때 원복
    void OnEnable()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (agent && baseSpeed > 0f) agent.speed = baseSpeed;
    }

    public void SetMultiplier(float mul)
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (!agent) return;
        if (baseSpeed <= 0f) baseSpeed = agent.speed;
        agent.speed = baseSpeed * mul;
    }

    public void Restore()
    {
        if (!agent) agent = GetComponent<NavMeshAgent>();
        if (agent && baseSpeed > 0f) agent.speed = baseSpeed;
    }
}
