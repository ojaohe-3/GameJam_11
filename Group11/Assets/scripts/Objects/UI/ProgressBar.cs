using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] public float _max = 100f;
    [SerializeField] public float _current = 100f;
    [SerializeField] private Image mask;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private GradientColorKey[] _colorKey;
    [SerializeField] private GradientAlphaKey[] _alphaKey;

    private void Start()
    {
        _gradient = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        _colorKey = new GradientColorKey[2];
        _colorKey[0].color = Color.red;
        _colorKey[0].time = 0.0f;
        _colorKey[1].color = Color.green;
        _colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        _alphaKey = new GradientAlphaKey[2];
        _alphaKey[0].alpha = 1.0f;
        _alphaKey[0].time = 0.0f;
        _alphaKey[1].alpha = 1.0f;
        _alphaKey[1].time = 1.0f;
        _gradient.SetKeys(_colorKey, _alphaKey);
    }

    void Update()
    {
        float fill = _current / _max;
        mask.fillAmount = fill;
        mask.color = _gradient.Evaluate(fill);

    }


}
