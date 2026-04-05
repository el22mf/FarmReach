using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WateringCan : MonoBehaviour
{
    public ParticleSystem waterParticles;

    public float pourThreshold = 10f; // how far to tilt before pouring
    public float maxTilt = 120f;
    public float currentTilt;

    public float baseWaterRate = 2f; // max water per second at full tilt

    public Transform spoutPoint;
    public float wateringRange = 5f;

    private bool isPouring = false;
    private float currentPourStrength = 0f;

    public CropWateringAccuracyBar accuracyBar;

    void Update()
    {
        currentTilt = Vector3.SignedAngle(Vector3.up, transform.up, transform.right);

        if (accuracyBar != null)
        {
            accuracyBar.UpdateIndicator(currentTilt);
        }


        if (currentTilt > pourThreshold)
        {

            currentPourStrength = Mathf.InverseLerp(pourThreshold, maxTilt, currentTilt);
            StartPouring(currentPourStrength);
        }
        else
        {
            currentPourStrength = 0f;
            StopPouring();
        }

        if (isPouring)
        {
            DoWaterRaycast();
        }

    }

    void StartPouring(float strength)
    {
        if (!isPouring)
        {
            waterParticles.Play();
            isPouring = true;
        }
        var emission = waterParticles.emission;
        emission.rateOverTime = Mathf.Lerp(10f, 150f, strength); // adjust min and max for visual pouring
    }

    void StopPouring()
    {
        if (isPouring)
        {
            waterParticles.Stop();
            isPouring = false;
        }
    }

    void DoWaterRaycast()
    {
        Debug.DrawRay(spoutPoint.position, spoutPoint.forward * wateringRange, Color.yellow);

        int layerMask = LayerMask.GetMask("Cabbage");

        RaycastHit hit;

        if (Physics.Raycast(spoutPoint.position, spoutPoint.forward, out hit, wateringRange, layerMask))
        {
            //Debug.Log("Ray hit: " + hit.collider.name);
            GrowCabbage cabbage = hit.collider.GetComponent<GrowCabbage>();

            if (cabbage != null)
            {
                float accuracy = 1f; // default to full
                CropWateringGameManager cropGameManager = FindAnyObjectByType<CropWateringGameManager>();
                if (cropGameManager != null)
                {
                    accuracy = cropGameManager.GetAccuracy(currentTilt);
                }

                // Apply water rate scaled by both tilt and accuracy
                float effectiveWater = baseWaterRate * currentPourStrength * accuracy;

                cabbage.AddWater(effectiveWater * Time.deltaTime);
                //Debug.Log("Growing cabbage: " + hit.collider.name);

            }
            else
            {
                //Debug.Log("Hit object " + hit.collider.name + " but no GrowCabbage");
            }
        }
        else
        {
            //Debug.Log("Ray hit nothing");
        }
    }
}