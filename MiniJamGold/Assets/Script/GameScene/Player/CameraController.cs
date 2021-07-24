using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetFocusDirection
{
    eFront,
    eRight,
    eBack,
    eLeft,
    eUp,
    eDown
}

public static class TargetFocusDirections
{
    public static Vector3[] mDirections = new Vector3[6] {
            Vector3.forward,
            Vector3.right,
            Vector3.back,
            Vector3.left,
            Vector3.up,
            Vector3.down,
        };
}

public class CameraController : MonoBehaviour
{

    [SerializeField] private GameObject mTarget = null;
    [SerializeField] private float mTargetRange = 10f;
    [SerializeField] private float mTargetYOffset = 5f;
    [SerializeField] private TargetFocusDirection mFocusDirection = TargetFocusDirection.eFront;

    private Queue<Vector3> mTargetPositions = new Queue<Vector3>();
    private Vector3 mTargetCameraPosition = Vector3.zero;

    [SerializeField] private float mMovementSpeed = 1f;
    [SerializeField] private float mAccuracyThreshold = 1f;

    // Start is called before the first frame update
    void Start()
    {
        CalculateNextTargetPosition(mTarget);
        PopNextTargetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (mTarget != null) FocusTarget();

        Vector3 directionToTarget = mTargetCameraPosition - transform.position;
        transform.position += directionToTarget.normalized * mMovementSpeed * Time.deltaTime;

        if (Vector3.Distance(transform.position, mTargetCameraPosition) < mAccuracyThreshold)
        {
            if (mTargetPositions.Count > 0)
            {
                PopNextTargetPosition();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.N))
        {
            CalculateNextTargetPosition(mTarget);
        }
    }

    void FocusTarget()
    {
        transform.LookAt(mTarget.transform, Vector3.up);
    }

    void PopNextTargetPosition()
    {
        if (mTargetPositions.Count <= 0) return;
        mTargetCameraPosition = mTargetPositions.Dequeue();
    }

    public void CalculateNextTargetPosition(GameObject argTargetObject)
    {
        Vector3 nextTargetPosition = GetTargetCameraPosition(argTargetObject);

        mTargetPositions.Enqueue(nextTargetPosition);
    }

    Vector3 GetTargetCameraPosition(GameObject argTargetObject)
    {
        Vector3 targetPosition = Vector3.zero;

        switch (mFocusDirection)
        {
            case TargetFocusDirection.eRight:
                targetPosition = argTargetObject.transform.position + argTargetObject.transform.right * mTargetRange + argTargetObject.transform.up * mTargetYOffset;
                break;

            default:
                break;
        }

        return targetPosition;
    }
}
