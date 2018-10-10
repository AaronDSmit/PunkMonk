using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public enum CameraDirection
{
    N,
    NE,
    NW,
    SE,
    SW,
    S
}

public enum GlamCamType
{
    EARTH_BASIC,
    EARTH_SPECIAL,
    LIGHNING_BASIC,
    LIGHNING_SPECIAL
}


public class CameraController : MonoBehaviour
{
    private Unit earthUnit;
    private Unit lightningUnit;

    private bool inOverworld;


    private Vector3 rigTargetPos;
    private Vector3 cameraTargetPos;
    private Vector3 cameraStartPos;

    private Quaternion targetRot;

    private Vector3 vel;


    private bool lookAtObject = false;
    private bool canMove = true;

    private Vector3 oldCamPosition;
    //private Quaternion oldCamRotation;

    private Settings settings;

    public delegate void GlamCamEvent();

    public GlamCamEvent onGlamCamStart;
    public GlamCamEvent onGlamCamEnd;


    [Header("Overworld")]
    [SerializeField]
    private float overworldSpeed;
    [SerializeField] private float overworldDistance;
    [Header("Game")]
    [SerializeField]
    private float speed;
    [SerializeField] private float RotationalSpeed;
    [SerializeField] private float scrollSpeed;

    [SerializeField] private bool inverseRotation;
    [SerializeField] private bool screenPan = true;

    [SerializeField] private int mousePanThresholdYUp = 100;
    [SerializeField] private int mousePanThresholdYDown = 100;
    [SerializeField] private int mousePanThresholdXLeft = 100;
    [SerializeField] private int mousePanThresholdXRight = 100;

    [SerializeField] private float scrollAmount = 1;
    [SerializeField] private float distance = 25;
    [SerializeField] private float minDistance = 5;
    [SerializeField] private float maxDistance = 30;

    [SerializeField] private Transform cinemachineDefault;

    private Vector3 dir;

    private Cinemachine.CinemachineBrain cinemachineBrain;

    bool cinemachine = false;
    new GameObject camera = null;



    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
        camera = transform.GetChild(0).gameObject;
        settings = Resources.Load<Settings>("Settings/current");
    }

    private void Start()
    {
        rigTargetPos = transform.position;
        cameraTargetPos = cinemachineDefault.position;
        cameraStartPos = cinemachineDefault.localPosition;
        distance = overworldDistance;
        targetRot = transform.rotation;

        // Subscribe to the settings changing delegate
        settings.onSettingsChanged += SettingsChanged;

        cinemachineBrain = gameObject.GetComponentInChildren<Cinemachine.CinemachineBrain>();
    }

    private void OnDestroy()
    {
        // Unsubscribe to the settings changing delegate
        settings.onSettingsChanged -= SettingsChanged;
    }

    private void SettingsChanged()
    {
        inverseRotation = settings.InverseCameraRotation;
        screenPan = settings.ScreenEdgePan;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (a_newstate == GameState.overworld);

        if (a_oldstate != GameState.overworld && a_newstate == GameState.overworld)
        {
            distance = overworldDistance;
            SetRotation(CameraDirection.NE);
        }
    }

    private void Update()
    {
        // Don't update if in any other game state

        if (cinemachine == false)
        {
            if (lookAtObject == false)
            {
                if (canMove == true)
                {

                    if (inOverworld)
                    {
                        ProcessOverworldCamera();
                    }
                    else
                    {
                        ProcessKeyboardInput();

                        if (screenPan)
                        {
                            if (Input.mousePosition.x <= mousePanThresholdXLeft)
                            {
                                rigTargetPos += -transform.right * speed * Time.deltaTime;
                            }
                            else if (Input.mousePosition.x >= Screen.width - mousePanThresholdXRight)
                            {
                                rigTargetPos += transform.right * speed * Time.deltaTime;
                            }

                            if (Input.mousePosition.y <= mousePanThresholdYDown)
                            {
                                rigTargetPos += -transform.forward * speed * Time.deltaTime;
                            }
                            else if (Input.mousePosition.y >= Screen.height - mousePanThresholdYUp)
                            {
                                rigTargetPos += transform.forward * speed * Time.deltaTime;
                            }
                        }
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, rigTargetPos) < 1f)
                    {

                        canMove = true;
                    }
                }
            }
            cameraTargetPos = (cameraStartPos).normalized * distance;
            cinemachineDefault.localPosition = Vector3.Slerp(cinemachineDefault.localPosition, cameraTargetPos, Time.deltaTime * overworldSpeed);

            transform.position = Vector3.Slerp(transform.position, rigTargetPos, Time.deltaTime * overworldSpeed);
            //transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * scrollSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * RotationalSpeed);
        }

    }

    public void Init()
    {
        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");
        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");


        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }
        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<Unit>();
        }
    }

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {

        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (dir.magnitude > 1)
        {
            vel = dir.normalized * speed * Time.deltaTime;
        }
        else
        {
            vel = dir * speed * Time.deltaTime;
        }


        rigTargetPos += transform.right * vel.x;
        rigTargetPos += transform.forward * vel.z;


        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (distance > minDistance)
            {
                distance -= scrollAmount;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (distance < maxDistance)
            {
                distance += scrollAmount;
            }
        }
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (inverseRotation)
            {
                targetRot *= Quaternion.Euler(new Vector3(0, 60, 0));
            }
            else
            {
                targetRot *= Quaternion.Euler(new Vector3(0, -60, 0));
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (inverseRotation)
            {
                targetRot *= Quaternion.Euler(new Vector3(0, -60, 0));
            }
            else
            {
                targetRot *= Quaternion.Euler(new Vector3(0, 60, 0));
            }

        }
    }

    private void ProcessOverworldCamera()
    {
        if (earthUnit != null)
        {
            rigTargetPos = earthUnit.transform.position;
        }
        else
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<EarthUnit>();
        }
        // transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * overworldSpeed);
    }


    public void SetRotation(CameraDirection a_dir)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, CalculateRot(a_dir), 0));
        targetRot = transform.rotation;
    }

    public void SetRotationLerp(CameraDirection a_dir)
    {
        targetRot = Quaternion.Euler(new Vector3(0, CalculateRot(a_dir), 0));
    }

    private float CalculateRot(CameraDirection a_dir)
    {
        if (a_dir == CameraDirection.N)
        {
            return 0;
        }
        else if (a_dir == CameraDirection.NE)
        {
            return 60;
        }
        else if (a_dir == CameraDirection.SE)
        {
            return 120;
        }
        else if (a_dir == CameraDirection.S)
        {
            return 180;
        }
        else if (a_dir == CameraDirection.NW)
        {
            return -60;
        }
        else if (a_dir == CameraDirection.SW)
        {
            return -120;
        }
        return -1;
    }

    public void LookAtPosition(Vector3 a_position, float a_time = 0)
    {
        Vector3 oldPos = transform.position;

        if (a_time == 0)
        {
            rigTargetPos = a_position;
            canMove = false;
        }
        else
        {
            lookAtObject = true;
            rigTargetPos = a_position;
            StartCoroutine(LookAtTimer(a_time, oldPos));
        }
    }

    private IEnumerator LookAtTimer(float a_timer, Vector3 a_pos)
    {
        yield return new WaitForSeconds(a_timer);

        lookAtObject = false;
        rigTargetPos = a_pos;
    }



    public void PlayCinematicBars(Unit a_unit)
    {

        
        Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().PlayBlackBars(1);
        StartCoroutine(StopBlackBars((float)a_unit.GetComponent<PlayableDirector>().duration));

    }


    private IEnumerator StopBlackBars(float a_time)
    {
        yield return new WaitForSeconds(a_time);
        Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().PlayBlackBars(1);


    }

    //private IEnumerator EndGlamCam(Unit a_unit)
    //{
    //    yield return new WaitForSeconds(cinemachineBrain.m_CustomBlends.m_CustomBlends[0].m_Blend.m_Time);

    //    Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().StopBlackBars();

    //    a_unit.transform.GetChild(6).gameObject.SetActive(false);
    //    transform.GetChild(1).gameObject.SetActive(true);

    //    onGlamCamEnd();
    //}




}
