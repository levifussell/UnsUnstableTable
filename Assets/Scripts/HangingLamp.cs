using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HangingLamp : MonoBehaviour
{
    #region variables
    [SerializeField][Range(0.0f, 1.0f)]
    float m_forceResponseScale = 1.0f;
    #endregion

    #region variables
    Rigidbody m_rigidbody;
    #endregion

    private void Awake()
    {
        m_rigidbody = this.GetComponent<Rigidbody>(); 

        ProjectileDeathManager.Instance.onProjectileDeath += OnProjectileDeath;
    }

    void OnProjectileDeath(Vector3 force, Vector3 position)
    {
        //m_rigidbody.AddForce(force * m_forceResponseScale);
        if(m_rigidbody.velocity.magnitude < 0.01f)
            m_rigidbody.AddForce((this.transform.position - position).normalized * force.magnitude * m_forceResponseScale, ForceMode.Impulse);
    }
}
