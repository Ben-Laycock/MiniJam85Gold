using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraTargetPosition
{
    public GameObject mTargetView = null;
    public GameObject mTargetPosition = null;
}

public class CameraController : MonoBehaviour
{
    private static CameraController sInstance;

    public static CameraController Instance { get { return sInstance; } }


    private void Awake()
    {
        if (sInstance != null && sInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sInstance = this;
        }
    }


    [SerializeField] private List<CameraTargetPosition> mTargetPositions = new List<CameraTargetPosition>();

    [SerializeField] private float mMovementSpeed = 1f;
    [SerializeField] private float mAccuracyThreshold = 1f;

    private CameraTargetPosition mIdealTarget = null;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ManageTarget();

        if (Input.GetKeyDown(KeyCode.N)) FindTargetPosition();
    }

    void ManageTarget()
    {
        if (mIdealTarget == null) return;

        transform.LookAt(mIdealTarget.mTargetView.transform);

        if (Vector3.Distance(transform.position, mIdealTarget.mTargetPosition.transform.position) > mAccuracyThreshold)
        {
            Vector3 directionToTarget = mIdealTarget.mTargetPosition.transform.position - transform.position;
            transform.position += directionToTarget.normalized * mMovementSpeed * Time.deltaTime;
        }
    }

    public void ForceTargetChange(CameraTargetPosition argTarget)
    {
        mIdealTarget = argTarget;
    }

    public void FindTargetPosition()
    {
        float distanceToIdealTarget = float.MaxValue;
        foreach(CameraTargetPosition targetPos in mTargetPositions)
        {
            float distanceToTarget = Vector3.Distance(targetPos.mTargetPosition.transform.position, transform.position);
            if (distanceToTarget > distanceToIdealTarget) continue;

            distanceToIdealTarget = distanceToTarget;
            mIdealTarget = targetPos;
        }
    }
}
