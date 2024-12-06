using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive3DSP;
using System.Linq;
using UnityEngine;

public class ListenerAction : MonoBehaviour
{
    // 3DSP room
    Vive3DSPAudioRoom vive3dspLivingroom;
    Vive3DSPAudioRoom vive3dspBathroom;
    Vive3DSPAudioSource vive3dspGramaphoneSource;
    public GameObject rightHand;

    // player
    //public GameObject playerCamera;

    // stand points
    public GameObject pointRmYard;
    public GameObject pointRmLiving;
    public GameObject pointRmBath;
    public GameObject pointNearField;
    public GameObject pointSrcDir;
    public GameObject pointOcc;
    public GameObject pointSpatial;
    public GameObject pointFrame;

    // text instruction
    public GameObject textRmYard;
    public GameObject textPtRmYard;
    public GameObject textRmLiving;
    public GameObject textPtRmLiving;
    public GameObject textRmBath;
    public GameObject textPtRmBath;
    public GameObject textNearField;
    public GameObject textPtNearField;
    public GameObject textSrcDir;
    public GameObject textPtSrcDir;
    public GameObject textOcc;
    public GameObject textPtOcc;
    public GameObject textSpatial;
    public GameObject textPtSpatial;
    public GameObject textExit;
    public GameObject textPtExit;


    // source object
    public GameObject bee;
    public GameObject robot;
    public GameObject robotOrigin;
    public GameObject gramaphone;
    public GameObject rmLivingroom;
    public GameObject rmBathroom;
    public GameObject door;


    // audio source
    public AudioSource beeAudioSrc;
    public AudioSource robotAudioSrc;
    public AudioSource gramaphoneAudioSrc;

    // room effect audio clip
    public AudioClip clip_yard;
    public AudioClip clip_livingroom;
    public AudioClip clip_bathroom;
    public AudioClip clip_vrss;

    // distance to teleport point
    private float distToRmYard;
    private float distToRmLiving;
    private float distToRmBath;
    private float distToNearField;
    private float distToSrcDir;
    private float distToOcc;
    private float distToSpatial;
    private float distToFrame;

    // effect flag
    private enum EffectFlag
    {
        isRmYard,
        isRmLiving,
        isRmBath,
        isNearField,
        isSrcDir,
        isOcc,
        isSpatial
    }

    private int[] flag;

    // Init
    private Vector3 beePositionInit;
    private Vector3 beeTargetPosition;
    private Vector3 doorPositionInit;
    private Vector3 doorPositionTarget;
    private float _time = 0F;

    private Vector3 gramaphonePositionInit;
    bool isDoorOpen = true;
    bool isDoorMove = true;
    float timerDoor = 0;
    float timerAudio = 2f;
    int demoMode = 0;  // 1: rmYard, 2: rmLiving, 3: rmBath, 4: nearField, 5: srcDir, 6:Occ, 7:Spatial, 8:Exit
    int preMode = 0;
    private Transform vrOrigin = null;
    private RigidPose hmdPose = RigidPose.identity;

    // Start is called before the first frame update
    void Start()
    {
        //if (vrOrigin == null) vrOrigin = GameObject.Find("VROrigin").transform;
        //hmdPose = VivePose.GetPose(DeviceRole.Hmd, vrOrigin);
        if (Camera.main)
        {
            hmdPose.pos = Camera.main.transform.position;
            hmdPose.rot = Camera.main.transform.rotation;
        }
        //playerCamera = GameObject.Find("Camera");
        rightHand = GameObject.Find("RightHand");

        pointRmYard = GameObject.Find("PointRmYard");
        pointRmLiving = GameObject.Find("PointRmLiving");
        pointRmBath = GameObject.Find("PointRmBath");
        pointNearField = GameObject.Find("PointNearField");
        pointSrcDir = GameObject.Find("PointSrcDir");
        pointOcc = GameObject.Find("PointOcc");
        pointSpatial = GameObject.Find("PointSpatial");
        pointFrame = GameObject.Find("PointFrame");

        textRmYard = GameObject.Find("Text-RmYard");
        textRmLiving = GameObject.Find("Text-RmLiving");
        textRmBath = GameObject.Find("Text-RmBath");
        textNearField = GameObject.Find("Text-NearField");
        textSrcDir = GameObject.Find("Text-SrcDir");
        textOcc = GameObject.Find("Text-Occ");
        textSpatial = GameObject.Find("Text-Spatial");
        textExit = GameObject.Find("Text-Exit");

        textPtRmYard = GameObject.Find("TextPt-RmYard");
        textPtRmLiving = GameObject.Find("TextPt-RmLiving");
        textPtRmBath = GameObject.Find("TextPt-RmBath");
        textPtNearField = GameObject.Find("TextPt-NearField");
        textPtSrcDir = GameObject.Find("TextPt-SrcDir");
        textPtOcc = GameObject.Find("TextPt-Occ");
        textPtSpatial = GameObject.Find("TextPt-Spatial");
        textPtExit = GameObject.Find("TextPt-Exit");

        bee = GameObject.Find("Bee");
        robot = GameObject.Find("RobotObj");
        robotOrigin = GameObject.Find("Robot");
        gramaphone = GameObject.Find("Gramaphone");
        rmLivingroom = GameObject.Find("Room-LivinRoom");
        rmLivingroom.SetActive(true);
        rmBathroom = GameObject.Find("Room-BathRoom");
        rmBathroom.SetActive(true);
        door = GameObject.Find("DoorL");

        beeAudioSrc = bee.GetComponent<AudioSource>();
        robotAudioSrc = GameObject.Find("RobotAudioSource").GetComponent<AudioSource>();
        gramaphoneAudioSrc = gramaphone.GetComponentInChildren<AudioSource>();

        clip_yard = Resources.Load("DemoAudioClips/vivee_yard_2594") as AudioClip;
        clip_livingroom = Resources.Load("DemoAudioClips/vivee_livingroom_2594") as AudioClip;
        clip_bathroom = Resources.Load("DemoAudioClips/vivee_bathroom_2594") as AudioClip;
        clip_vrss = Resources.Load("DemoAudioClips/vivee_return") as AudioClip;

        distToRmYard = getDistance(pointRmYard);
        distToRmLiving = getDistance(pointRmLiving);
        distToRmBath = getDistance(pointRmBath);
        distToNearField = getDistance(pointNearField);
        distToSrcDir = getDistance(pointSrcDir);
        distToOcc = getDistance(pointOcc);
        distToSpatial = getDistance(pointSpatial);
        distToFrame = getDistance(pointFrame);

        vive3dspLivingroom = rmLivingroom.GetComponent<Vive3DSPAudioRoom>();
        vive3dspBathroom = rmBathroom.GetComponent<Vive3DSPAudioRoom>();
        vive3dspGramaphoneSource = gramaphoneAudioSrc.GetComponent<Vive3DSPAudioSource>();

        beePositionInit = bee.transform.position;
        gramaphonePositionInit = gramaphone.transform.localEulerAngles;
        doorPositionInit = door.transform.position;
        doorPositionTarget = door.transform.position + new Vector3(0F, 0F, 1.8F);

        beeStandby();
        gramaphoneStandby();
    }

    public void setFlag()
    {
        if (distToRmYard < 1)
        {
            flag = new int[] { 1, 0, 0, 0, 0, 0, 0, 0 };
        }
        else if (distToRmLiving < 0.5)
        {
            flag = new int[] { 0, 1, 0, 0, 0, 0, 0, 0 };
        }
        else if (distToRmBath < 0.5)
        {
            flag = new int[] { 0, 0, 1, 0, 0, 0, 0, 0 };
        }
        else if (distToNearField < 1.5)
        {
            flag = new int[] { 0, 0, 0, 1, 0, 0, 0, 0 };
        }
        else if (distToSrcDir < 1)
        {
            flag = new int[] { 0, 0, 0, 0, 1, 0, 0, 0 };
        }
        else if (distToOcc < 1)
        {
            flag = new int[] { 0, 0, 0, 0, 0, 1, 0, 0 };
        }
        else if (distToSpatial < 1)
        {
            flag = new int[] { 0, 0, 0, 0, 0, 0, 1, 0 };
        }
        else if (distToFrame < 1)
        {
            flag = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        }
    }

    public void setMode()
    {
        if (flag.SequenceEqual(new int[] { 1, 0, 0, 0, 0, 0, 0, 0 }) && hmdPose.pos.x > (-2.35 + 12.38))
        {
            demoMode = 1; // room yard
        }
        else if (flag.SequenceEqual(new int[] { 0, 1, 0, 0, 0, 0, 0, 0 }) && hmdPose.pos.x > (-12.35 + 12.38) && hmdPose.pos.x < (-2.35 + 12.38))
        {
            demoMode = 2; // room living room
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 1, 0, 0, 0, 0, 0 }) && hmdPose.pos.x < (-12.35 + 12.38))
        {
            demoMode = 3; // room bath room
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 1, 0, 0, 0, 0 }) && distToNearField <= 1.5)
        {
            demoMode = 4; // nearfield
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 0, 1, 0, 0, 0 }) && distToSrcDir <= 1.5)
        {
            demoMode = 5; // source directivity
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 0, 0, 1, 0, 0 }))
        {
            demoMode = 6; // occlusion
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 0, 0, 0, 1, 0 }))
        {
            demoMode = 7; // spatializer
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 0, 0, 0, 0, 1 }) || (flag.SequenceEqual(new int[] { 0, 0, 0, 1, 0, 0, 0, 0 }) && distToNearField > 1.5))
        {
            demoMode = 0;
        }
        else if (flag.SequenceEqual(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 }))
        {
            demoMode = 8; // exit
        }
    }

    public void textRotation()
    {
        Vector3 heghShift;
        Vector3 ptShift = new Vector3(0, 0.15F, 0);
        Vector3 cameraRot = new Vector3(0F, hmdPose.rot.eulerAngles.y, 0F);
        if (hmdPose.pos.y > 0.5)
        {
            heghShift = new Vector3(0F, hmdPose.pos.y - 0.4F, 0F);
        }
        else
        {
            heghShift = new Vector3(0F, 0.2F, 0F);
        }
        textRmYard.transform.eulerAngles = cameraRot;
        textPtRmYard.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textRmYard.transform.position = pointRmYard.transform.position + heghShift;
        textPtRmYard.transform.position = pointRmYard.transform.position + ptShift;
        textRmLiving.transform.eulerAngles = cameraRot;
        textPtRmLiving.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textRmLiving.transform.position = pointRmLiving.transform.position + heghShift;
        textPtRmLiving.transform.position = pointRmLiving.transform.position + ptShift;
        textRmBath.transform.eulerAngles = cameraRot;
        textPtRmBath.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textRmBath.transform.position = pointRmBath.transform.position + heghShift;
        textPtRmBath.transform.position = pointRmBath.transform.position + ptShift;
        textNearField.transform.eulerAngles = cameraRot;
        textPtNearField.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textNearField.transform.position = pointNearField.transform.position + heghShift;
        textPtNearField.transform.position = pointNearField.transform.position + ptShift;
        textSrcDir.transform.eulerAngles = cameraRot;
        textPtSrcDir.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textSrcDir.transform.position = pointSrcDir.transform.position + heghShift;
        textPtSrcDir.transform.position = pointSrcDir.transform.position + ptShift;
        textOcc.transform.eulerAngles = cameraRot;
        textPtOcc.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textOcc.transform.position = pointOcc.transform.position + heghShift;
        textPtOcc.transform.position = pointOcc.transform.position + ptShift;
        textSpatial.transform.eulerAngles = cameraRot;
        textPtSpatial.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textSpatial.transform.position = pointSpatial.transform.position + heghShift;
        textPtSpatial.transform.position = pointSpatial.transform.position + ptShift;
        textExit.transform.eulerAngles = cameraRot;
        textPtExit.transform.eulerAngles = cameraRot + new Vector3(90, 0, 0);
        textExit.transform.position = pointFrame.transform.position + heghShift;
        textPtExit.transform.position = pointFrame.transform.position + ptShift;
    }

    public void textAction()
    {
        switch (demoMode)
        {
            case 0: // not in any teleport
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 1: // room yard
                textRmYard.SetActive(false);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(true);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 2: // room living room
                textRmYard.SetActive(true);
                textRmLiving.SetActive(false);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(true);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 3: // room bathroom
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(false);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(true);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 4: // near field
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(false);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(true);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 5: // source dir
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(false);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(true);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 6: // occlusion
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(false);
                textSpatial.SetActive(true);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(true);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(false);
                break;
            case 7: // spatial
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(false);
                textExit.SetActive(true);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(true);
                textPtExit.SetActive(false);
                break;

            case 8: // exit
                textRmYard.SetActive(true);
                textRmLiving.SetActive(true);
                textRmBath.SetActive(true);
                textNearField.SetActive(true);
                textSrcDir.SetActive(true);
                textOcc.SetActive(true);
                textSpatial.SetActive(true);
                textExit.SetActive(false);

                textPtRmYard.SetActive(false);
                textPtRmLiving.SetActive(false);
                textPtRmBath.SetActive(false);
                textPtNearField.SetActive(false);
                textPtSrcDir.SetActive(false);
                textPtOcc.SetActive(false);
                textPtSpatial.SetActive(false);
                textPtExit.SetActive(true);
                break;
        }
    }

    bool isClockwise = true;
    public void gramaphoneRotate()
    {
        gramaphoneAudioSrc.mute = false;
        if (gramaphone.transform.eulerAngles.y > 170)
        {
            isClockwise = true;
        }
        if (gramaphone.transform.eulerAngles.y < 10)
        {
            isClockwise = false;
        }
        switch (isClockwise)
        {
            case true:
                gramaphone.transform.Rotate(0F, -Time.deltaTime * 10F, 0F);
                break;
            case false:
                gramaphone.transform.Rotate(0F, Time.deltaTime * 10F, 0F);
                break;
        }
    }

    public void gramaphoneStandby()
    {
        gramaphoneAudioSrc.mute = true;
        gramaphone.transform.localEulerAngles = gramaphonePositionInit;
    }

    public void gramaphoneStandAndPlay()
    {
        gramaphoneAudioSrc.mute = false;
        if (hmdPose.pos.x > 8.3)
        {
            gramaphone.transform.localEulerAngles = gramaphonePositionInit;
        }
        else
        {
            gramaphone.transform.localEulerAngles = gramaphonePositionInit + new Vector3(0F, -130F, 0F);
        }
    }

    public void beeFly()
    {
        float speed;
        if (demoMode != preMode)
        {
            _time = 0;
            beeAudioSrc.mute = false;
            beeTargetPosition = hmdPose.pos;
        }

        _time += Time.deltaTime;

        if (_time > 2)
        {
            Vector3 targetAngle = new Vector3(Random.Range(0F, 360F), Random.Range(0F, 360F), Random.Range(0F, 360F));
            Vector3 targetRad = new Vector3(Random.Range(-1.2F, 1.2F), 0F, 0F);
            beeTargetPosition = hmdPose.pos + Quaternion.Euler(targetAngle) * targetRad;
            _time = 0;
        }
        else
        {
            beeTargetPosition = beeTargetPosition + new Vector3(Random.Range(-0.1F, 0.1F), Random.Range(-0.1F, 0.1F), Random.Range(-0.1F, 0.1F));
        }

        Vector3 flyDir = bee.transform.position - beeTargetPosition;
        if (Vector3.Distance(beeTargetPosition, bee.transform.position) > 2)
        {
            speed = 1;
        }
        else
        {
            speed = 0.1F;
        }

        bee.transform.position -= flyDir * speed * Time.deltaTime;
    }

    float timer = 0;
    float angle = 0;
    public void beeFlyCircle()
    {
        float speed = 1.2F;

        if (demoMode != preMode)
        {
            beeAudioSrc.mute = false;
            beeTargetPosition = hmdPose.pos + hmdPose.right * 0.8F;
            angle = 90;
        }

        Vector3 flyDir = bee.transform.position - beeTargetPosition + new Vector3(Random.Range(-0.1F, 0.1F), Random.Range(-0.1F, 0.1F), Random.Range(-0.1F, 0.1F));
        bee.transform.position -= flyDir * speed * Time.deltaTime;
        bee.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y - 180, 0F);
        if (Vector3.Distance(beeTargetPosition, bee.transform.position) < 0.1 && timer > 3)
        {
            angle += Random.Range(15F, 30F) * Time.deltaTime;
            float rad = Random.Range(0.3F, 0.9F);
            beeTargetPosition = hmdPose.pos + new Vector3(Mathf.Sin(angle) * rad, Random.Range(-0.2F, 0.05F), Mathf.Cos(angle) * rad);
            if (angle > 90 + 3.1415926 * 3.6F)
            {
                beeTargetPosition = beePositionInit;
                angle = 90;
                timer = 0;
            }
        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    private bool isBeeApproach = false;
    private float rotSum = 0;
    private int beeFlyMode = 0;  // 0: rest, 1:approach to camera right side, 2: rotate around, 3: close to head, 4: fly away
    public void beeFlyCloseToHead()
    {
        if (preMode != demoMode)
        {
            timer = 0;
            rotSum = 0;
        }
        if (beeFlyMode == 0 && timer <= 3)
        {
            beeAudioSrc.mute = true;
            timer += Time.deltaTime;
        }

        float speed = 1.4F;
        Vector3 flyDir;
        if (beeFlyMode == 0 && timer > 3) //beeFlyMode 0:rest
        {
            beeAudioSrc.mute = false;
            beeFlyMode = 1;
            rotSum = 0;
            beeTargetPosition = hmdPose.pos + hmdPose.right * 0.3F;
            flyDir = beePositionInit - beeTargetPosition;
            bee.transform.position -= flyDir * speed * Time.deltaTime;
        }

        if (beeFlyMode == 1) //beeFlyMode 1: approach to camera right side
        {
            beeAudioSrc.mute = false;
            bee.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y - 180, 0F);
            flyDir = beePositionInit - beeTargetPosition;
            bee.transform.position -= flyDir * speed / 4 * Time.deltaTime;
            if (Vector3.Distance(beeTargetPosition, bee.transform.position) < 0.1)
            {
                beeFlyMode = 2;
            }
        }

        if (beeFlyMode == 2) //beeFlyMode 2: rotate around
        {
            beeAudioSrc.mute = false;
            bee.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y - 180, 0F);
            bee.transform.RotateAround(Camera.main.transform.position, Vector3.up, 40 * Time.deltaTime);
            rotSum += 40 * Time.deltaTime;
            if (rotSum > 270)
            {
                beeFlyMode = 3;
            }
        }

        if (beeFlyMode == 3) //beeFlyMode 3: close to head
        {
            beeAudioSrc.mute = false;
            bee.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y - 180, 0F);
            beeTargetPosition = Camera.main.transform.position + hmdPose.forward * 0.07F - hmdPose.up * 0.06F;
            flyDir = bee.transform.position - beeTargetPosition;
            bee.transform.position -= flyDir * speed * 6 * Time.deltaTime;
            if (Vector3.Distance(beeTargetPosition, bee.transform.position) < 0.01)
            {
                beeFlyMode = 4;
                beeTargetPosition = beePositionInit;
            }
        }

        if (beeFlyMode == 4) //beeFlyMode 4: fly away
        {
            beeAudioSrc.mute = false;
            bee.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y - 180, 0F);
            flyDir = bee.transform.position - beeTargetPosition + new Vector3(Random.Range(-0.2F, 0.2F), Random.Range(-0.2F, 0.2F), Random.Range(-0.2F, 0.2F));
            bee.transform.position -= flyDir * speed / 4 * Time.deltaTime;
            if (Vector3.Distance(beeTargetPosition, bee.transform.position) < 0.1)
            {
                beeFlyMode = 0;
                timer = 0;
            }
        }
    }
    public void beeStandby()
    {
        _time = 0F;
        bee.transform.position = beePositionInit;
        if (beeAudioSrc.mute == false)
        {
            beeAudioSrc.mute = true;
        }
    }

    bool isUp = true;
    public void robotSpeak()
    {
        // robot obj setting
        robot.SetActive(true);
        robot.transform.eulerAngles = new Vector3(0F, hmdPose.rot.eulerAngles.y + 180, 0F);

        float robotDown = hmdPose.pos.y - 0.05F; ;
        float robotUp = hmdPose.pos.y;

        if (demoMode != preMode)
        {
            robotOrigin.transform.position = hmdPose.pos + hmdPose.right * 0.5F;
            robotOrigin.transform.eulerAngles = new Vector3(0, (hmdPose.rot.eulerAngles.y - 90), 0);
        }
        if (robotOrigin.transform.position.y > robotUp)
        {
            isUp = false;
        }
        if (robotOrigin.transform.position.y < robotDown)
        {
            isUp = true;
        }
        switch (isUp)
        {
            case true:
                {
                    robotOrigin.transform.position += new Vector3(0F, 0.02F * Time.deltaTime, 0F);
                    break;
                }
            case false:
                {
                    robotOrigin.transform.position -= new Vector3(0F, 0.02F * Time.deltaTime, 0F);
                    break;
                }
        }

        //robotOrigin.transform.Rotate(0F, Time.deltaTime * 10F, 0F);

        // robot audio source setting
        robotAudioSrc.mute = false;
        if (robotAudioSrc.isPlaying == false)
        {
            robotAudioSrc.Play();
        }
    }

    public void robotStandby()
    {
        robot.SetActive(false);
        robotAudioSrc.mute = true;
    }

    public void doorOpenAndClose()
    {
        if (getDistance(GameObject.Find("DoorFrame")) < 1)
        {
            door.transform.position = doorPositionTarget;
        }
        else
        {
            if (doorPositionTarget.z < door.transform.position.z && timerDoor == 0)
            {
                isDoorOpen = false;
                isDoorMove = false;
            }
            else if (doorPositionInit.z > door.transform.position.z && timerDoor == 0)
            {
                isDoorOpen = true;
                isDoorMove = false;
            }
            else if (timerDoor > 1 && !isDoorMove)
            {
                timerDoor = 0;
                isDoorMove = true;
            }

            if (isDoorOpen && isDoorMove)
            {
                door.transform.position += new Vector3(0, 0, Time.deltaTime * 0.3F);
            }
            else if (!isDoorOpen && isDoorMove)
            {
                door.transform.position -= new Vector3(0, 0, Time.deltaTime * 0.3F);
            }
            else if (!isDoorMove)
            {
                timerDoor += Time.deltaTime;
            }

        }
    }

    public void doorAction()
    {
        if (getDistance(GameObject.Find("DoorFrame")) < 1)
        {
            door.transform.position = doorPositionTarget;
        }
        else
        {
            door.transform.position = doorPositionInit;
        }
    }

    public void roomControl()
    {
        float camerX = hmdPose.pos.x;
        if (camerX > 9.5) // in the yard
        {
            rmLivingroom.SetActive(false);
            vive3dspLivingroom.RoomEffect = false;
            rmBathroom.SetActive(false);
            vive3dspBathroom.RoomEffect = false;
        }
        else if (camerX < -0.4) // in the bath room
        {
            rmLivingroom.SetActive(false);
            vive3dspLivingroom.RoomEffect = false;
            rmBathroom.SetActive(true);
            vive3dspBathroom.RoomEffect = true;
        }
        else // in the living room
        {
            rmLivingroom.SetActive(true);
            vive3dspLivingroom.RoomEffect = true;
            rmBathroom.SetActive(false);
            vive3dspBathroom.RoomEffect = false;
        }
    }
    public float getDistance(GameObject obj)
    {
        //float dist = Vector3.Distance(hmdPose.pos, obj.transform.position);
        float dist = Mathf.Sqrt(Mathf.Pow((hmdPose.pos.x - obj.transform.position.x), 2) + Mathf.Pow((hmdPose.pos.z - obj.transform.position.z), 2));
        return dist;
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera.main)
        {
            hmdPose.pos = Camera.main.transform.position;
            hmdPose.rot = Camera.main.transform.rotation;
        }

        distToRmYard = getDistance(pointRmYard);
        distToRmLiving = getDistance(pointRmLiving);
        distToRmBath = getDistance(pointRmBath);
        distToNearField = getDistance(pointNearField);
        distToSrcDir = getDistance(pointSrcDir);
        distToOcc = getDistance(pointOcc);
        distToSpatial = getDistance(pointSpatial);
        distToFrame = getDistance(pointFrame);

        setFlag();
        setMode();
        textRotation();
        textAction();

        if (demoMode != 8) robotAudioSrc.loop = true;

        switch (demoMode)
        {
            case 0: // not in any teleport
                doorAction();
                robotStandby();
                gramaphoneStandAndPlay();
                beeStandby();
                break;
            case 1: // room yard
                doorAction();
                robotAudioSrc.clip = clip_yard;
                robotSpeak();
                gramaphoneStandby();
                beeStandby();
                break;
            case 2: // room living room
                doorAction();
                robotAudioSrc.clip = clip_livingroom;
                robotSpeak();
                gramaphoneStandAndPlay();
                beeStandby();
                break;
            case 3: // room bathroom
                doorAction();
                robotAudioSrc.clip = clip_bathroom;
                robotSpeak();
                gramaphoneStandAndPlay();
                beeStandby();
                break;
            case 4: // near field
                doorAction();
                robotStandby();
                gramaphoneStandby();
                beeFlyCloseToHead();
                break;
            case 5: // source dir
                door.transform.position = doorPositionTarget;
                robotStandby();
                gramaphoneRotate();
                beeStandby();
                break;
            case 6: // occlusion
                robotStandby();
                gramaphoneStandAndPlay();
                beeStandby();
                doorOpenAndClose();
                break;
            case 7: // spatial
                robotStandby();
                gramaphoneStandAndPlay();
                beeStandby();
                break;
            case 8: // exit
                if (preMode != demoMode)
                {
                    doorAction();
                    robotAudioSrc.clip = clip_vrss;
                    robotAudioSrc.loop = false;
                    robotSpeak();
                    gramaphoneStandby();
                    beeStandby();
                }
                else
                {
                    if (robotAudioSrc.isPlaying) return;

                    if (timerAudio > 0f)
                    {
                        timerAudio -= Time.deltaTime;
                    }
                    else
                    {
                        timerAudio = 2f;
                        doorAction();
                        robotAudioSrc.clip = clip_vrss;
                        robotSpeak();
                        gramaphoneStandby();
                        beeStandby();
                    }
                }

                break;
        }

        if (preMode != demoMode)
        {
            preMode = demoMode;
        }
    }
}
