using System.Collections;
using UnityEngine;

public class OrbitController : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float radius;
    [SerializeField] private float radiusLerpSpeed = 0.1f;
    [SerializeField] private float angularSpeed = 60f;
    [SerializeField] private float heightOffset = 0.3f;
    [SerializeField] private float heightSpeed = 0.2f;
    [SerializeField] private float heightInitOffset = -0.3f;
    [SerializeField] private Transform body;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private GameObject targetToFollow;
    [SerializeField] private int noOfRounds = 2;
    [SerializeField] private AudioSource audioSource;

    Vector3 currHorizontalDir;
    Vector3 initPos;
    Vector3 initRot;
    Vector3 prevPos;
    Quaternion prevRot;
    float currRadius;
    float currHeightOffset;
    float totalAngle = 0f;
    float curr2DDistToCenter;
    bool startOrbit = false;
    bool isTeleport = false;

    enum LandingMode
    {
        FlyBack = 0,
        PitchAdjustment,
        YawAdjustment,
        End,
    };
    LandingMode landingMode = LandingMode.FlyBack;

    private void Start()
    {
        initPos = transform.position;
        initRot = transform.eulerAngles + new Vector3(0.0f, -90.0f, 0.0f);
    }

    private void Update()
    {
        if (!startOrbit)
        {
            // To avoid accidentally play audio source, root cause not found yet
            if (audioSource.isPlaying) audioSource.Stop();
            return;
        }

        if (!((totalAngle / 360) > noOfRounds))
        {
            curr2DDistToCenter = Vector2.Distance(new Vector2(center.position.x, center.position.z), new Vector2(transform.position.x, transform.position.z));
            if ((curr2DDistToCenter > radius * 1.5) && (currHeightOffset != heightInitOffset))
            {
                isTeleport = true;
            }
            if (isTeleport)
            {
                totalAngle = 0;
                targetToFollow.transform.position = center.position + Vector3.up * heightInitOffset + currHorizontalDir * currRadius;
                currHeightOffset = heightInitOffset;
                isTeleport = false;
            }
            else
            {
                targetToFollow.transform.position = center.position + Vector3.up * currHeightOffset + currHorizontalDir * currRadius;
            }

            var step = 3f * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation((targetToFollow.transform.position - transform.position).normalized);
            transform.position = Vector3.MoveTowards(transform.position, targetToFollow.transform.position, step);
            currHeightOffset += (center.transform.position.y + heightOffset - transform.position.y) / (noOfRounds * 360 - totalAngle);

            currHorizontalDir = Quaternion.Euler(0, -angularSpeed * Time.deltaTime, 0) * currHorizontalDir;
            currRadius = Mathf.Min(radius, currRadius + radiusLerpSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.rotation.eulerAngles.y - prevRot.eulerAngles.y) < 2f
                && curr2DDistToCenter <= radius * 1.1f)
            {
                totalAngle += Mathf.Abs(transform.rotation.eulerAngles.y - prevRot.eulerAngles.y);
            }
        }
        else
        {
            switch (landingMode)
            {
                case LandingMode.FlyBack:
                    var step = 3f * Time.deltaTime;
                    transform.rotation = Quaternion.LookRotation((initPos + new Vector3(0.0f, 0.1f, 0.0f) - transform.position).normalized);
                    transform.position = Vector3.Lerp(transform.position, initPos + new Vector3(0.0f, 0.1f, 0.0f), Time.deltaTime);

                    if (Vector3.Distance(transform.position, initPos + new Vector3(0.0f, 0.1f, 0.0f)) <= 0.1f)
                    {
                        landingMode = LandingMode.PitchAdjustment;
                    }
                    break;

                case LandingMode.PitchAdjustment:

                    if (initRot.x > transform.eulerAngles.x)
                    {
                        transform.eulerAngles += new Vector3(20f * Time.deltaTime, 0.0f, 0.0f);
                    }
                    else
                    {
                        transform.eulerAngles += new Vector3(20f * -Time.deltaTime, 0.0f, 0.0f);
                    }

                    if (Mathf.Abs(initRot.x - transform.eulerAngles.x) < 1.0f)
                    {
                        landingMode = LandingMode.YawAdjustment;
                    }
                    break;

                case LandingMode.YawAdjustment:
                    transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime);
                    if ((transform.eulerAngles.y > initRot.y + 180) || (transform.eulerAngles.y < initRot.y))
                    {
                        transform.eulerAngles += new Vector3(0.0f, 60f * Time.deltaTime, 0.0f);
                    }
                    else
                    {
                        transform.eulerAngles += new Vector3(0.0f, 60f * -Time.deltaTime, 0.0f);
                    }

                    if (Mathf.Abs(initRot.y - transform.eulerAngles.y) < 1.0f && Vector3.Distance(transform.position, initPos) < 0.5f)
                    {
                        landingMode = LandingMode.End;
                    }
                    break;

                case LandingMode.End:
                    transform.eulerAngles = initRot;
                    totalAngle = 0f;
                    startOrbit = false;
                    rigidbody.useGravity = true;
                    rigidbody.isKinematic = false;
                    rigidbody.transform.localRotation = Quaternion.identity;
                    body.localRotation = Quaternion.identity;
                    StartCoroutine(DecreasingVolume());
                    isTeleport = false;
                    landingMode = LandingMode.FlyBack;
                    break;
            }
        }

        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    public void StartOrbit()
    {
        center = Camera.main.transform;
        Vector3 vecCenterToNode = transform.position - center.position;
        Vector3 projectNode = Vector3.Project(vecCenterToNode, Vector3.up) + center.position;
        Vector3 vec = transform.position - projectNode;
        currRadius = vec.magnitude;
        currHorizontalDir = vec.normalized;
        currHeightOffset = projectNode.y - center.position.y;

        startOrbit = true;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        body.localRotation = Quaternion.Euler(0f, 90f, 0f);
        audioSource.Play();
    }

    IEnumerator DecreasingVolume()
    {
        while (audioSource.volume > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            audioSource.volume -= 0.25f;
        }

        audioSource.Stop();
        audioSource.volume = 1f;
    }
}
