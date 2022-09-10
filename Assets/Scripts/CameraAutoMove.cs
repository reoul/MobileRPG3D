using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAutoMove : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    private void LateUpdate()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, _playerTransform.position + new Vector3(0,10,-15), 0.1f);
    }
}
