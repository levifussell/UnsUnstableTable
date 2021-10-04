using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KjuAnimation : MonoBehaviour
{
    [SerializeField]
    Image m_imageFace = null;

    [SerializeField]
    Image m_imageJaw = null;

    [SerializeField]
    float m_maxJawExtent = 0.1f;

    [SerializeField]
    public Vector2 m_targetPosition = new Vector2(-489.0f, 462.0f);

    RectTransform m_baseRect = null;
    RectTransform m_jawRect = null;
    RectTransform m_faceRect = null;
    Vector2 m_faceOffset = Vector2.zero;
    Quaternion m_faceRotOffset = Quaternion.identity;
    Vector2 m_basePos;
    Quaternion m_baseRotation;
    float m_currentPeriod = 10.0f;
    float m_currentAmp = 1.0f;
    float m_periodSwitchRate = 0.3f;
    float m_timer = 0.0f;

    Vector2 m_initPosition;

    private void Awake()
    {
        m_baseRect = this.GetComponent<RectTransform>();
        m_jawRect = m_imageJaw.GetComponent<RectTransform>();
        m_faceRect = m_imageFace.GetComponent<RectTransform>();
        m_basePos = m_faceRect.anchoredPosition;

        m_baseRotation = m_faceRect.transform.rotation;

        m_initPosition = m_baseRect.anchoredPosition;
    }

    private void Update()
    {
        m_timer += Time.deltaTime;
        if (m_timer > m_periodSwitchRate)
        {
            m_timer = 0.0f;
            m_currentPeriod = Random.Range(5.0f, 10.0f);
            m_currentAmp = Random.Range(m_currentAmp * 0.5f, m_maxJawExtent);

            if(Random.Range(0.0f, 1.0f) < 0.2f)
            {
                m_currentAmp = 0.0f;
                m_periodSwitchRate = 1.0f;
            }
            else
            {
                m_periodSwitchRate = 0.3f;
            }
        }

        m_baseRect.anchoredPosition += (m_targetPosition - m_baseRect.anchoredPosition) * 0.1f;

        Vector2 upPlane = new Vector2(m_jawRect.transform.up.x, m_jawRect.transform.up.y);
        m_jawRect.anchoredPosition = m_faceOffset + m_basePos - upPlane * Mathf.Abs(Mathf.Sin(Time.time * m_currentPeriod)) * m_currentAmp;
        m_faceRect.anchoredPosition = m_faceOffset + m_basePos;

        m_faceRect.transform.rotation = m_faceRotOffset * m_baseRotation;
        m_jawRect.transform.rotation = m_faceRotOffset * m_baseRotation;

        m_faceOffset = upPlane * Mathf.Sin(Time.time * 3.0f) * 10.0f;
        m_faceRotOffset = Quaternion.Euler(0.0f, 0.0f, Mathf.Sin(Time.time * 4.0f) * 7.0f);

    }

    public void Reset()
    {
        m_baseRect.anchoredPosition = m_initPosition;
    }

    public void SetColor(Color color)
    {
        m_imageFace.color = color;
        m_imageJaw.color = color;
    }
}
