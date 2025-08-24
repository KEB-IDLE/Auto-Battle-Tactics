using UnityEngine;
using UnityEngine.AI;

public class SpeedController : MonoBehaviour
{
    private NavMeshAgent agent;
    [HideInInspector] public float baseSpeed;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null) baseSpeed = agent.speed; // 프리팹 기본 속도 캐싱
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
