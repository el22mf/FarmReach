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
        if (((1 << other.gameObject.layer) & handLayer) == 0)
            return;

        HandToggle hand = other.GetComponent<HandToggle>();
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
        GameObject appleGO = hand.ReleaseApple();
        if (appleGO == null)
            return;

        Apple apple = appleGO.GetComponent<Apple>();
        if (apple == null)
            return;

        // Notify game manager FIRST
        if (gameManager != null)
        {
            gameManager.OnAppleDeposited(apple);
            Debug.Log("Apple deposited");
        }

        // Remove apple from scene
        appleGO.SetActive(false);

        lastDepositTime = Time.time;
    }

}
