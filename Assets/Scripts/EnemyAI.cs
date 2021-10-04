using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : Singleton<EnemyAI>
{
    const float TARGET_NOISE = 0.8f;

    #region parameters
    [SerializeField]
    public FormationSpawner formationSpawnerEnemy = null;
    [SerializeField]
    public FormationSpawner formationSpawnerRebel = null;
    #endregion

    #region variables
    List<int> enemyTargetRebel = new List<int>();
    List<Rigidbody> enemyRigidbodies = new List<Rigidbody>();

    float m_replanRate = 0.1f;
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

        int deadCount = 0;
        for(int i = 0; i < formationSpawnerRebel.m_spawnedSoldiers.Count; ++i)
        {
            if (formationSpawnerRebel.m_spawnedSoldiers[i].isDead)
                deadCount++;
        }
        if (deadCount == formationSpawnerRebel.m_spawnedSoldiers.Count)
            return;

        int nextRebelIndex = 0;
        while(enemyTargetRebel.Count < formationSpawnerEnemy.totalSoldiersCount)
        {
            if(!formationSpawnerRebel.m_spawnedSoldiers[nextRebelIndex].isDead)
            {
                enemyTargetRebel.Add(nextRebelIndex);
                enemyRigidbodies.Add(formationSpawnerEnemy.m_spawnedSoldiers[enemyTargetRebel.Count - 1].GetComponent<Rigidbody>());
            }

            nextRebelIndex = (nextRebelIndex + 1) % formationSpawnerRebel.totalSoldiersCount;
        }
    }

    void AimAtTargets()
    {
        //for(int i = 0; i < formationSpawnerEnemy.totalSoldiersCount; ++i)
        for(int i = 0; i < enemyTargetRebel.Count; ++i)
        {
            Soldier currentEnemy = formationSpawnerEnemy.m_spawnedSoldiers[i];
            Soldier targetRebel = formationSpawnerRebel.m_spawnedSoldiers[enemyTargetRebel[i]];
            Vector3 noise = Vector3Extensions.RandomSphere(TARGET_NOISE);
            noise.y = currentEnemy.transform.position.y;
            Vector3 perturbedRebelPosition = targetRebel.transform.position + noise;
            Vector3 diffTowards = perturbedRebelPosition - currentEnemy.transform.position;
            Quaternion rotTowards = Quaternion.FromToRotation(currentEnemy.transform.forward, diffTowards.normalized);
            rotTowards.eulerAngles = new Vector3(0.0f, rotTowards.eulerAngles.y, 0.0f);
            enemyRigidbodies[i].MoveRotation(Quaternion.Lerp(currentEnemy.transform.rotation, rotTowards * currentEnemy.transform.rotation, 0.5f));
        }
    }
}
