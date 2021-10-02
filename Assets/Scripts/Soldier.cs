using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Soldier : MonoBehaviour
{
    const float FIRE_RANGE = 100.0f;

    #region parameters
    [SerializeField] [Range(1.0f, 10.0f)]
    float m_coolDownTimeSeconds = 2.0f;
    [SerializeField]
    Vector3 m_fireFromLocalPosition = Vector3.zero;
    [SerializeField] [Range(10.0f, 500.0f)]
    float m_fireForce = 10.0f;
    [SerializeField]
    Projectile m_projectilePrefab = null;
    #endregion

    #region variables
    float m_coolDownCounter = 0.0f;

    Vector3 fireFromGlobalPosition
    {
        get => this.transform.TransformPoint(m_fireFromLocalPosition);
    }
    #endregion

    #region builtins
    private void Start()
    {
        StartCoroutine(WaitAndFire());
    }
    // Update is called once per frame
    void Update()
    {
        m_coolDownCounter += Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Handles.Label(this.transform.position + Vector3.up * 0.2f, string.Format("{0}", m_coolDownCounter));
#endif
    }
    #endregion

    #region control
    IEnumerator WaitAndFire()
    {
        while(true)
        {
            yield return new WaitForSeconds(m_coolDownTimeSeconds);
            m_coolDownCounter = 0.0f;

            FireForward();
        }
    }

    void FireForward()
    {
        Projectile.SpawnFromPrefab(m_projectilePrefab, this.gameObject, this.fireFromGlobalPosition, this.transform.rotation, 3.0f, m_fireForce);

        //if(Physics.Raycast(this.fireFromGlobalPosition, this.transform.forward, out RaycastHit hitInfo, FIRE_RANGE, ~0, QueryTriggerInteraction.Ignore))
        //{
        //    Rigidbody hitBody = hitInfo.rigidbody;
        //    if(hitBody != null)
        //    {
        //        hitBody.AddForceAtPosition(this.transform.forward * m_fireForce, hitInfo.point);
        //        Debug.DrawRay(this.fireFromGlobalPosition, this.transform.forward * FIRE_RANGE, Color.red, 1.0f);
        //    }
        //    else
        //    {
        //        Debug.DrawRay(this.fireFromGlobalPosition, this.transform.forward * FIRE_RANGE, Color.yellow, 1.0f);
        //    }
        //}
        //else
        //{
        //    Debug.DrawRay(this.fireFromGlobalPosition, this.transform.forward * FIRE_RANGE, Color.green, 1.0f);
        //}

    }
    #endregion
}
