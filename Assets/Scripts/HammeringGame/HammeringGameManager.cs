//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public class HammeringGameManager : MonoBehaviour, IMinigame
//{
//    [Header("Hammer Setup")]
//    public Transform hammer;                   // Hammer object (rotation input)

//    [Header("Fences & Nails")]
//    public List<Fence> fences = new List<Fence>();  // All fences in the scene
//    private int currentFenceIndex = 0;              // Tracks current fence

//    [Header("Session Settings")]
//    public float sessionDuration = 60f;         // Total game time in seconds

//    [Header("Rotation Settings")]
//    public float rotationTolerance = 10f;       // Degrees tolerance for hit/pullback

//    [Header("Tracking")]
//    private float sessionTimer;
//    private int totalNailsHammered = 0;

//    public int TotalNailsHammered => totalNailsHammered;
//    public float SessionTimeLeft => Mathf.Max(0f, sessionTimer);
//    public bool IsSessionComplete => sessionTimer <= 0f;

//    void Start()
//    {
//        sessionTimer = sessionDuration;
//        ActivateCurrentFence();
//    }

//    void LateUpdate()
//    {
//        if (sessionTimer <= 0f) return;

//        // Countdown timer
//        sessionTimer -= Time.deltaTime;

//        TrackHammerHits();

//        if (sessionTimer <= 0f)
//        {
//            CompleteSession();
//        }
//    }

//    void TrackHammerHits()
//    {
//        Fence currentFence = fences[currentFenceIndex];

//        foreach (Nail nail in currentFence.nails)
//        {
//            if (!nail.IsHammered)
//            {
//                bool hitRegistered = nail.TryHit(hammer.rotation);
//                if (hitRegistered && nail.IsHammered)
//                {
//                    totalNailsHammered++;
//                }
//            }
//        }

//        if (currentFence.IsComplete && currentFenceIndex < fences.Count - 1)
//        {
//            currentFenceIndex++;
//            ActivateCurrentFence();
//        }
//    }

//    void ActivateCurrentFence()
//    {
//        for (int i = 0; i < fences.Count; i++)
//        {
//            fences[i].gameObject.SetActive(i == currentFenceIndex);
//        }
//    }

//    void CompleteSession()
//    {
//        Debug.Log($"Hammering Session Complete! Total nails hammered: {totalNailsHammered}");
//        // Optional: trigger scoring, UI, or callbacks here
//    }

//    public void ResetGame()
//    {
//        Debug.Log("Resetting Hammering Game");

//        sessionTimer = sessionDuration;
//        totalNailsHammered = 0;
//        currentFenceIndex = 0;

//        // Reset nails
//        foreach (Fence fence in fences)
//        {
//            foreach (Nail nail in fence.nails)
//            {
//                nail.state = Nail.NailState.Untouched;
//                if (nail.nailVisual != null)
//                {
//                    nail.nailVisual.transform.localPosition = Vector3.zero; // reset depth
//                }
//            }
//        }

//        ActivateCurrentFence();
//    }

//    public bool IsActive()
//    {
//        return gameObject.activeInHierarchy && sessionTimer > 0f;
//    }
//}