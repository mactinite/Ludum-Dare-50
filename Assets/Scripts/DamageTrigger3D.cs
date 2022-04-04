using System;
using System.Collections;
using System.Collections.Generic;
using ECM.Components;
using mactinite.EDS.Basic;
using mactinite.ToolboxCommons;
using UnityEngine;

public class DamageTrigger3D : MonoBehaviour
{
    public float damage = 10;
    public float knockBackForce = 10;

    public LayerMask damageMask;

    public SoundBox hitSounds;
    public PrefabSpawner hitEffectsSpawner;

    public List<Collider> previouslyHit = new List<Collider>();
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.IsInLayerMask(damageMask) || previouslyHit.Contains(collision))
        {
            
            // if other collider has a DamageReceiver, apply that damage
            if (collision.TryGetComponent<BasicDamageReceiver>(out var DamageReceiver))
            {
                DamageReceiver.DamageAt(new BasicDamage(damage), collision.ClosestPoint(transform.position));
            }

            if (collision.TryGetComponent<CharacterMovement>(out var movement))
            {
                movement.ApplyImpulse(transform.forward * knockBackForce);
                movement.DisableGrounding();

            } else if (collision.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.AddForce(transform.forward * knockBackForce);
            }
            
            previouslyHit.Add(collision);
            hitSounds?.PlayRandomOneShot();
            hitEffectsSpawner?.SpawnPrefabAt(collision.ClosestPoint(transform.position));
        }
    }

    private void OnDisable()
    {
        previouslyHit.Clear();
    }
}