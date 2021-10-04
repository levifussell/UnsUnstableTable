using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Train : MonoBehaviour
{
    #region parameters
    [SerializeField]
    GameObject trainController = null;
    [SerializeField]
    GameObject trainFront = null;
    [SerializeField]
    GameObject trainBack = null;
    [SerializeField] [Range(0.1f, 10.0f)]
    float trainSpeed = 2.0f;


    [SerializeField]
    List<Vector3> m_trainStops = new List<Vector3>();
    [SerializeField]
    List<float> m_trainStopLengthsSeconds = new List<float>();
    #endregion

    #region variables
    Vector3 initPosTrainController;
    Quaternion initRotTrainController;
    Vector3 initPosTrainFront;
    Quaternion initRotTrainFront;
    Vector3 initPosTrainBack;
    Quaternion initRotTrainBack;

    Vector3 targetControlPosition;
    #endregion

    #region builtins
    private void Awake()
    {
        initPosTrainController = trainController.transform.position;
        initPosTrainFront = trainFront.transform.position;
        initPosTrainBack = trainBack.transform.position;

        initRotTrainController = trainController.transform.rotation;
        initRotTrainFront = trainFront.transform.rotation;
        initRotTrainBack = trainBack.transform.rotation;

        targetControlPosition = initPosTrainController;
    }

    private void Start()
    {
        StartCoroutine(DoTrainStop(0));
    }
    private void Update()
    {
        Vector3 difToTarget = targetControlPosition - trainController.transform.position;
        trainController.transform.position += Mathf.Min(difToTarget.magnitude, Time.deltaTime * trainSpeed) * difToTarget.normalized;
    }
    #endregion

    #region controls
    void ResetTrain()
    {
        trainController.transform.position = initPosTrainController;
        trainFront.transform.position = initPosTrainFront;
        trainBack.transform.position = initPosTrainBack;
        targetControlPosition = initPosTrainController;

        trainController.transform.rotation = initRotTrainController;
        trainFront.transform.rotation = initRotTrainFront;
        trainBack.transform.rotation = initRotTrainBack;
    }
    IEnumerator DoTrainStop(int stopIndex)
    {
        if(stopIndex == 0)
        {
            ResetTrain();
            yield return new WaitForSeconds(1.0f);
        }
        else
        {
            targetControlPosition = this.transform.TransformPoint(m_trainStops[stopIndex - 1]);
            yield return new WaitForSeconds(m_trainStopLengthsSeconds[stopIndex - 1]);
        }

        StartCoroutine(DoTrainStop((stopIndex + 1) % (m_trainStops.Count + 1)));
    }
    #endregion

#if UNITY_EDITOR
    #region editor
    public static void DrawScene(Train train)
    {
        for(int i = 0; i < train.m_trainStops.Count; ++i)
        {
            train.m_trainStops[i] = train.transform.InverseTransformPoint(Handles.PositionHandle(train.transform.TransformPoint(train.m_trainStops[i]), Quaternion.identity));
            train.m_trainStops[i] = Vector3.Project(train.m_trainStops[i], Vector3.right);
        }
    }
    #endregion
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Train))]
public class E_Train : Editor
{
    Train train;
    private void OnEnable()
    {
        train = (Train)target;
    }
    private void OnSceneGUI()
    {
        Train.DrawScene(train);
    }
}
#endif
