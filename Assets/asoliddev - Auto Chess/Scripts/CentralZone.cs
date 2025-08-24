using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CentralZone : MonoBehaviour
{
    public enum Mode { Fast, Slow, IceTrap }
    public Mode mode = Mode.Fast;

    [Header("Speed Zone")]
    [Tooltip("원래 유닛 속도에 곱할 배수 (예: 2.0=두배, 0.5=절반)")]
    public float speedMultiplier = 1.2f;

    [Header("Ice Trap")]
    public float rootDuration = 1.0f;
    public float trapCooldown = 4.0f;

    private readonly Dictionary<GameObject, float> trapReadyAt = new();

    private GamePlayController gpc;
    private Collider zoneCol;
    private Rigidbody rb;

    void Awake()
    {
        gpc = GameObject.Find("Scripts")?.GetComponent<GamePlayController>()
             ?? Object.FindFirstObjectByType<GamePlayController>();

        // 트리거 조건 보장
        zoneCol = GetComponent<Collider>();
        if (zoneCol) zoneCol.isTrigger = true;

        rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity  = false;
    }

    void OnDisable()
    {
        trapReadyAt.Clear();
    }

    private bool CombatNow() => gpc == null || gpc.currentGameStage == GameStage.Combat;

    void OnTriggerEnter(Collider other)
    {
        if (!CombatNow()) return;

        var champ = other.GetComponentInParent<ChampionController>();
        if (!champ || champ.isDead) return;

        if (mode == Mode.IceTrap)
        {
            if (!trapReadyAt.TryGetValue(champ.gameObject, out var t) || Time.time >= t)
            {
                trapReadyAt[champ.gameObject] = Time.time + trapCooldown;
                champ.OnGotStun(rootDuration);
            }
            return;
        }

        var sc = champ.GetComponent<SpeedController>() ?? champ.gameObject.AddComponent<SpeedController>();
        sc.SetMultiplier(speedMultiplier);
    }

    void OnTriggerStay(Collider other)
    {
        if (!CombatNow()) return;

        var champ = other.GetComponentInParent<ChampionController>();
        if (!champ || champ.isDead) return;

        if (mode == Mode.IceTrap)
        {
            if (!trapReadyAt.TryGetValue(champ.gameObject, out var t) || Time.time >= t)
            {
                trapReadyAt[champ.gameObject] = Time.time + trapCooldown;
                champ.OnGotStun(rootDuration);
            }
            return;
        }

        // 존 안에 있는 동안 배수 유지
        var sc = champ.GetComponent<SpeedController>() ?? champ.gameObject.AddComponent<SpeedController>();
        sc.SetMultiplier(speedMultiplier);
    }

    void OnTriggerExit(Collider other)
    {
        var champ = other.GetComponentInParent<ChampionController>();
        if (!champ) return;

        if (mode != Mode.IceTrap)
        {
            var sc = champ.GetComponent<SpeedController>();
            if (sc) sc.Restore();
        }

        trapReadyAt.Remove(champ.gameObject);
    }
}
