using Cinemachine;
using System.Collections;
using UnityEngine;

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

    private Unit targetUnit;

    private bool inOverworld = true;

    private OverworldController overworldController;

    private Vector3 rigTargetPos;
    private Vector3 cameraTargetPos;
    private Vector3 cameraStartPos;

    private Quaternion targetRot;

    private Vector3 vel;

    private Camera cam;

    private bool lookAtObject = false;
    private bool canMove = true;

    private GameSettings settings;

    public delegate void GlamCamEvent();

    public GlamCamEvent onGlamCamStart;
    public GlamCamEvent onGlamCamEnd;

    [Header("Overworld")]
    [SerializeField]
    private float overworldSpeed;
    [SerializeField] private float overworldDistance;

    [Header("CutScene")]
    [SerializeField] private GameObject defaultCam;
    [SerializeField] private GameObject startingCam;


    [Header("Game")]
    [SerializeField]
    private float speed;
    [SerializeField] private float RotationalSpeed;
    [SerializeField] private float scrollSpeed;

    [SerializeField] private bool inverseRotation;
    [SerializeField] private bool screenPan = true;

    [SerializeField] private int zMax;
    [SerializeField] private int zMin;
    [SerializeField] private int xMax;
    [SerializeField] private int xMin;

    [SerializeField] private int mousePanThresholdYUp = 100;
    [SerializeField] private int mousePanThresholdYDown = 100;
    [SerializeField] private int mousePanThresholdXLeft = 100;
    [SerializeField] private int mousePanThresholdXRight = 100;

    [SerializeField] private float scrollAmount = 1;
    [SerializeField] private float distance = 25;
    [SerializeField] private float minDistance = 5;
    [SerializeField] private float maxDistance = 30;

    [SerializeField] [Range(0, 100)] private float glamCamChance = 15f;

    [SerializeField] private Transform cinemachineDefault;


    private Vector3 dir;

    private Cinemachine.CinemachineBrain cinemachineBrain;

    bool cinemachine = true;

    public Unit TargetUnit
    {
        get
        {
            return targetUnit;
        }

    }

    public float GlamCamChance { get { return glamCamChance; } }

    public int ZMax
    {
        set
        {
            zMax = value;
        }
    }

    public int ZMin
    {
        set
        {
            zMin = value;
        }
    }

    public int XMax
    {
        set
        {
            xMax = value;
        }
    }

    public int XMin
    {
        set
        {
            xMin = value;
        }
    }

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
        //camera = transform.GetChild(0).gameObject;
        settings = SettingsLoader.Instance.CurrentSettings;

        overworldController = GameObject.FindGameObjectWithTag("Player").GetComponent<OverworldController>();
    }

    private void Start()
    {
        rigTargetPos = lightningUnit.transform.position;
        cameraTargetPos = cinemachineDefault.position;
        cameraStartPos = cinemachineDefault.localPosition;
        distance = overworldDistance;
        targetRot = transform.rotation;


        cam = GetComponentInChildren<Camera>();

        cinemachineBrain = gameObject.GetComponentInChildren<Cinemachine.CinemachineBrain>();

        StartCoroutine(StartCutscene());
        StartCoroutine(FinishCutscene());


        // Subscribe to the settings changing delegate
        settings.onSettingsChanged += SettingsChanged;
        SettingsChanged();

    }

    private IEnumerator FinishCutscene()
    {
        yield return new WaitForSeconds(cinemachineBrain.m_CustomBlends.m_CustomBlends[1].m_Blend.m_Time);
        Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().StopIntroCutscene();
    }

    private IEnumerator StartCutscene()
    {
        yield return new WaitForSeconds(2.0f);
        startingCam.SetActive(false);
        defaultCam.SetActive(true);
        cinemachine = false;
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


            if (Vector3.Distance(cam.transform.position, defaultCam.transform.position) < 0.1f)
            {
                transform.position = Vector3.Slerp(transform.position, rigTargetPos, Time.deltaTime * overworldSpeed);
            }


            if (!inOverworld)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, xMin, xMax), transform.position.y, Mathf.Clamp(transform.position.z, zMin, zMax));
            }


            if (transform.position.x == xMin || transform.position.x == xMax || transform.position.z == zMin || transform.position.z == zMax)
            {
                rigTargetPos = transform.position;
            }

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

        targetUnit = Manager.instance.PlayerController.GetComponent<OverworldController>().startWithClade ? earthUnit : lightningUnit;
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
        if (targetUnit != null)
        {
            rigTargetPos = targetUnit.transform.position;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchOverworldTargetUnit();
            }

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

        Plane plane = new Plane(Vector3.up, transform.position);
        Ray ray = new Ray(cinemachineDefault.transform.position, cinemachineDefault.transform.forward);
        float enterDistance = -1;
        if (plane.Raycast(ray, out enterDistance))
        {
            Vector3 hitpos = ray.GetPoint(enterDistance);
            rigTargetPos = a_position - (hitpos - transform.position);
        }
        else
        {
            rigTargetPos = a_position;
        }

        if (a_time == 0)
        {
            canMove = false;
        }
        else
        {
            lookAtObject = true;
            StartCoroutine(LookAtTimer(a_time, oldPos));
        }
    }

    private IEnumerator LookAtTimer(float a_timer, Vector3 a_pos)
    {
        yield return new WaitForSeconds(a_timer);

        lookAtObject = false;
        rigTargetPos = a_pos;
    }

    public void PlayGlamCam(Unit a_unit)
    {
        if (onGlamCamStart != null)
        {
            onGlamCamStart();
        }

        defaultCam.SetActive(false);
        a_unit.transform.GetChild(5).gameObject.SetActive(true);

        StartCoroutine(ChangeGlamCam(a_unit));

    }

    private IEnumerator ChangeGlamCam(Unit a_unit)
    {
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);


        a_unit.transform.GetChild(4).gameObject.SetActive(false);
        a_unit.transform.GetChild(5).gameObject.SetActive(true);

        Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().PlayBlackBars(cinemachineBrain.m_CustomBlends.m_CustomBlends[0].m_Blend.m_Time);

        StartCoroutine(EndGlamCam(a_unit));
    }

    private IEnumerator EndGlamCam(Unit a_unit)
    {
        yield return new WaitForSeconds(cinemachineBrain.m_CustomBlends.m_CustomBlends[0].m_Blend.m_Time);

        Manager.instance.transform.GetChild(0).GetComponent<MenuHelper>().StopBlackBars();

        a_unit.transform.GetChild(5).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);

        onGlamCamEnd();
    }

    public void SwitchOverworldTargetUnit()
    {
        targetUnit.GetComponent<OverworldFollower>().enabled = true;

        if (targetUnit == earthUnit)
        {
            targetUnit = lightningUnit;
        }
        else
        {
            targetUnit = earthUnit;
        }

        overworldController.Controller = targetUnit.GetComponent<CharacterController>();
        targetUnit.GetComponent<OverworldFollower>().enabled = false;
    }

}
