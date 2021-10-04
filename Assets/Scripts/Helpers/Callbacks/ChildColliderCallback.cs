using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildColliderCallback : MonoBehaviour
{
    public delegate void OnCollisionCallback(Collision other, GameObject child);
    public delegate void OnTriggerCallback(Collider other, GameObject child);

    public OnCollisionCallback onCollisionEnterCallback = null;
    public OnCollisionCallback onCollisionExitCallback = null;
    public OnTriggerCallback onTriggerEnterCallback = null;
    public OnTriggerCallback onTriggerExitCallback = null;

    /// <summary>
    /// If true, will only callback triggers based on new GameObject collisions,
    ///   not based on new collider collisions. False will respond for each collider.
    /// </summary>
    public bool onlyTriggerOnUniqueObjects = false;
    //private List<GameObject> activeTriggerCollisions = new List<GameObject>();
    private Dictionary<GameObject, int> activeTriggerCollisions = new Dictionary<GameObject, int>();

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnterCallback?.Invoke(collision, this.gameObject);
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionExitCallback?.Invoke(collision, this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(onlyTriggerOnUniqueObjects)
        {
            GameObject topObject = other.gameObject.GetTopmostParent();
            if (activeTriggerCollisions.ContainsKey(topObject))
            {
                activeTriggerCollisions[topObject]++;
                return;
            }
            else
            {
                activeTriggerCollisions.Add(topObject, 1);
            }
        }

        onTriggerEnterCallback?.Invoke(other, this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if(onlyTriggerOnUniqueObjects)
        {
            GameObject topObject = other.gameObject.GetTopmostParent();
            if (activeTriggerCollisions.ContainsKey(topObject))
            {
                if (activeTriggerCollisions[topObject] == 1)
                    activeTriggerCollisions.Remove(topObject);
                else
                {
                    activeTriggerCollisions[topObject]--;
                    return;
                }
            }
            else
                return;
        }

        onTriggerExitCallback?.Invoke(other, this.gameObject);
    }
}
