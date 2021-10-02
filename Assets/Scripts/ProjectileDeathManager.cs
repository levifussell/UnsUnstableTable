using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDeathManager : Singleton<ProjectileDeathManager>
{
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public Action<Vector3, Vector3> onProjectileDeath;

    public void RegisterNewProjectileDeath(Vector3 force, Vector3 position)
    {
        onProjectileDeath?.Invoke(force, position);
    }
}
