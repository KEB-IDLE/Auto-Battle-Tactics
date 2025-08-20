using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls champion animations
/// </summary>
public class ChampionAnimation : MonoBehaviour
{
    private GameObject characterModel;
    [SerializeField] private Animator animator;   // ← 인스펙터로 직접 지정 가능(선택)
    private ChampionController championController;

    private Vector3 lastFramePosition;

    void Start()
    {
        // get character model (기존 구조 우선)
        var character = this.transform.Find("character");
        if (character != null)
        {
            characterModel = character.gameObject;
            if (animator == null) animator = characterModel.GetComponent<Animator>();
        }

        // fallback: 자식/자신 어디에 있든 Animator 자동 탐색
        if (animator == null) animator = GetComponentInChildren<Animator>(true);
        if (animator == null) animator = GetComponent<Animator>();

        // ChampionController (기존과 동일 위치 가정)
        championController = this.transform.GetComponent<ChampionController>();

        if (animator == null)
        {
            Debug.LogError($"{name}: Animator not found for ChampionAnimation.");
            enabled = false; // NRE 방지
            return;
        }

        // 첫 프레임 과속 방지
        lastFramePosition = this.transform.position;
    }

    void Update()
    {
        if (!animator) return;

        // calculate speed (안전 dt)
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float movementSpeed = (this.transform.position - lastFramePosition).magnitude / dt;

        // set movement speed on animator controller
        animator.SetFloat("movementSpeed", movementSpeed);

        // store last frame position
        lastFramePosition = this.transform.position;
    }

    public void DoAttack(bool b)
    {
        if (!animator) return;
        animator.SetBool("isAttacking", b);
    }

    public void OnAttackAnimationFinished()
    {
        if (animator) animator.SetBool("isAttacking", false);
        if (championController) championController.OnAttackAnimationFinished();
    }

    public void IsAnimated(bool b)
    {
        if (animator) animator.enabled = b;
    }
}
