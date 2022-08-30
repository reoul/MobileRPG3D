using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoMove : MonoBehaviour
{
    [SerializeField] private Transform _cameraPoint;
    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, _cameraPoint.position, 0.1f);
    }
}
