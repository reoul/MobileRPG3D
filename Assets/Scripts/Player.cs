using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private JoystickSystem _joystickSystem;
    [SerializeField] private Animator _animator;
    private bool _isMove;
    private static readonly int AnimationIdle = Animator.StringToHash("Idle");
    private static readonly int AnimationRun = Animator.StringToHash("Run");
    private static readonly int AnimationWalk = Animator.StringToHash("Walk");
    private static readonly int AnimationWalkSlow = Animator.StringToHash("WalkSlow");
    private static readonly int AnimationAttack = Animator.StringToHash("Attack");
    private bool _isSetAnimationRun = false;
    private bool _isSetAnimationWalk = false;
    private bool _isSetAnimationWalkSlow = false;
    private float _stamina = 10;
    private const float StaminaMax = 10;
    private const float StaminaMin = 0;

    private void Start()
    {
        _joystickSystem.PointerDownEvent = () => { _isMove = true; };
        _joystickSystem.PointerUpEvent = () =>
        {
            _isMove = false;
            _animator.SetBool(AnimationIdle, true);
        };
    }

    private void LateUpdate()
    {
        if (_joystickSystem.Distance > 0)
        {
            this.transform.rotation = Quaternion.LookRotation(_joystickSystem.DirectionByVector3.normalized);
            UpdateAnimation();
        }
        else
        {
            _stamina = Math.Min(StaminaMax, _stamina + Time.deltaTime * 3);
        }
    }

    private void UpdateAnimation()
    {
        _stamina = Math.Max(StaminaMin, _stamina - Time.deltaTime);
        if (_joystickSystem.Distance > 60f && _stamina > 0)
        {
            if (!_isSetAnimationRun)
            {
                SetAnimation(AnimationRun);
            }
        }
        else if (_joystickSystem.Distance > 30f)
        {
            if (!_isSetAnimationWalk)
            {
                SetAnimation(AnimationWalk);
            }
        }
        else
        {
            if (!_isSetAnimationWalkSlow)
            {
                SetAnimation(AnimationWalkSlow);
            }
        }
    }

    public void Attack()
    {
        SetAnimation(AnimationAttack);
    }

    private void SetAnimation(int animationHash)
    {
        bool tmp = false;
        SetAnimation_(AnimationAttack, animationHash == AnimationAttack, ref tmp);
        SetAnimation_(AnimationIdle, animationHash == AnimationIdle || animationHash == AnimationAttack, ref tmp);
        SetAnimation_(AnimationWalkSlow, animationHash == AnimationWalkSlow, ref _isSetAnimationWalkSlow);
        SetAnimation_(AnimationWalk, animationHash == AnimationWalk, ref _isSetAnimationWalk);
        SetAnimation_(AnimationRun, animationHash == AnimationRun, ref _isSetAnimationRun);
    }

    private void SetAnimation_(int animationHash, bool isSet, ref bool _flag)
    {
        _flag = isSet;
        if (isSet)
        {
            _animator.SetTrigger(animationHash);
        }
        else
        {
            _animator.ResetTrigger(animationHash);
        }
    }
}
