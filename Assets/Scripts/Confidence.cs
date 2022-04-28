using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Confidence : MonoBehaviour
{
    [SerializeField]
    private float confidence; //Level of confidence between 0.0 and 1.0

    public void SetConfidence(float _confidence) { confidence = _confidence; }
    public float GetConfidence() { return confidence; }

    private void Update()
    {
        confidence = Mathf.Clamp(confidence, 0.0f, 1.0f);
    }

    public void IncreaseCon(float amount)
    {
        confidence += amount;
    }

    public void DecreaseCon(float amount)
    {
        confidence -= amount;
    }
}
