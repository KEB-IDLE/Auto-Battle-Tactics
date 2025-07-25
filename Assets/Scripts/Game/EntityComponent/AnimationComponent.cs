using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent (typeof(AudioSource))]
public class AnimationComponent : MonoBehaviour
{
    private Animator _animator;
    private AnimatorOverrideController _controller;
    private AudioSource _audioSource;
    private AudioClip summonSound;
    private AudioClip attackSound;
    private AudioClip deathSound;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void Initialize(EntityData data)
    {
        if (_controller == null)
            _controller = new AnimatorOverrideController(data.animatorController);
         
        _animator.runtimeAnimatorController = _controller;

        if (data.attackClip != null)
            _controller["Attack"] = data.attackClip;
        if (data.runClip != null)
            _controller["Run"] = data.runClip;
        if (data.deathClip != null)
            _controller["Die"] = data.deathClip;
        if (data.idleClip != null)
            _controller["Idle"] = data.idleClip;
        if (data.summonSound != null)
            this.summonSound = data.summonSound;
        if (data.attackSound != null)
            this.attackSound = data.attackSound;
        if (data.deathSound != null)
            this.deathSound = data.deathSound;
    }

    public void HandleAttack(bool isAttacking)
    {
        if (isAttacking)
        {
            _animator.SetBool("Attack", true);
            if(this.attackSound != null)
                _audioSource.PlayOneShot(attackSound);
        }
        else
            _animator.SetBool("Attack", false);
    }
            

    public void HandleMove()
    {
        _animator.SetTrigger("Move");
    }

    public void HandleDeath()
    {
        Debug.Log("DeathAnimation is Called");
        _animator.SetTrigger("Death");
        if(this.deathSound != null)
            _audioSource.PlayOneShot(deathSound);
    }
}



// network
//using UnityEngine;

//[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(AudioSource))]
//public class AnimationComponent : MonoBehaviour
//{
//    private Animator _animator;
//    private AnimatorOverrideController _controller;
//    private AudioSource _audioSource;
//    private AudioClip summonSound;
//    private AudioClip attackSound;
//    private AudioClip deathSound;

//    IAttackNotifier _attackSrc;
//    IMoveNotifier _moveSrc;
//    IDeathNotifier _deathSrc;

//    void Awake()
//    {
//        _animator = GetComponent<Animator>();
//        _attackSrc = GetComponent<IAttackNotifier>();
//        _moveSrc = GetComponent<IMoveNotifier>();
//        _deathSrc = GetComponent<IDeathNotifier>();
//        _audioSource = GetComponent<AudioSource>();
//    }

//    public void Initialize(EntityData data)
//    {
//        if (_controller == null)
//            _controller = new AnimatorOverrideController(data.animatorController);
//        _animator.runtimeAnimatorController = _controller;

//        if (data.attackClip != null)
//            _controller["Attack"] = data.attackClip;
//        if (data.runClip != null)
//            _controller["Run"] = data.runClip;
//        if (data.deathClip != null)
//            _controller["Die"] = data.deathClip;
//        if (data.idleClip != null)
//            _controller["Idle"] = data.idleClip;
//        if (data.summonSound != null)
//            this.summonSound = data.summonSound;
//        if (data.attackSound != null)
//            this.attackSound = data.attackSound;
//        if (data.deathSound != null)
//            this.deathSound = data.deathSound;
//    }

//    public void Bind()
//    {
//        _attackSrc.OnAttackStateChanged += HandleAttack;
//        _moveSrc.OnMove += HandleMove;
//        _deathSrc.OnDeath += HandleDeath;
//    }

//    public void Unbind()
//    {
//        _attackSrc.OnAttackStateChanged -= HandleAttack;
//        _moveSrc.OnMove -= HandleMove;
//        _deathSrc.OnDeath -= HandleDeath;
//    }

//    void HandleAttack(bool isAttacking)
//    {
//        if (isAttacking)
//        {
//            Debug.Log("attack Start!");
//            _animator.SetBool("Attack", true);
//            _audioSource.PlayOneShot(attackSound);
//        }
//        else
//        {
//            Debug.Log("attack End!");
//            _animator.SetBool("Attack", false);
//        }
//    }
//    public void PlayAnimation(string animName)
//    {
//        // 예시: 애니메이터를 통해 애니메이션 재생
//        GetComponent<Animator>().Play(animName);
//    }
//    public string GetCurrentAnimation()
//    {
//        if (_animator == null) return null;

//        var clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
//        return clipInfo.Length > 0 ? clipInfo[0].clip.name : null;
//    }
//    void HandleMove()
//    {
//        _animator.SetTrigger("Move");
//    }

//    void HandleDeath()
//    {
//        _animator.SetTrigger("Die");
//        _audioSource.PlayOneShot(deathSound);
//        Unbind();
//    }
//}
