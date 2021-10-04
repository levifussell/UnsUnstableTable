using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class _ProjectileDeathManager : MonoBehaviour
{
    List<Projectile> m_liveProjectiles = new List<Projectile>();

    public Action<Vector3, Vector3, bool> onProjectileDeath;

    public void RegisterNewProjectile(Projectile projectile)
    {
        m_liveProjectiles.Add(projectile);
    }

    public void RegisterNewProjectileDeath(Projectile projectile, Vector3 force, Vector3 position, bool hitSoldier)
    {
        m_liveProjectiles.Remove(projectile);
        onProjectileDeath?.Invoke(force, position, hitSoldier);
    }

    public void DestroyAllProjectiles()
    {
        foreach(Projectile p in m_liveProjectiles)
        {
            p.DestroyWithSmokeTrail();
        }
    }
}

public class ProjectileDeathManager : Singleton<_ProjectileDeathManager>
{

}
