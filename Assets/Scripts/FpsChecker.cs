using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FpsChecker : MonoBehaviour
{
    [SerializeField]private TMP_Text _text;
    private int i = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        i++;
        if (i % 40 == 0)
        {
            _text.text = (1000 / (Time.deltaTime * 1000)).ToString();
        }
    }
}
