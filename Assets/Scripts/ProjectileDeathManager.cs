using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeathManager : Singleton<ProjectileDeathManager>
{
    List<Projectile> m_liveProjectiles = new List<Projectile>();

    public Action<Vector3, Vector3> onProjectileDeath;

    public void RegisterNewProjectile(Projectile projectile)
    {
        m_liveProjectiles.Add(projectile);
    }

    public void RegisterNewProjectileDeath(Projectile projectile, Vector3 force, Vector3 position)
    {
        m_liveProjectiles.Remove(projectile);
        onProjectileDeath?.Invoke(force, position);
    }

    public void DestroyAllProjectiles()
    {
        foreach(Projectile p in m_liveProjectiles)
        {
            p.DestroyWithSmokeTrail();
        }
    }
}
