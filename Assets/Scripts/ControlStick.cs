using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ControlStick : MonoBehaviour
{
    #region parameters
    [SerializeField] [Range(0.0f, 10000.0f)]
    float m_springPositionStrength = 1000.0f;
    [SerializeField] [Range(0.0f, 1000.0f)]
    float m_springPositionDamper = 100.0f;
    [SerializeField] [Range(0.0f, 10000.0f)]
    float m_springRotationStrength = 1000.0f;
    [SerializeField] [Range(0.0f, 1000.0f)]
    float m_springRotationDamper = 100.0f;
    [SerializeField] [Range(0.0f, 1000.0f)]
    float m_positionForce = 100.0f;
    [SerializeField] [Range(0.0f, 1000.0f)]
    float m_rotationForce = 100.0f;

    [SerializeField]
    GameObject m_stickControlPoint = null;
    [SerializeField]
    GameObject m_stickObject = null;
    [SerializeField] [Range(0.0f, 1.0f)]
    float m_stickDownHeight = 0.1f;
    [SerializeField] [Range(0.0f, 1.0f)]
    float m_stickUpHeight = 0.5f;
    [SerializeField] [Range(0.0f, 10.0f)]
    float m_forwardPivotDistance = 1.0f;
    #endregion

    #region variables
    ConfigurableJoint m_springJoint = null;
    Vector3 m_currentPivotPosition;
    bool m_isPivoting = false;

    bool m_isStickUp = true;
    float m_stickTargetHeight = 0.0f;

    [HideInInspector]
    public Rigidbody m_rigidbody = null;
    [HideInInspector]
    public List<Rigidbody> m_containedSoldierRbs = new List<Rigidbody>();

    public Vector3 stickGroundForward
    {
        get => m_stickControlPoint.transform.forward;
    }
    Vector3 stickGroundPivotDirection
    {
        get => Vector3.ProjectOnPlane((m_currentPivotPosition - m_stickControlPoint.transform.position).normalized, Vector3.up).normalized;
    }
    Vector3 stickForwardPivotPosition
    {
        get => m_stickControlPoint.transform.position + stickGroundForward * m_forwardPivotDistance;
    }
    public Vector3 stickControlPosition
    {
        get => m_stickControlPoint.transform.forward;
    }
    public bool isPivoting { get => m_isPivoting; set => m_isPivoting = value; }
    #endregion

    #region builtins
    private void Awake()
    {
        m_springJoint = this.GetComponentInChildren<ConfigurableJoint>();
        m_springJoint.SetPdParamters(m_springPositionStrength, m_springPositionDamper, m_springRotationStrength, m_springRotationDamper, m_positionForce, m_rotationForce);

        m_rigidbody = m_stickObject.GetComponent<Rigidbody>();

        m_stickTargetHeight = m_stickUpHeight;

        ChildColliderCallback cc = m_stickObject.AddComponent<ChildColliderCallback>();
        cc.onlyTriggerOnUniqueObjects = true;
        cc.onTriggerEnterCallback += OnStickTriggerEnter;
        cc.onTriggerExitCallback += OnStickTriggerExit;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isPivoting)
        {
            Quaternion controlAdjust = Quaternion.FromToRotation(stickGroundForward, stickGroundPivotDirection);
            m_stickControlPoint.transform.rotation *= controlAdjust;
            m_currentPivotPosition = stickForwardPivotPosition;
        }
        else
        {
            m_currentPivotPosition = stickForwardPivotPosition;
        }

        Debug.DrawLine(m_stickControlPoint.transform.position, m_stickControlPoint.transform.position + stickGroundForward);
        Debug.DrawLine(m_stickControlPoint.transform.position, m_stickControlPoint.transform.position + stickGroundPivotDirection);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(stickForwardPivotPosition, 0.05f);    
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(m_currentPivotPosition, 0.05f);

#if UNITY_EDITOR
        Handles.Label(m_currentPivotPosition + Vector3.up * 0.3f, string.Format("{0}", m_containedSoldierRbs.Count));
#endif
    }

    private void OnStickTriggerEnter(Collider other, GameObject gameObject)
    {
        Soldier soldier = other.gameObject.GetComponentInParent<Soldier>();
        if(soldier != null)
        {
            m_containedSoldierRbs.Add(soldier.GetComponent<Rigidbody>());
        }
    }
    private void OnStickTriggerExit(Collider other, GameObject gameObject)
    {
        Soldier soldier = other.gameObject.GetComponentInParent<Soldier>();
        if (soldier != null)
        {
            Rigidbody rb = soldier.GetComponent<Rigidbody>();
            if (m_containedSoldierRbs.Contains(rb))
            {
                m_containedSoldierRbs.Remove(rb);

                // remove a configurable body if it was left on there accidentally.
                ConfigurableJoint js = rb.GetComponent<ConfigurableJoint>();
                if (js != null)
                    Destroy(js);
            }
            else
                Debug.LogError("Tried to remove a soldier that wasn't added to the list!");
        }
    }
    #endregion

    #region controls
    public void SetTargetStickGroundPosition(float posX, float posZ)
    {
        m_stickControlPoint.transform.position = new Vector3(
            posX,
            m_stickTargetHeight,
            posZ);

        // project the control point to the nearest point on the circle.
        //if (isPivoting)
        //{
        //    m_stickControlPoint.transform.position = m_currentPivotPosition + (m_stickControlPoint.transform.position - m_currentPivotPosition).normalized * m_forwardPivotDistance;
        //}
    }

    public void LiftStick()
    {
        if (m_isStickUp)
        {
            //Debug.LogError("Stick is already up.");
            return;
        }

        m_stickTargetHeight = m_stickUpHeight;
        m_isStickUp = true;
    }
    public void DropStick()
    {
        if (!m_isStickUp)
        {
            //Debug.LogError("Stick is already dropped.");
            return;
        }

        m_stickTargetHeight = m_stickDownHeight;
        m_isStickUp = false;
    }
    #endregion
}
