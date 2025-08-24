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

    // === 추가: 센터존이 활성화되어 있는 동안 계속 보일 이펙트 ===
    [Header("Zone Visual (Always while active)")]
    [Tooltip("CenterZone이 Active인 동안 계속 재생될 루프 VFX 프리팹")]
    public GameObject zoneLoopEffect;
    [Tooltip("콜라이더 크기에 맞춰 자동 스케일")]
    public bool autoScaleToCollider = true;
    [Tooltip("자동 스케일 보정(1=정확히, 1.1=조금 크게)")]
    public float radiusScale = 1f;
    [Tooltip("지면 z-fighting 방지용 Y 오프셋")]
    public float effectYOffset = 0.01f;

    private readonly Dictionary<GameObject, float> trapReadyAt = new();

    private GamePlayController gpc;
    private Collider zoneCol;
    private Rigidbody rb;

    // 내부 이펙트 인스턴스
    private GameObject zoneLoopInstance;

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

    void OnEnable()
    {
        SpawnZoneVFX();
        FitVFXToCollider();
    }

    void OnDisable()
    {
        trapReadyAt.Clear();
        if (zoneLoopInstance) Destroy(zoneLoopInstance);
        zoneLoopInstance = null;
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

    // ====== 아래는 추가된 이펙트 관리 유틸 ======
    void SpawnZoneVFX()
    {
        if (!zoneLoopEffect || zoneLoopInstance) return;
        zoneLoopInstance = Instantiate(zoneLoopEffect, transform);
        zoneLoopInstance.transform.localPosition = new Vector3(0f, effectYOffset, 0f);
        zoneLoopInstance.transform.localRotation = Quaternion.identity;
        zoneLoopInstance.transform.localScale    = Vector3.one;
    }

    void FitVFXToCollider()
    {
        if (!autoScaleToCollider || !zoneLoopInstance || !zoneCol) return;

        float worldRadius = 1f;
        var ls = transform.lossyScale;
        float xzScale = Mathf.Max(ls.x, ls.z);

        if (zoneCol is SphereCollider s)
            worldRadius = s.radius * xzScale;
        else if (zoneCol is CapsuleCollider c)
            worldRadius = c.radius * xzScale;
        else if (zoneCol is BoxCollider b)
            worldRadius = Mathf.Min(b.size.x * ls.x, b.size.z * ls.z) * 0.5f;

        float sUniform = Mathf.Max(0.0001f, worldRadius * 2f * radiusScale);
        zoneLoopInstance.transform.localScale = new Vector3(sUniform, sUniform, sUniform);

        var p = zoneLoopInstance.transform.localPosition;
        zoneLoopInstance.transform.localPosition = new Vector3(p.x, effectYOffset, p.z);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying)
            zoneCol = GetComponent<Collider>();
        FitVFXToCollider();
    }
#endif
}
