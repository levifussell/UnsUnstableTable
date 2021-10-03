using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingTimer : MonoBehaviour
{
    #region parameters
    [SerializeField] [Range(0.0f, 20.0f)]
    public float m_ringTimer = 1.0f;

    [SerializeField] [Range(0.0f, 1.0f)]
    float m_minRadius = 0.2f;
    [SerializeField] [Range(0.0f, 1.0f)]
    float m_maxRadius = 0.5f;

    [SerializeField] [Range(0.0f, 1.0f)]
    float m_minWidth = 0.02f;
    [SerializeField] [Range(0.0f, 1.0f)]
    float m_maxWidth = 0.1f;

    [SerializeField] [Range(0.0f, 100.0f)]
    float m_updateSpeed = 10.0f;

    [SerializeField]
    Color m_minColor = Color.red;
    [SerializeField]
    Color m_maxColor = Color.gray;

    #endregion

    #region variables
    Material m_ringMaterail;

    float m_currentRadius;
    float m_currentWidth;
    Color m_currentColor;
    #endregion

    #region builtins
    private void Awake()
    {
        m_ringMaterail = this.GetComponent<MeshRenderer>().material;

        m_currentRadius = m_maxRadius;
        m_currentWidth = m_minWidth;
        m_currentColor = m_minColor;
    }

    private void Update()
    {
    }
    #endregion

    #region control
    public void SetTime(float time)
    {
        float timeLerp = Mathf.Pow(time / m_ringTimer, 5.0f);
        float diffRadius = ((m_maxRadius - m_minRadius) * (1.0f - timeLerp) + m_minRadius) - m_currentRadius;
        m_currentRadius += diffRadius * Mathf.Min(m_updateSpeed * Time.deltaTime, 1.0f);
        m_ringMaterail.SetFloat("_RingRadius", m_currentRadius);
        m_ringMaterail.SetFloat("_RingWidth", (m_maxWidth - m_minWidth) * (1.0f - timeLerp) + m_minWidth);
        m_ringMaterail.SetColor("_Color", (m_maxColor - m_minColor) * (1.0f - timeLerp) + m_minColor);

        //m_ringMaterail.SetFloat("_RingWidth", m_currentWidth);
        //m_ringMaterail.SetColor("_Color", m_currentColor);
    }
    #endregion
}
