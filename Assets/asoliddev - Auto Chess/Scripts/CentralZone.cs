using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralZone : MonoBehaviour
{
    public enum Mode { Fast, Slow, IceTrap }
    public Mode mode = Mode.Fast;

    [Header("Speed Zone")]
    public float speedMultiplier = 1.2f;    // Haste=1.2, Slow=0.7
    public GameObject enterEffect;          // 가속/감속 진입 VFX (선택)
    public float enterEffectDuration = 1.5f;

    [Header("Ice Trap")]
    public float rootDuration = 1.0f;       // 얼음 함정 멈춤 시간
    public float trapCooldown = 4.0f;       // 유닛별 재적용 쿨다운
    public GameObject iceEffect;            // 빙결 VFX (선택)

    // 유닛별 트랩 재적용 가능 시각
    private readonly Dictionary<GameObject, float> trapReadyAt = new();
    private GamePlayController gpc;

    void Awake()
    {
        // 전투 단계에서만 동작시키기 위해 참조
        var scripts = GameObject.Find("Scripts");
        if (scripts) gpc = scripts.GetComponent<GamePlayController>();

        // 트리거 충돌을 확실히 받으려면(상대에 Rigidbody가 없어도)
        // 이 오브젝트에 Kinematic Rigidbody가 있는 게 안전함
        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // 이 오브젝트에는 Sphere/Box Collider (IsTrigger=On) 필요
    }

    bool CombatNow()
    {
        return gpc == null || gpc.currentGameStage == GameStage.Combat;
    }

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
                champ.OnGotStun(rootDuration); // 1초 멈춤
                if (iceEffect) champ.AddEffect(iceEffect, rootDuration);
            }
            return;
        }

        // Haste/Slow 공통: 속도 배수 적용 + 효과
        var store = champ.GetComponent<SpeedController>();
        if (!store) store = champ.gameObject.AddComponent<SpeedController>();
        store.SetMultiplier(speedMultiplier);

        if (enterEffect) champ.AddEffect(enterEffect, enterEffectDuration);
    }

    void OnTriggerExit(Collider other)
    {
        if (mode == Mode.IceTrap) return;

        var champ = other.GetComponentInParent<ChampionController>();
        if (!champ) return;

        var store = champ.GetComponent<SpeedController>();
        if (store) store.Restore();
    }
}
