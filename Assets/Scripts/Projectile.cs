using System;
using System.Collections;
using System.Collections.Generic;
using ECM.Components;
using mactinite.EDS.Basic;
using mactinite.ToolboxCommons;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float damage = 10;
    public float knockBackForce = 10;
    
    public LayerMask damageMask;
    public float lifetime = 5;
    public float fadeTime = 1;

    private Rigidbody _rigidbody;

    public float bulletSpeed = 50f;


    private void OnTriggerEnter(Collider collision)
    {
        if (enabled && collision.gameObject.IsInLayerMask(damageMask))
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
            Destroy(this.gameObject);
        }
        
    }




    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.AddForce(transform.forward * bulletSpeed);
        StartCoroutine(FadeAway());
    }


    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(lifetime);
        float t = 0;
        while (t < fadeTime)
        {
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t/fadeTime);
            
            t += Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
        
    }
}
