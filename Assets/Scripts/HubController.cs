using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HubController : MonoBehaviour
{
    #region parameters
    [SerializeField]
    List<float> levelPositions = new List<float>();
    [SerializeField]
    PlayerController player = null;
    #endregion

    #region variables
    Vector3 m_initHubPosition;
    public int currentLevelIndex { get; private set; }

    Vector3 initHubPosition
    {
        get
        {
            if (Application.isPlaying)
                return m_initHubPosition;
            else
                return this.transform.position;
        }
    }

    public int levelCount { get => levelPositions.Count; }

    public Vector3 currentLevelOrigin { get => GetLevel3Position(currentLevelIndex); }
    #endregion

    #region builtins
    private void Awake()
    {
        m_initHubPosition = this.transform.position;

        currentLevelIndex = 0;
    }
    private void Start()
    {
        ProcessNewLevel();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = GetLevel3Position(currentLevelIndex);
        this.transform.position += (targetPos - this.transform.position) * 0.05f;

        //if(Input.GetKeyDown(KeyCode.D))
        //{
        //    NextLevel();
        //}

        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    PreviousLevel();
        //}
    }
    #endregion

    #region control
    Vector3 GetLevel3Position(int index)
    {
        return initHubPosition + new Vector3(
            levelPositions[index],
            0.0f,
            0.0f
            );
    }
    public void NextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levelPositions.Count;
        ProcessNewLevel();
    }
    public void PreviousLevel()
    {
        currentLevelIndex = ((currentLevelIndex - 1) % levelPositions.Count + levelPositions.Count) % levelPositions.Count;
        ProcessNewLevel();
    }
    void ProcessNewLevel()
    {
        player.TABLE_CENTER = GetLevel3Position(currentLevelIndex);
    }
    #endregion

#if UNITY_EDITOR
    #region editor
    public static void DrawScene(HubController hub)
    {
        for(int i = 0; i < hub.levelPositions.Count; ++i)
        {
            hub.levelPositions[i] = (Handles.PositionHandle(hub.GetLevel3Position(i), Quaternion.identity) - hub.initHubPosition).x;
        }
    }
    #endregion
#endif
    
}

#if UNITY_EDITOR
[CustomEditor(typeof(HubController))]
public class E_HubController : Editor
{
    HubController hub;
    private void OnEnable()
    {
        hub = (HubController)target;
    }
    private void OnSceneGUI()
    {
        HubController.DrawScene(hub);
    }
}
#endif
