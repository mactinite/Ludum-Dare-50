using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine<PlayerStateMachine>
{

    public float punchDuration = 0.5f;
    public float punchForwardSpeed = 10f;
    public GameObject punchHitbox;

    
    public float kickDuration = 0.5f;
    public float kickUpwardSpeed = 10f;
    public GameObject kickHitbox;


    
    [SerializeField]
    public ActionCharacterController characterController;

    

    public ActionCharacterController CharacterController
    {
        get => characterController;
        set => characterController = value;
    }


    private void Start()
    {
        StateMap.Add("Ground",  new GroundState(this));
        StateMap.Add("Air",  new AirState(this));
        StateMap.Add("Punch",  new PunchState(this));
        StateMap.Add("Kick",  new KickState(this));
        StateMap.Add("AirKick",  new AirKickState(this));
        
        SetState("Ground");
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
    public PunchState(PlayerStateMachine stateMachine) : base(stateMachine)
    {
    }

    public PunchState()
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.CharacterController.enabled = false;
        StateMachine.CharacterController.movement.useGravity = false;
        StateMachine.punchHitbox.SetActive(true);
        while (t <= StateMachine.punchDuration)
        {
            StateMachine.characterController.movement.velocity = Vector3.zero;
            StateMachine.characterController.movement.Move(StateMachine.transform.forward * StateMachine.punchForwardSpeed, StateMachine.punchForwardSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        
        StateMachine.CharacterController.enabled = true;
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
        StateMachine.CharacterController.enabled = false;
        StateMachine.CharacterController.movement.useGravity = false;
        StateMachine.kickHitbox.SetActive(true);
        StateMachine.characterController.movement.velocity = Vector3.zero;
        while (t <= StateMachine.kickDuration)
        {
            
            StateMachine.characterController.movement.ApplyVerticalImpulse(StateMachine.kickUpwardSpeed);
            t += Time.deltaTime;
            yield return null;
        }
        
        StateMachine.CharacterController.enabled = true;
        StateMachine.CharacterController.movement.useGravity = true;
        StateMachine.characterController.movement.snapToGround = true;

        StateMachine.kickHitbox.SetActive(false);
        StateMachine.SetState("Ground");
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
        StateMachine.CharacterController.enabled = false;
        StateMachine.CharacterController.movement.useGravity = false;
        StateMachine.kickHitbox.SetActive(true);
        StateMachine.characterController.movement.velocity = Vector3.zero;

        while (t <= StateMachine.kickDuration)
        {
            StateMachine.CharacterController.movement.Move((-StateMachine.transform.up) * StateMachine.kickUpwardSpeed, StateMachine.kickUpwardSpeed, false);
            t += Time.deltaTime;
            yield return null;
        }
        
        StateMachine.CharacterController.enabled = true;
        StateMachine.CharacterController.movement.useGravity = true;
        StateMachine.kickHitbox.SetActive(false);
        StateMachine.SetState("Ground");
    }

}
