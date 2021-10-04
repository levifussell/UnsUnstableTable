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

    public void SetupNewLevelHud(GameObject levelObject)
    {
        FormationSpawner[] spawners = levelObject.GetComponentsInChildren<FormationSpawner>();

        // rebels.

        for(int i = 0; i < spawners[0].totalSoldiersCount; ++i)
        {
            Image im = Image.Instantiate(m_iconRebel, this.transform);
            RectTransform rect = im.GetComponent<RectTransform>();
            rect.anchoredPosition += new Vector2(i * 60.0f, 0.0f);
            m_rebelIcons.Add(im);
        }
        spawners[0].onSoldierIndexSpawned += OnRebelSpawn;

        // enemies.

        for(int i = 0; i < spawners[1].totalSoldiersCount; ++i)
        {
            Image im = Image.Instantiate(m_iconEnemy, this.transform);
            RectTransform rect = im.GetComponent<RectTransform>();
            rect.anchoredPosition -= new Vector2(i * 60.0f, 0.0f);
            m_enemyIcons.Add(im);
        }
        spawners[1].onSoldierIndexSpawned += OnEnemySpawn;
    }

    void OnSpawnIconPair(List<Image> icons, Soldier soldier, int index)
    {
        soldier.onIsAlive += (x) => icons[index].color = Color.white;
        soldier.onIsDead += (x) => icons[index].color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
    }

    void OnRebelSpawn(Soldier soldier, int index)
    {
        OnSpawnIconPair(m_rebelIcons, soldier, index);
    }

    void OnEnemySpawn(Soldier soldier, int index)
    {
        OnSpawnIconPair(m_enemyIcons, soldier, index);
    }
}
