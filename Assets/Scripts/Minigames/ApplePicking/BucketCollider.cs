using UnityEngine;

public class BucketCollider : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask handLayer;
    public float depositCooldown = 0.3f;

    private float lastDepositTime = -1f;
    private ApplePickingGameManager gameManager;

    void Start()
    {
        gameManager = FindAnyObjectByType<ApplePickingGameManager>();
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("Bucket hit by: " + other.name);
        Debug.Log("Layer match: " + ((((1 << other.gameObject.layer) & handLayer) != 0)));
        Debug.Log("Has HandToggle: " + (other.GetComponent<HandToggle>() != null));

        HandToggle hand = other.GetComponent<HandToggle>();
        if (hand != null)
        {
            Debug.Log("IsHoldingApple: " + hand.IsHoldingApple());
            Debug.Log("NeutralAngle: " + hand.IsAtNeutralAngle(15f));
        }

        if (((1 << other.gameObject.layer) & handLayer) == 0)
            return;

        if (hand == null)
            return;

        if (!hand.IsHoldingApple())
            return;

        if (!hand.IsAtNeutralAngle(15f))
            return;

        if (Time.time - lastDepositTime < depositCooldown)
            return;

        DepositApple(hand);
    }


    void DepositApple(HandToggle hand)
    {
        // Force the apple to be released here (not earlier)
        GameObject appleGO = hand.ReleaseApple();
        if (appleGO == null)
            return;

        // Now safely get the Apple component
        Apple apple = appleGO.GetComponentInParent<Apple>();
        if (apple == null)
            return;

        // Notify game manager FIRST
        if (gameManager != null)
        {
            gameManager.OnAppleDeposited(apple);
            Debug.Log("Apple deposited");
        }

        // Remove apple from scene
        appleGO.transform.SetParent(null);

        Rigidbody rb = appleGO.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        appleGO.SetActive(false);

        lastDepositTime = Time.time;
    }


}
