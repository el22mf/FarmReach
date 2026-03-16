using UnityEngine;

public class HandToggle : MonoBehaviour
{
    [Header("Grab Detection")]
    public float grabRadius = 0.15f;
    public Vector3 grabOffset = new Vector3(0f, 0f, 0.1f);
    public LayerMask appleLayer;

    [Header("Rotation Intent")]
    public float grabAngleThreshold = 30f;   // degrees of supination (30-60)
    public float grabHoldTime = 0.4f;        // seconds to confirm intent

    [Header("Hand Visuals")]
    public GameObject openHand;
    public GameObject closedHand;

    private float grabHoldTimer = 0f;
    private bool isGrabbing = false;
    private GameObject grabbedApple;

    private Apple currentTargetApple;
    private float currentTilt;
    public float CurrentTilt => currentTilt;

    public bool CanTranslate { get; private set; } = true;

    public ApplePickingAccuracyBar accuracyBar;

    private ApplePickingGameManager gameManager;

    void Start()
    {
        if (openHand != null) openHand.SetActive(true);
        if (closedHand != null) closedHand.SetActive(false);

        gameManager = FindAnyObjectByType<ApplePickingGameManager>();

    }

    void Update()
    {
        // Smooth visual tilt updates every frame
        currentTilt = GetSupinationAngle();

        // Update target apple reference
        //Debug.Log("Hand sees manager: " + (gameManager != null));

        if (gameManager == null || !gameManager.IsActive()) return;
        currentTargetApple = gameManager.GetCurrentTarget();

        if (currentTargetApple == null) return;

        Debug.Log("Hand target: " + (currentTargetApple ? currentTargetApple.name : "NULL"));
    }

    void FixedUpdate()
    {
        if (gameManager == null || !gameManager.IsActive()) return;
        // Physics-aligned grab logic
        if (!isGrabbing)
        {
            bool appleNearby = IsAppleNearby();

            // Lock translation if near apple
            CanTranslate = !appleNearby;
            if (appleNearby)
                SetHandState(true); // closed hand

            bool validRotation = currentTilt >= grabAngleThreshold;

            if (appleNearby && validRotation)
            {
                grabHoldTimer += Time.fixedDeltaTime;

                if (grabHoldTimer >= grabHoldTime)
                {
                    TryGrabApple();

                    // Unlock translation after grabbing
                    CanTranslate = true;
                }
            }
            else
            {
                grabHoldTimer = 0f;
            }
        }
    }

    bool IsAppleNearby()
    {
        if (currentTargetApple == null) return false;

        Collider[] hits = Physics.OverlapSphere(transform.position + grabOffset, grabRadius, appleLayer);

        foreach (Collider c in hits)
        {
            float yDiff = Mathf.Abs(c.transform.position.y - transform.position.y);
            if (yDiff > grabRadius * 0.5f) continue;

            Apple appleComp = c.GetComponentInParent<Apple>();
            if (appleComp == currentTargetApple)
                return true;
        }

        return false;
    }

    void TryGrabApple()
    {
        if (currentTargetApple == null) return;

        Collider[] hits = Physics.OverlapSphere(transform.position + grabOffset, grabRadius, appleLayer);

        foreach (Collider c in hits)
        {
            float yDiff = Mathf.Abs(c.transform.position.y - transform.position.y);
            if (yDiff > grabRadius * 0.5f) continue;

            Apple appleComp = c.GetComponentInParent<Apple>();
            if (appleComp == currentTargetApple)
            {
                GrabApple(appleComp.gameObject);
                break;
            }
        }
    }

    public bool IsHoldingApple() => grabbedApple != null;

    public GameObject ReleaseApple()
    {
        if (grabbedApple == null) return null;

        GameObject apple = grabbedApple;

        Rigidbody rb = apple.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        grabbedApple.transform.SetParent(null);
        grabbedApple = null;
        isGrabbing = false;

        SetHandState(false);

        return apple;
    }

    public void GrabApple(GameObject apple)
    {
        grabbedApple = apple;

        grabbedApple.transform.SetParent(transform);
        grabbedApple.transform.localPosition = Vector3.zero;
        grabbedApple.transform.localRotation = Quaternion.identity;

        Rigidbody rb = grabbedApple.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        isGrabbing = true;
        grabHoldTimer = 0f;

        SetHandState(true);
        CanTranslate = true; // unlock translation after grab
    }

    void SetHandState(bool closed)
    {
        if (openHand != null) openHand.SetActive(!closed);
        if (closedHand != null) closedHand.SetActive(closed);
    }

    public float GetSupinationAngle()
    {
        float z = transform.localEulerAngles.z;
        if (z > 180f) z -= 360f;

        return Mathf.Abs(z);
    }

    public bool IsAtNeutralAngle(float tolerance = 15f)
    {
        return GetSupinationAngle() <= tolerance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + grabOffset, grabRadius);
    }
}
