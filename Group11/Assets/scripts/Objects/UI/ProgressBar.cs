using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] public float _max = 100f;
    [SerializeField] public float _current = 100f;
    [SerializeField] private Image mask;

    void Update()
    {
        float fill = _current / _max;
        mask.fillAmount = fill;
    }


}
