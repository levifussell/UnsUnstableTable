using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Soldier : MonoBehaviour
{
    const float FIRE_RANGE = 100.0f;
    const float DEATH_MARGIN = 0.5f;

    #region parameters
    [SerializeField] [Range(1.0f, 10.0f)]
    float m_coolDownTimeSeconds = 2.0f;
    [SerializeField]
    Vector3 m_fireFromLocalPosition = Vector3.zero;
    [SerializeField] [Range(10.0f, 1000.0f)]
    float m_fireForce = 10.0f;
    [SerializeField]
    Projectile m_projectilePrefab = null;
    [SerializeField]
    Color m_deathColor = Color.gray;
    [SerializeField][Range(0.0f, 1.0f)]
    float m_forceDefenceScale = 0.0f;
    #endregion

    #region variables
    float m_coolDownCounter = 0.0f;

    Material[] m_materials = null;
    Color m_baseColor;

    Rigidbody m_rigidbody;

    bool m_isDead = false;

    Vector3 fireFromGlobalPosition
    {
        get => this.transform.TransformPoint(m_fireFromLocalPosition);
    }
    #endregion

    #region builtins
    private void Awake()
    {
        m_materials = this.GetComponentsInChildren<MeshRenderer>().Select((x) => x.material).ToArray();
        m_baseColor = m_materials[0].color;

        m_rigidbody = this.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        StartCoroutine(WaitAndFire());
    }
    // Update is called once per frame
    void Update()
    {
        m_coolDownCounter += Time.deltaTime;

        if (IsFallenOver())
        {
            if(!m_isDead)
            {
                m_isDead = true;
                SetMaterialsColor(m_deathColor);
            }
        }
        else
        {
            if(m_isDead)
            {
                m_isDead = false;
                SetMaterialsColor(m_baseColor);
            }
        }
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

            if (!m_isDead)
            {
                FireForward();
            }
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

    bool IsFallenOver()
    {
        return Vector3.Dot(this.transform.up, Vector3.up) < DEATH_MARGIN;
    }

    void SetMaterialsColor(Color color)
    {
        foreach(Material m in m_materials)
        {
            m.color = color;
        }
    }

    public void ApplyForce(Vector3 force, Vector3 position)
    {
        this.m_rigidbody.AddForceAtPosition(force * (1.0f - m_forceDefenceScale), position);
    }
    #endregion
}
