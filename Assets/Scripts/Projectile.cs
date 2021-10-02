using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Projectile : MonoBehaviour
{
    #region parameters
    [SerializeField] [Range(0.0f, 10.0f)]
    float m_speed = 1.0f;
    float m_fireForce = 200.0f;
    GameObject m_parentSource = null;
    float m_timeToDeath = 5.0f;
    #endregion

    #region variables
    ParticleSystem m_particleSystem;
    bool m_isDead = false;
    #endregion

    public static Projectile SpawnFromPrefab(
        Projectile prefab, 
        GameObject parentSource,
        Vector3 position, 
        Quaternion orientation, 
        float speed, 
        float fireForce)
    {
        Projectile newProjectile = GameObject.Instantiate(prefab);
        newProjectile.m_parentSource = parentSource;
        newProjectile.transform.position = position;
        newProjectile.transform.rotation = orientation;
        newProjectile.m_speed = speed;
        newProjectile.m_fireForce = fireForce;
        return newProjectile;
    }

    #region builtins
    void Awake()
    {
        m_particleSystem = this.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame 
    void Update()
    {
        this.transform.position += this.transform.forward * this.m_speed * Time.deltaTime;

        this.m_timeToDeath -= Time.deltaTime;
        if (!this.m_isDead && this.m_timeToDeath <= 0.0f)
        {
            StartCoroutine(WaitForParticlesAndDie());
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (this.m_isDead || other.gameObject.GetTopmostParent() == m_parentSource)
            return;

        Vector3 force = this.transform.forward * m_fireForce;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Soldier soldier = rb.GetComponent<Soldier>();
            if (soldier != null)
                soldier.ApplyForce(force, this.transform.position);
            else
                rb.AddForceAtPosition(force, this.transform.position);
        }

        StartCoroutine(WaitForParticlesAndDie());

        ProjectileDeathManager.Instance.RegisterNewProjectileDeath(force, this.transform.position);
    }
    #endregion

    #region controls
    IEnumerator WaitForParticlesAndDie()
    {
        while(true)
        {
            if(!this.m_isDead)
            {
                this.m_isDead = true;
                this.m_speed = 0.0f;
                ParticleSystem.EmissionModule em = this.m_particleSystem.emission;
                em.enabled = false;
                ParticleSystem.VelocityOverLifetimeModule vl = this.m_particleSystem.velocityOverLifetime;
                vl.speedModifierMultiplier = 0.0f;
                Destroy(this.GetComponent<MeshRenderer>());
            }

            if (this.m_particleSystem.particleCount == 0)
            {
                Destroy(this.gameObject);
                yield return null;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }
    #endregion
}
