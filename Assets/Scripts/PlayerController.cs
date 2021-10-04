using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region parameters
    [SerializeField][Range(0.0f, 10.0f)]
    float m_controlRate = 1.0f;

    [SerializeField]
    GameObject m_controlCursorPrefab;

    [SerializeField]
    ControlStick m_controlStick;

    [SerializeField] [Range(0.0f, 1.0f)]
    float m_cursorFloorOffset = 0.01f;

    [SerializeField] [Range(0.0f, 1.0f)]
    float m_cameraMoveScaleForward = 0.2f;
    [SerializeField] [Range(0.0f, 1.0f)]
    float m_cameraMoveScaleSideways = 0.05f;
    #endregion

    #region variables
    Camera m_camera;
    Vector3 m_cameraInitLocalPosition;
    GameObject m_controlCursor;
    GameObject m_pivotCursor;
    bool m_stickToggle = false;

    Vector3 m_currentControlPoint;
    Vector3 m_currentPivotPoint;
    int m_groundMask;

    Action m_onMouseLeftDown;
    Action m_onMouseLeftUp;
    Action m_onMouseRightDown;
    Action m_onMouseRightUp;

    public Vector3 TABLE_CENTER = Vector3.zero;
    #endregion

    #region data
    struct ControlHitData
    {
        public bool isHit;
        public Vector3 position;
        public Vector3 normal;
    }
    #endregion

    #region builtin

    private void Awake()
    {
        m_camera = this.GetComponentInChildren<Camera>();
        if (m_camera == null)
            Debug.LogError("Player is mmissing a camera!");
        m_cameraInitLocalPosition = m_camera.transform.localPosition;

        //m_onMouseLeftDown += m_controlStick.DropStick;
        //m_onMouseLeftUp += m_controlStick.LiftStick;
        m_onMouseLeftDown += ToggleStick;
        m_onMouseRightDown += EnablePivoting;
        m_onMouseRightDown += LockContainedSoldiersToStick;
        m_onMouseRightUp += DisablePivoting;
        m_onMouseRightUp += UnlockContainedSoldiersToStick;

        m_currentControlPoint = this.transform.position;
        m_groundMask = LayerMask.GetMask("Ground");
    }

    private void Start()
    {
        m_controlCursor = GameObject.Instantiate(m_controlCursorPrefab);
        //m_pivotCursor = GameObject.Instantiate(m_controlCursorPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        ControlHitData hitInfo = GetProjectedMouseControlInfo();
        if(hitInfo.isHit)
        {
            Vector3 controlDiff = (hitInfo.position + hitInfo.normal * m_cursorFloorOffset) - m_currentControlPoint;
            Vector3 controlIntegration = Mathf.Min(controlDiff.magnitude, Time.deltaTime * m_controlRate * controlDiff.magnitude) * controlDiff.normalized;
            m_currentControlPoint += controlIntegration;

            m_controlCursor.transform.position = m_currentControlPoint;
            m_controlStick.SetTargetStickGroundPosition(m_currentControlPoint.x, m_currentControlPoint.z);
        }

        m_controlCursor.transform.rotation = Quaternion.Euler(0.0f, m_controlStick.m_rigidbody.transform.rotation.eulerAngles.y, 0.0f) * Quaternion.Euler(90.0f, 0.0f, 0.0f);

        // move the camera

        Vector3 cameraDrift = (m_controlCursor.transform.position - TABLE_CENTER);
        cameraDrift.x *= m_cameraMoveScaleSideways;
        cameraDrift.y *= m_cameraMoveScaleSideways;
        cameraDrift.z *= m_cameraMoveScaleForward;
        m_camera.transform.position = m_camera.transform.parent.TransformPoint(m_cameraInitLocalPosition) + cameraDrift;

        // inputs.

        if (Input.GetMouseButtonDown(0))
            m_onMouseLeftDown?.Invoke();
        //if (Input.GetMouseButtonUp(0))
        //    m_onMouseLeftUp?.Invoke();
        if (Input.GetMouseButtonDown(1))
            m_onMouseRightDown?.Invoke();
        if (Input.GetMouseButtonUp(1))
            m_onMouseRightUp?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if(Application.isPlaying)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_currentControlPoint, 0.1f);
        }
    }

    #endregion

    #region controls

    ControlHitData GetProjectedMouseControlInfo()
    {
        Ray mouseRay = this.m_camera.ScreenPointToRay(new Vector3(
            Input.mousePosition.x,
            Input.mousePosition.y,
            0.0f));

        ControlHitData hitData;
        hitData.isHit = Physics.Raycast(mouseRay, out RaycastHit info, 10.0f, m_groundMask, QueryTriggerInteraction.Ignore);
        hitData.position = info.point;
        hitData.normal = info.normal;

        return hitData;
    }

    void ToggleStick()
    {
        if (m_controlStick.isPivoting) // no lifting the stick while pivoting.
            return;

        if (!m_stickToggle)
            m_controlStick.DropStick();
        else
            m_controlStick.LiftStick();

        m_stickToggle = !m_stickToggle;
    }

    void EnablePivoting()
    {
        m_controlStick.isPivoting = true;
    }
    void DisablePivoting()
    {
        m_controlStick.isPivoting = false;
    }
    
    void LockContainedSoldiersToStick()
    {
        foreach(Rigidbody rb in m_controlStick.m_containedSoldierRbs)
        {
            ConfigurableJoint js = rb.gameObject.AddComponent<ConfigurableJoint>();
            js.connectedBody = m_controlStick.m_rigidbody;
            js.SetAllJointMotions(ConfigurableJointMotion.Locked);
        }
    }

    void UnlockContainedSoldiersToStick()
    {
        foreach(Rigidbody rb in m_controlStick.m_containedSoldierRbs)
        {
            ConfigurableJoint js = rb.gameObject.GetComponent<ConfigurableJoint>();
            Destroy(js);
        }
    }

    #endregion
}
