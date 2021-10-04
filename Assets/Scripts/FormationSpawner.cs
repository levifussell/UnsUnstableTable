using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FormationSpawner : MonoBehaviour
{
    public static float GROUND_HEIGHT = 0.0f;
    public static float GUN_COLOR_MUL = 0.2f;
    public static float SHIELD_COLOR_MUL = 0.5f;
    public static float FLAG_COLOR_MUL = 1.0f;

    #region parameters
    [SerializeField]
    Soldier soldierGunPrefab = null;
    [SerializeField]
    Soldier soldierShieldPrefab = null;
    [SerializeField]
    Soldier soldierFlagPrefab = null;

    [SerializeField]
    public List<Vector3> soldierGunSpawnPoints = new List<Vector3>();
    [SerializeField]
    public List<Quaternion> soldierGunSpawnRotations = new List<Quaternion>();

    [SerializeField]
    List<Vector3> soldierShieldSpawnPoints = new List<Vector3>();
    [SerializeField]
    List<Quaternion> soldierShieldSpawnRotations = new List<Quaternion>();

    [SerializeField]
    List<Vector3> soldierFlagSpawnPoints = new List<Vector3>();
    [SerializeField]
    List<Quaternion> soldierFlagSpawnRotations = new List<Quaternion>();

    [SerializeField]
    bool isPlayer = true;
    #endregion

    #region variables
    int m_layer;
    public List<Soldier> m_spawnedSoldiers = new List<Soldier>();

    public Action<Soldier, int> onSoldierIndexSpawned = null;

    public int totalSoldiersCount { get => soldierGunSpawnPoints.Count + soldierShieldSpawnPoints.Count + soldierFlagSpawnPoints.Count; }
    #endregion

    #region builtin
    private void Awake()
    {
        m_layer = LayerMask.NameToLayer(isPlayer ? "Player1" : "Player2");
    }
    private void Start()
    {
        SpawnFormation();
    }
    private void OnDestroy()
    {
        UnspawnAndDestroyAllSoldiers();
    }
    #endregion

    #region control
    void SpawnFormation()
    {
        int soldierIndex = 0;

        // Gun soldiers.

        for(int i = 0; i < soldierGunSpawnPoints.Count; ++i)
        {
            Soldier s = GameObject.Instantiate(soldierGunPrefab);
            PostprocessSoldier(s, soldierGunSpawnPoints[i], soldierGunSpawnRotations[i]);
            onSoldierIndexSpawned?.Invoke(s, soldierIndex++);
        }

        // Shield soldiers.

        for(int i = 0; i < soldierShieldSpawnPoints.Count; ++i)
        {
            Soldier s = GameObject.Instantiate(soldierShieldPrefab);
            PostprocessSoldier(s, soldierShieldSpawnPoints[i], soldierShieldSpawnRotations[i]);
            onSoldierIndexSpawned?.Invoke(s, soldierIndex++);
        }

        // Flag soldiers.

        for(int i = 0; i < soldierFlagSpawnPoints.Count; ++i)
        {
            Soldier s = GameObject.Instantiate(soldierFlagPrefab);
            PostprocessSoldier(s, soldierFlagSpawnPoints[i], soldierFlagSpawnRotations[i]);
            onSoldierIndexSpawned?.Invoke(s, soldierIndex++);
        }

    }

    void PostprocessSoldier(Soldier s, Vector3 spawnPoint, Quaternion spawnRot)
    {
        spawnPoint.y = GROUND_HEIGHT;
        s.transform.position = this.transform.TransformPoint(spawnPoint);
        s.transform.rotation = spawnRot;
        s.gameObject.layer = m_layer;
        CorrectVerticalSoldier(s);
        m_spawnedSoldiers.Add(s);
    }

    void CorrectVerticalSoldier(Soldier s)
    {
        Mesh[] meshes = s.GetComponentsInChildren<MeshFilter>().Select(x => x.mesh).ToArray();
        Bounds allBounds = meshes[0].bounds;
        for(int i = 1; i < meshes.Length; ++i)
        {
            allBounds.Encapsulate(meshes[i].bounds);
        }

        s.transform.position += new Vector3(0.0f, allBounds.extents.y, 0.0f);
    }

    private void OnDrawGizmos()
    {
        Color mainColor = Color.blue;
        if (!isPlayer)
            mainColor = Color.red;

        Gizmos.color = NoAlphaMul(mainColor, GUN_COLOR_MUL);
        for(int i = 0; i < this.soldierGunSpawnPoints.Count; ++i)
        {
            Gizmos.DrawSphere(this.transform.TransformPoint(this.soldierGunSpawnPoints[i]), 0.1f);
        }

        Gizmos.color = NoAlphaMul(mainColor, SHIELD_COLOR_MUL);
        for(int i = 0; i < this.soldierShieldSpawnPoints.Count; ++i)
        {
            Gizmos.DrawCube(this.transform.TransformPoint(this.soldierShieldSpawnPoints[i]), Vector3.one * 0.2f);
        }

        Gizmos.color = NoAlphaMul(mainColor, FLAG_COLOR_MUL);
        for(int i = 0; i < this.soldierFlagSpawnPoints.Count; ++i)
        {
            Gizmos.DrawSphere(this.transform.TransformPoint(this.soldierFlagSpawnPoints[i]), 0.1f);
        }
    }

    Color NoAlphaMul(Color c, float m)
    {
        Color.RGBToHSV(c, out float h, out float s, out float v);
        return Color.HSVToRGB(h, s * m, v * m);
    }

    public void UnspawnAndDestroyAllSoldiers()
    {
        foreach(Soldier soldier in m_spawnedSoldiers)
        {
            if(soldier != null)
                Destroy(soldier.gameObject);
        }

        m_spawnedSoldiers.Clear();
    }
    #endregion

#if UNITY_EDITOR
    #region editor
    public static void DrawInspector(FormationSpawner fs)
    {

    }
    public static void DrawScene(FormationSpawner fs)
    {
        for(int i = 0; i < fs.soldierGunSpawnPoints.Count; ++i)
        {
            EditorGUI.BeginChangeCheck();
            if (fs.soldierGunSpawnRotations[i].x == 0 && fs.soldierGunSpawnRotations[i].y == 0 && fs.soldierGunSpawnRotations[i].z == 0 && fs.soldierGunSpawnRotations[i].w == 0)
                fs.soldierGunSpawnRotations[i] = Quaternion.identity;
            Vector3 newPoint = Handles.PositionHandle(fs.transform.TransformPoint(fs.soldierGunSpawnPoints[i]), fs.soldierGunSpawnRotations[i]);
            newPoint = fs.transform.InverseTransformPoint(new Vector3(newPoint.x, GROUND_HEIGHT, newPoint.z));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn point.");
                fs.soldierGunSpawnPoints[i] = newPoint;
            }
        }

        for(int i = 0; i < fs.soldierGunSpawnRotations.Count; ++i)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion newRot = Handles.RotationHandle(fs.soldierGunSpawnRotations[i], fs.transform.TransformPoint(fs.soldierGunSpawnPoints[i]));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn rotation.");
                fs.soldierGunSpawnRotations[i] = newRot;
            }
        }

        for(int i = 0; i < fs.soldierShieldSpawnPoints.Count; ++i)
        {
            if (fs.soldierShieldSpawnRotations[i].x == 0 && fs.soldierShieldSpawnRotations[i].y == 0 && fs.soldierShieldSpawnRotations[i].z == 0 && fs.soldierShieldSpawnRotations[i].w == 0)
                fs.soldierShieldSpawnRotations[i] = Quaternion.identity;
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.PositionHandle(fs.transform.TransformPoint(fs.soldierShieldSpawnPoints[i]), fs.soldierShieldSpawnRotations[i]);
            newPoint = fs.transform.InverseTransformPoint(new Vector3(newPoint.x, GROUND_HEIGHT, newPoint.z));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn point.");
                fs.soldierShieldSpawnPoints[i] = newPoint;
            }
        }

        for(int i = 0; i < fs.soldierShieldSpawnRotations.Count; ++i)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion newRot = Handles.RotationHandle(fs.soldierShieldSpawnRotations[i], fs.transform.TransformPoint(fs.soldierShieldSpawnPoints[i]));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn rotation.");
                fs.soldierShieldSpawnRotations[i] = newRot;
            }
        }

        for(int i = 0; i < fs.soldierFlagSpawnPoints.Count; ++i)
        {
            if (fs.soldierFlagSpawnRotations[i].x == 0 && fs.soldierFlagSpawnRotations[i].y == 0 && fs.soldierFlagSpawnRotations[i].z == 0 && fs.soldierFlagSpawnRotations[i].w == 0)
                fs.soldierFlagSpawnRotations[i] = Quaternion.identity;
            EditorGUI.BeginChangeCheck();
            Vector3 newPoint = Handles.PositionHandle(fs.transform.TransformPoint(fs.soldierFlagSpawnPoints[i]), fs.soldierFlagSpawnRotations[i]);
            newPoint = fs.transform.InverseTransformPoint(new Vector3(newPoint.x, GROUND_HEIGHT, newPoint.z));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn point.");
                fs.soldierFlagSpawnPoints[i] = newPoint;
            }
        }

        for(int i = 0; i < fs.soldierFlagSpawnRotations.Count; ++i)
        {
            EditorGUI.BeginChangeCheck();
            Quaternion newRot = Handles.RotationHandle(fs.soldierFlagSpawnRotations[i], fs.transform.TransformPoint(fs.soldierFlagSpawnPoints[i]));
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(fs, "change spawn rotation.");
                fs.soldierFlagSpawnRotations[i] = newRot;
            }
        }

    }
    #endregion
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(FormationSpawner))]
public class E_FormationSpawner : Editor
{
    FormationSpawner formationSpawner;
    private void OnEnable()
    {
        formationSpawner = (FormationSpawner)target; 
    }
    private void OnSceneGUI()
    {
        FormationSpawner.DrawScene(formationSpawner);
    }
}
#endif
