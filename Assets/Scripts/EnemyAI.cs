using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Singleton<EnemyAI>
{
    const float TARGET_NOISE = 0.1f;

    #region parameters
    [SerializeField]
    public FormationSpawner formationSpawnerEnemy = null;
    [SerializeField]
    public FormationSpawner formationSpawnerRebel = null;
    #endregion

    #region variables
    List<int> enemyTargetRebel = new List<int>();
    List<Rigidbody> enemyRigidbodies = new List<Rigidbody>();

    float m_replanRate = 1.0f;
    float m_replanTimer = 0.0f;
    #endregion

    private void Update()
    {
        if (formationSpawnerEnemy == null || formationSpawnerRebel == null)
            return;

        m_replanTimer += Time.deltaTime;

        if(m_replanTimer > m_replanRate)
        {
            m_replanTimer = 0.0f;
            PairEnemiesToRebels();
            AimAtTargets();
        }
    }

    private void OnDrawGizmos()
    {
        if (formationSpawnerEnemy == null || enemyTargetRebel == null || enemyTargetRebel.Count == 0)
            return;

        for (int i = 0; i < formationSpawnerEnemy.totalSoldiersCount; ++i)
        {
            Soldier currentEnemy = formationSpawnerEnemy.m_spawnedSoldiers[i];
            Soldier targetRebel = formationSpawnerRebel.m_spawnedSoldiers[enemyTargetRebel[i]];
            Gizmos.color = Color.red;
            Gizmos.DrawLine(currentEnemy.transform.position, targetRebel.transform.position);
        }
    }

    void PairEnemiesToRebels()
    {
        enemyTargetRebel = new List<int>();
        enemyRigidbodies = new List<Rigidbody>();

        int nextRebelIndex = 0;
        while(enemyTargetRebel.Count < formationSpawnerEnemy.totalSoldiersCount)
        {
            enemyTargetRebel.Add(nextRebelIndex);
            enemyRigidbodies.Add(formationSpawnerEnemy.m_spawnedSoldiers[enemyTargetRebel.Count - 1].GetComponent<Rigidbody>());

            nextRebelIndex = (nextRebelIndex + 1) % formationSpawnerRebel.totalSoldiersCount;
        }
    }

    void AimAtTargets()
    {
        for(int i = 0; i < formationSpawnerEnemy.totalSoldiersCount; ++i)
        {
            Soldier currentEnemy = formationSpawnerEnemy.m_spawnedSoldiers[i];
            Soldier targetRebel = formationSpawnerRebel.m_spawnedSoldiers[enemyTargetRebel[i]];
            Vector3 diffTowards = targetRebel.transform.position - currentEnemy.transform.position;
            Quaternion rotTowards = Quaternion.FromToRotation(currentEnemy.transform.forward, diffTowards.normalized);
            //enemyRigidbodies[i].AddTorque(currentEnemy.transform.up * 100000.0f * rotTowards.eulerAngles.z, ForceMode.VelocityChange);
            //enemyRigidbodies[i].angularVelocity = -currentEnemy.transform.up * 10000.0f * rotTowards.eulerAngles.y / Time.deltaTime;
            enemyRigidbodies[i].MoveRotation(Quaternion.Lerp(currentEnemy.transform.rotation, rotTowards * currentEnemy.transform.rotation, 0.5f));
        }
    }
}
