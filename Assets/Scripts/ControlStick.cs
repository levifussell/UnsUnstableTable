using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlStick : MonoBehaviour
{
    #region parameters
    [SerializeField] [Range(0.0f, 3000.0f)]
    float m_springPositionStrength = 1000.0f;
    [SerializeField] [Range(0.0f, 300.0f)]
    float m_springPositionDamper = 100.0f;
    [SerializeField] [Range(0.0f, 3000.0f)]
    float m_springRotationStrength = 1000.0f;
    [SerializeField] [Range(0.0f, 300.0f)]
    float m_springRotationDamper = 100.0f;

    [SerializeField]
    GameObject m_stickControlPoint = null;
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
        m_springJoint.SetPdParamters(m_springPositionStrength, m_springPositionDamper, m_springRotationStrength, m_springRotationDamper, 100.0f);

        m_stickTargetHeight = m_stickUpHeight;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isPivoting)
        {
            Quaternion controlAdjust = Quaternion.FromToRotation(stickGroundForward, stickGroundPivotDirection);
            m_stickControlPoint.transform.rotation *= controlAdjust;
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
        //if(isPivoting)
        //{
        //    m_stickControlPoint.transform.position = m_currentPivotPosition + (m_stickControlPoint.transform.position - m_currentPivotPosition).normalized * m_forwardPivotDistance;
        //}
    }

    public void LiftStick()
    {
        if (m_isStickUp)
        {
            Debug.LogError("Stick is already up.");
            return;
        }

        m_stickTargetHeight = m_stickUpHeight;
        m_isStickUp = true;
    }
    public void DropStick()
    {
        if (!m_isStickUp)
        {
            Debug.LogError("Stick is already dropped.");
            return;
        }

        m_stickTargetHeight = m_stickDownHeight;
        m_isStickUp = false;
    }
    #endregion
}
