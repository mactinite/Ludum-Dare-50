using System;
using System.Collections;
using System.Collections.Generic;
using mactinite.EDS;
using mactinite.EDS.Basic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySoldierStateMachine : StateMachine<EnemySoldierStateMachine>
{
    public LayerMask playerLayerMask;
    public LayerMask obstacleLayerMask;
    public BasicDamageReceiver damageReceiver;
    public Animator animator;

    public NavMeshAgent navMeshAgent;

    public float visionRange;
    public Transform currentTarget;

    internal Collider[] targets = new Collider[5];

    public float attackRange = 0.5f;
    public float attackDuration = 1f;
    public float attackRecoveryDuration = 1f;

    public GameObject attackHitbox;

    public float stunDuration = 1f;
    public float attackWindupDuration = 0.5f;
    public float attackHitboxDelay = 0.2f;

    // Start is called before the first frame update
    void Start()
    {

        damageReceiver.OnDamage += OnDamage;
        damageReceiver.OnDestroyed += OnDestroyed;
        StateMap.Add("Idle", new IdleState(this));
        StateMap.Add("Move", new MoveState(this));
        StateMap.Add("Attack", new AttackState(this));
        StateMap.Add("Stunned", new StunState(this));
        SetState("Idle");
    }

    private void OnDestroy()
    {
        damageReceiver.OnDamage -= OnDamage;
        damageReceiver.OnDestroyed -= OnDestroyed;
    }

    private void OnDestroyed(Vector2 pos)
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }

    private void OnDamage(Vector2 pos, BasicDamage dmg)
    {
        
    }
}


public class IdleState : State<EnemySoldierStateMachine>
{
    public IdleState(EnemySoldierStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        // play idle animation and wait until we see the player
        StateMachine.animator.Play("Idle");
        while (StateMachine.currentTarget == null)
        {
            if (StateMachine.damageReceiver.wasDamagedThisFrame)
            {
                StateMachine.SetState("Stunned");
                yield break;
            }
            
            var size = Physics.OverlapSphereNonAlloc(StateMachine.transform.position, StateMachine.visionRange,
                StateMachine.targets, StateMachine.playerLayerMask);
            
            if (size > 0)
            {
                StateMachine.currentTarget = StateMachine.targets[0].transform;
                StateMachine.SetState("Move");
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
            }
            

        }
    }
}

public class MoveState : State<EnemySoldierStateMachine>
{
    public MoveState(EnemySoldierStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        // play walk animation and move to player
        StateMachine.animator.Play("Walk");

        while (StateMachine.currentTarget != null)
        {
            StateMachine.navMeshAgent.SetDestination(StateMachine.currentTarget.position);
            float distance = Vector3.Distance(StateMachine.transform.position, StateMachine.currentTarget.position);
            if (distance <= StateMachine.attackRange)
            {
                t += Time.deltaTime;
            }
            else if(t > 0)
            {
                t -= Time.deltaTime;
            }

            if (t > 0.5f)
            {
                StateMachine.SetState("Attack");
                yield break;
            }

            if (StateMachine.damageReceiver.wasDamagedThisFrame)
            {
                StateMachine.SetState("Stunned");
                yield break;
            }
            
            yield return null;
        }
        
        StateMachine.SetState("Idle");
    }
}

public class AttackState : State<EnemySoldierStateMachine>
{
    public AttackState(EnemySoldierStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.navMeshAgent.isStopped = true;
        StateMachine.animator.Play("Wind Up");
        yield return new WaitForSeconds(StateMachine.attackWindupDuration);
        StateMachine.animator.Play("Attack");
        yield return new WaitForSeconds(StateMachine.attackHitboxDelay);
        StateMachine.attackHitbox.SetActive(true);
        // play idle animation and wait until we see the player
        yield return new WaitForSeconds(StateMachine.attackDuration);
        StateMachine.attackHitbox.SetActive(false);
        yield return new WaitForSeconds(StateMachine.attackRecoveryDuration);
        StateMachine.navMeshAgent.isStopped = false;
        StateMachine.SetState("Move");
    }
}


public class StunState : State<EnemySoldierStateMachine>
{
    public StunState(EnemySoldierStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.animator.Play("Stun");
        StateMachine.navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(StateMachine.stunDuration);
        StateMachine.navMeshAgent.isStopped = false;
        StateMachine.SetState("Move");
    }
}