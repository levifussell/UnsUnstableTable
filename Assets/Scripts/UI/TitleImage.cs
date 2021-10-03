using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TitleImage : MonoBehaviour
{
    Image image;
    float minZRot = -2.0f;
    float maxZRot = 2.0f;
    float rotSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        image = this.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        image.transform.rotation = Quaternion.Euler(
            0.0f,
            0.0f,
            minZRot + (maxZRot - minZRot) * Mathf.Sin(Time.time * rotSpeed)
            );
    }
}
