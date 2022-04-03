using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using mactinite.EDS.Basic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerStateMachine : StateMachine<PlayerStateMachine>
{
    
    public float punchDuration = 0.5f;
    public float punchForwardSpeed = 10f;
    public float punchHitboxActivateTime = 0.25f;
    public float punchDistance = 1f;
    [SerializeField] public AnimationCurve punchAnimationTiming;
    public GameObject punchHitbox;

    
    public float kickDuration = 0.5f;
    public float kickUpwardSpeed = 10f;
    public float kickDownwardSpeed = 10f;
    public float kickHitboxActivateTime = 0.25f;
    public float kickHeight = 4f;
    public GameObject kickUpHitbox;
    public GameObject kickDownHitbox;
    public float airKickRecoveryTime = 0.2f;

    
    [SerializeField]
    public ActionCharacterController characterController;

    [SerializeField] internal Animator animator;

    public BasicDamageReceiver damageReceiver;



    public ActionCharacterController CharacterController
    {
        get => characterController;
        set => characterController = value;
    }

    public float stunDuration = 0.2f;


    private void Start()
    {
        damageReceiver.OnDamage += OnDamage;
        damageReceiver.OnDestroyed += OnDeath;

        StateMap.Add("Ground",  new GroundState(this));
        StateMap.Add("Air",  new AirState(this));
        StateMap.Add("Punch",  new PunchState(this, "Punch", "Punch 2"));
        StateMap.Add("Punch 2",  new PunchState(this, "Punch 2", "Punch"));
        StateMap.Add("Kick",  new KickState(this));
        StateMap.Add("AirKick",  new AirKickState(this));
        StateMap.Add("Stun",  new PlayerStunState(this));
        SetState("Ground");
    }

    private void OnDeath(Vector2 pos)
    {
        gameObject.SetActive(false);
        Debug.Log("Game Over");
    }

    private void OnDamage(Vector2 pos, BasicDamage damage)
    {
        
    }
}

public class PlayerStunState : State<PlayerStateMachine>
{
    public PlayerStunState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.animator.Play("Stun");
        StateMachine.CharacterController.canControl = false;
        StateMachine.CharacterController.movement.useGravity = false;
        yield return new WaitForSeconds(StateMachine.stunDuration);
        StateMachine.CharacterController.canControl = true;
        StateMachine.CharacterController.movement.useGravity = true;
        StateMachine.SetState("Ground");
    }
}


public class GroundState : State<PlayerStateMachine>
{
    public GroundState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public GroundState()
    {
    }

    public override IEnumerator Start()
    {
        // Allow movement and ground attacks
        while (StateMachine.CharacterController.isGrounded)
        {
            if (StateMachine.CharacterController.movement.forwardSpeed > 0)
            {
                StateMachine.animator.Play("Walk");
            }
            else
            {
                StateMachine.animator.Play("Idle");
            }
            
            // transition to ground attack states, punch & kick
            if (StateMachine.CharacterController.input.actions["Punch"].triggered)
            {
                StateMachine.SetState("Punch");
                yield break;
            }
            
            if (StateMachine.CharacterController.input.actions["Kick"].triggered)
            {
                StateMachine.SetState("Kick");
                yield break;
            }

            if (StateMachine.damageReceiver.wasDamagedThisFrame)
            {
                StateMachine.SetState("Stun");
                yield break;
            }
            
            yield return null;
        }

        StateMachine.SetState("Air");
    }
    
}

public class AirState : State<PlayerStateMachine>
{
    
    public AirState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public AirState()
    {
    }

    public override IEnumerator Start()
    {
        
        // Allow air movement and air attacks
        while (!this.StateMachine.CharacterController.isGrounded)
        {

            if (StateMachine.CharacterController.movement.velocity.y > 0)
            {
                StateMachine.animator.Play("Jump");
            }
            else
            {
                StateMachine.animator.Play("Fall");
            }
            
            if (StateMachine.damageReceiver.wasDamagedThisFrame)
            {
                StateMachine.SetState("Stun");
                yield break;
            }
            
            // transition to air attack states, punch & kick
            if (StateMachine.CharacterController.input.actions["Punch"].triggered)
            {
                StateMachine.SetState("Punch");
                yield break;
            }
            
            if (StateMachine.CharacterController.input.actions["Kick"].triggered)
            {
                StateMachine.SetState("AirKick");
                yield break;
            }
            


            yield return null;
        }
        StateMachine.SetState("Ground");
    }
}

public class PunchState : State<PlayerStateMachine>
{
    private string animation;
    private string nextAnimationState;
    public PunchState(PlayerStateMachine stateMachine, string animationState, string nextAnimationState) : base(stateMachine)
    {
        this.animation = animationState;
        this.nextAnimationState = nextAnimationState;
    }

    public PunchState()
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.CharacterController.canControl = false;
        StateMachine.CharacterController.movement.useGravity = false;
        StateMachine.CharacterController.movement.velocity = Vector3.zero;
        
        Vector3 startPosition = StateMachine.CharacterController.transform.position;
        StateMachine.animator.Play(animation);
        while (t <= StateMachine.punchDuration)
        {
            
            if (StateMachine.damageReceiver.wasDamagedThisFrame)
            {
                StateMachine.punchHitbox.SetActive(false);
                StateMachine.CharacterController.movement.velocity = Vector3.zero;
                StateMachine.SetState("Stun");
                yield break;
            }
            
            if (t > StateMachine.punchDuration * 0.4f && StateMachine.CharacterController.input.actions["Punch"].triggered)
            {
                StateMachine.punchHitbox.SetActive(false);
                StateMachine.SetState(nextAnimationState);
                yield break;
            }

            if (t > StateMachine.punchHitboxActivateTime)
            {
                StateMachine.punchHitbox.SetActive(true);
            }
            
            float evaluatedT = StateMachine.punchAnimationTiming.Evaluate(Mathf.Clamp01(t / StateMachine.punchDuration));
            // StateMachine.transform.position = Vector3.Lerp(startPosition, startPosition + (StateMachine.transform.forward * StateMachine.punchForwardSpeed), evaluatedT);
            float speed = Mathf.Lerp(0, StateMachine.punchForwardSpeed, evaluatedT);
            StateMachine.characterController.movement.Move(StateMachine.transform.forward * speed, StateMachine.punchForwardSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        
        StateMachine.CharacterController.canControl = true;
        StateMachine.CharacterController.movement.useGravity = true;
        StateMachine.punchHitbox.SetActive(false);
        StateMachine.SetState("Ground");
    }

}


public class KickState : State<PlayerStateMachine>
{
    public KickState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public KickState()
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        float maxHeight = StateMachine.transform.position.y + StateMachine.kickHeight;
        StateMachine.CharacterController.canControl = false;
        StateMachine.CharacterController.movement.velocity = Vector3.zero;
        StateMachine.animator.Play("Punch Up");
        while (t < StateMachine.kickDuration || StateMachine.transform.position.y < maxHeight)
        {
            StateMachine.CharacterController.movement.ApplyVerticalImpulse(StateMachine.kickUpwardSpeed);
            StateMachine.CharacterController.movement.DisableGrounding();

            if (t > StateMachine.kickHitboxActivateTime)
            {
                StateMachine.kickUpHitbox.SetActive(true);
            }
            
            t += Time.deltaTime;
            yield return null;
        }
        
        StateMachine.CharacterController.canControl = true;
        StateMachine.kickUpHitbox.SetActive(false);
        StateMachine.SetState("Air");
    }
}

public class AirKickState : State<PlayerStateMachine>
{
    public AirKickState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public AirKickState()
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.CharacterController.movement.useGravity = false;
        StateMachine.CharacterController.canControl = false;
        StateMachine.animator.Play("Slam");
        while (!StateMachine.CharacterController.isGrounded)
        {
            if (t > StateMachine.kickHitboxActivateTime)
            {
                StateMachine.kickDownHitbox.SetActive(true);
            }
            StateMachine.CharacterController.movement.ApplyImpulse(-(StateMachine.transform.up) * StateMachine.kickDownwardSpeed);
            t += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(StateMachine.airKickRecoveryTime);
        
        StateMachine.CharacterController.movement.useGravity = true;
        StateMachine.CharacterController.canControl = true;
        StateMachine.kickDownHitbox.SetActive(false);

        StateMachine.SetState("Ground");
    }
}


