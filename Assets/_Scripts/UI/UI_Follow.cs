using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Follow : MonoBehaviour
{
    [SerializeField] GameObject followTarget;
    [SerializeField] float height = 4;
    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        if (followTarget != null)
        {
            Vector3 temp = followTarget.transform.position;
            temp.y = height;
            transform.position = temp;
        }
    }
}
