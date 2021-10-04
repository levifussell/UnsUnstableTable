using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HudMenu : MonoBehaviour
{
    [SerializeField]
    Image m_iconEnemy = null;
    [SerializeField]
    Image m_iconRebel = null;

    List<Image> m_enemyIcons = new List<Image>();
    List<Image> m_rebelIcons = new List<Image>();
    int m_aliveEnemies;
    int m_aliveRebels;

    public Action<bool> onGameOver = null;

    public void SetupNewLevelHud(GameObject levelObject)
    {
        // Clear old hud.
        foreach(Image im in m_rebelIcons)
        {
            Destroy(im.gameObject);
        }
        m_rebelIcons.Clear();
        foreach(Image im in m_enemyIcons)
        {
            Destroy(im.gameObject);
        }
        m_enemyIcons.Clear();

        // New formations.

        FormationSpawner[] spawners = levelObject.GetComponentsInChildren<FormationSpawner>();

        // rebels.

        for(int i = 0; i < spawners[0].totalSoldiersCount; ++i)
        {
            Image im = Image.Instantiate(m_iconRebel, this.transform);
            RectTransform rect = im.GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(i * 60.0f, 0.0f);
            m_rebelIcons.Add(im);
        }
        spawners[0].onSoldierIndexSpawned -= OnRebelSpawn;
        spawners[0].onSoldierIndexSpawned += OnRebelSpawn;
        m_aliveRebels = spawners[0].totalSoldiersCount;

        // enemies.

        for(int i = 0; i < spawners[1].totalSoldiersCount; ++i)
        {
            Image im = Image.Instantiate(m_iconEnemy, this.transform);
            RectTransform rect = im.GetComponent<RectTransform>();
            rect.anchoredPosition -= new Vector2(i * 60.0f, 0.0f);
            m_enemyIcons.Add(im);
        }
        spawners[1].onSoldierIndexSpawned -= OnEnemySpawn;
        spawners[1].onSoldierIndexSpawned += OnEnemySpawn;
        m_aliveEnemies = spawners[1].totalSoldiersCount;
    }

    void OnSpawnIconPair(List<Image> icons, Soldier soldier, int index, bool isRebel)
    {
        soldier.onIsAlive += (x) => { 
            icons[index].color = Color.white;

            if (isRebel)
                m_aliveRebels++;
            else
                m_aliveEnemies++;
        };
        soldier.onIsDead += (x) => {
            icons[index].color = new Color(1.0f, 1.0f, 1.0f, 0.3f);

            if (isRebel)
            {
                m_aliveRebels--;
                if (m_aliveRebels == 0)
                    onGameOver?.Invoke(false);
            }
            else
            {
                m_aliveEnemies--;
                if (m_aliveEnemies == 0)
                    onGameOver?.Invoke(true);
            }
        };
    }

    void OnRebelSpawn(Soldier soldier, int index)
    {
        OnSpawnIconPair(m_rebelIcons, soldier, index, true);
    }

    void OnEnemySpawn(Soldier soldier, int index)
    {
        OnSpawnIconPair(m_enemyIcons, soldier, index, false);
    }
}
