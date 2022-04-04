using System;
using System.Collections;
using System.Collections.Generic;
using mactinite.EDS.Basic;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

public class FlyingEnemyStateMachine : StateMachine<FlyingEnemyStateMachine>
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
    public float attackWindUpDuration = 0.5f;

    public Projectile projectilePrefab;
    public Transform projectileSpawnPosition;
    public float projectileForce = 50f;

    private void Start()
    {
        
        damageReceiver.OnDamage += OnDamage;
        damageReceiver.OnDestroyed += OnDestroyed;
        StateMap.Add("Idle", new FlyingIdleState(this));
        StateMap.Add("Move", new FlyingMoveState(this));
        StateMap.Add("Stunned", new FlyingStunState(this));
        StateMap.Add("Attack", new ProjectileAttack(this));

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


public class FlyingIdleState : State<FlyingEnemyStateMachine>
{
    public FlyingIdleState(FlyingEnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
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

public class FlyingMoveState : State<FlyingEnemyStateMachine>
{
    public FlyingMoveState(FlyingEnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        // play walk animation and move to player
        StateMachine.animator.Play("Idle");

        while (StateMachine.currentTarget != null)
        {
            StateMachine.navMeshAgent.SetDestination(StateMachine.currentTarget.position);
            float distance = Vector3.Distance(StateMachine.transform.position, StateMachine.currentTarget.position);
            if (distance <= StateMachine.attackRange)
            {
                StateMachine.navMeshAgent.isStopped = true;
                t += Time.deltaTime;
            }
            else if(t > 0)
            {
                StateMachine.navMeshAgent.isStopped = false;
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

public class ProjectileAttack : State<FlyingEnemyStateMachine>
{
    
    public ProjectileAttack(FlyingEnemyStateMachine stateMachine) : base(stateMachine)
    {
    }
    public override IEnumerator Start()
    {
        float t = 0;

        StateMachine.navMeshAgent.isStopped = true;
        StateMachine.animator.Play("Wind Up");
        yield return new WaitForSeconds(StateMachine.attackWindUpDuration);
        StateMachine.animator.Play("Attack");
        // play idle animation and wait until we see the player
        yield return new WaitForSeconds(StateMachine.attackDuration);

        var position = StateMachine.projectileSpawnPosition.position;
        Vector3 dirToTarget = StateMachine.currentTarget.position - position;
        Quaternion rotation = Quaternion.LookRotation(dirToTarget.normalized, StateMachine.transform.up);
        var projectile = Object.Instantiate(StateMachine.projectilePrefab, position, rotation);
        projectile.enabled = false;
        while (t < StateMachine.attackDuration)
        {
            rotation = Quaternion.LookRotation(dirToTarget.normalized, StateMachine.transform.up);
            projectile.transform.rotation = rotation;
            projectile.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t/StateMachine.attackDuration);
            t += Time.deltaTime;
            yield return null;
        }

        projectile.bulletSpeed = StateMachine.projectileForce;
        projectile.enabled = true;
        
        yield return new WaitForSeconds(StateMachine.attackRecoveryDuration);
        StateMachine.navMeshAgent.isStopped = false;
        StateMachine.SetState("Move");
    }
}

public class FlyingStunState : State<FlyingEnemyStateMachine>
{
    public FlyingStunState(FlyingEnemyStateMachine stateMachine) : base(stateMachine)
    {
    }

    public override IEnumerator Start()
    {
        float t = 0;
        StateMachine.animator.Play("Stun");
        StateMachine.navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(0.3f);
        StateMachine.navMeshAgent.isStopped = false;
        StateMachine.SetState("Move");
    }
}
