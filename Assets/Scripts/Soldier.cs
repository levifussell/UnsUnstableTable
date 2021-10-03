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
    [SerializeField]
    bool m_canShoot = true;
    #endregion

    #region variables
    float m_coolDownCounter = 0.0f;
    RingTimer m_ringTimer = null;

    Material[] m_materials = null;
    Color m_baseColor;

    Rigidbody m_rigidbody;

    int m_layerInit;

    bool is_warmup = true;
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

        m_ringTimer = this.GetComponentInChildren<RingTimer>();
        if(m_ringTimer != null)
        {
            this.m_ringTimer.m_ringTimer = this.m_coolDownTimeSeconds;
        }

        m_layerInit = this.gameObject.layer;
    }
    private void Start()
    {
        if(m_canShoot)
        {
            StartCoroutine(WaitAndFire());
        }
    }
    // Update is called once per frame
    void Update()
    {
        m_coolDownCounter += Time.deltaTime;

        if (m_ringTimer != null && !is_warmup)
            m_ringTimer.SetTime(m_coolDownCounter);

        if (IsFallenOver())
        {
            if(!m_isDead)
            {
                m_isDead = true;
                this.gameObject.SetLayerRecursive(0);
                SetMaterialsColor(m_deathColor);
                if (m_ringTimer != null)
                    m_ringTimer.gameObject.SetActive(false);
            }
        }
        else
        {
            if(m_isDead)
            {
                m_isDead = false;
                this.gameObject.SetLayerRecursive(m_layerInit);
                SetMaterialsColor(m_baseColor);
                if (m_ringTimer != null)
                    m_ringTimer.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(Random.Range(0.0f, this.m_coolDownTimeSeconds));
        is_warmup = false;

        while (true)
        {
            m_coolDownCounter = 0.0f;
            yield return new WaitForSeconds(m_coolDownTimeSeconds);

            if (!m_isDead)
            {
                FireForward();
            }
        }
    }

    void FireForward()
    {
        Projectile.SpawnFromPrefab(m_projectilePrefab, this.gameObject, this.fireFromGlobalPosition, this.transform.rotation, 3.0f, m_fireForce, this.m_rigidbody.velocity);

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
