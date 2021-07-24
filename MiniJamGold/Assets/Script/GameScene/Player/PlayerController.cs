using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundInformation
{
    public bool mWalkwableSurface = false;
    public Vector3 mSurfaceNormal = Vector3.up;
    public float mSurfaceAngle = 0f;
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float mMovementSpeed = 1f;
    [SerializeField] private float mSlideSpeed = 1f;
    [SerializeField] private float mMaxSlopeAngle = 45f;
    [SerializeField] private float mJumpForce = 1f;
    private bool mJump = false;
    [SerializeField] private bool mGrounded = false;

    [SerializeField] private Rigidbody mRigidbody = null;
    [SerializeField] private LayerMask mGroundLayerMask = new LayerMask();

    private Vector3 mTargetMovement = Vector3.zero;
    private GroundInformation mCurrentGroundInformation = new GroundInformation();

    // Start is called before the first frame update
    void Start()
    {
        mRigidbody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mCurrentGroundInformation = GetGroundInformation();
        mGrounded = IsGrounded();

        mTargetMovement = GetTargetMovementVector();
        if (!mGrounded && mCurrentGroundInformation.mWalkwableSurface) mTargetMovement = Vector3.down;

        mRigidbody.useGravity = !mGrounded;
        if (mCurrentGroundInformation.mWalkwableSurface)
        {
            if (mTargetMovement != Vector3.zero) CalculateTargetMovementOnSurface();
            if (mGrounded && Input.GetAxisRaw("Jump") > 0) mJump = true;
        }

        if (mTargetMovement != Vector3.zero) transform.forward = new Vector3(mTargetMovement.x, 0, mTargetMovement.z);
    }

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void ManageMovement()
    {
        Vector3 newTargetMovement = mTargetMovement.normalized * mMovementSpeed;

        if (mGrounded) mRigidbody.velocity = newTargetMovement;
        else
        {
            if (mCurrentGroundInformation.mWalkwableSurface)
            {
                mRigidbody.velocity = mTargetMovement.normalized * mSlideSpeed;
            }
            else
            {
                float velocityY = mRigidbody.velocity.y;
                mRigidbody.velocity = new Vector3(newTargetMovement.x, velocityY, newTargetMovement.z);
            }
        }

        if (mJump)
        {
            mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, 0, mRigidbody.velocity.z);
            mRigidbody.AddForce(Vector3.up * mJumpForce, ForceMode.Impulse);
            mJump = false;
        }
    }

    public void CalculateTargetMovementOnSurface()
    {
        Vector3 newTargetMovementVector = Vector3.ProjectOnPlane(mTargetMovement, mCurrentGroundInformation.mSurfaceNormal);
        mTargetMovement = newTargetMovementVector.normalized;
    }

    public Vector3 GetTargetMovementVector()
    {
        Vector3 targetMovement = Vector3.forward * Input.GetAxisRaw("Vertical") + Vector3.right * Input.GetAxisRaw("Horizontal");
        return targetMovement.normalized;
    }

    private bool IsGrounded()
    {
        // No object for player to walk on -Not grounded
        if (!mCurrentGroundInformation.mWalkwableSurface) return false;

        // Walkable surface too steep -Not grounded
        if (mCurrentGroundInformation.mSurfaceAngle > mMaxSlopeAngle) return false;

        return true;
    }

    public GroundInformation GetGroundInformation()
    {
        GroundInformation newGroundInformation = new GroundInformation();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 0.49f, Vector3.down, 0.575f, mGroundLayerMask);

        bool walkableSurface = false;
        if (hits.Length > 0) walkableSurface = true;

        float bestAngle = float.MinValue;
        RaycastHit bestHit = new RaycastHit();
        foreach (RaycastHit hit in hits)
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > bestAngle)
            {
                bestHit = hit;
                bestAngle = angle;
            }
        }

        // No walkable surfaces -Return default
        if (!walkableSurface) return newGroundInformation;

        newGroundInformation.mWalkwableSurface = walkableSurface;
        newGroundInformation.mSurfaceNormal = bestHit.normal;
        newGroundInformation.mSurfaceAngle = bestAngle;

        return newGroundInformation;
    }
}
