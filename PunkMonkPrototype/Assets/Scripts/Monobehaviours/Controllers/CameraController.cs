using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum CameraDirection
{
    N,
    NE,
    NW,
    SE,
    SW,
    S
}



public class CameraController : MonoBehaviour
{
    private Unit earthUnit;

    private bool inOverworld;

    private float targetY;

    private Vector3 targetPos;

    private Quaternion targetRot;

    private Vector3 vel;

    private bool lookAtObject = false;
    private bool canMove = true;


    [SerializeField] private float glamCamDistance;

    [SerializeField] private float speed;
    [SerializeField] private float overworldSpeed;
    [SerializeField] private float RotationalSpeed;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private bool inverseRotation;

    private Vector3 dir;


    bool cinemachine;


    GameObject basicEarthGlamCam;
    GameObject specialEarthGlamCam;
    GameObject basicLightningGlamCam;
    GameObject specialLightningGlamCam;


    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void Start()
    {
        targetPos = transform.position;
        targetY = transform.position.y;
        targetRot = transform.rotation;

        basicEarthGlamCam = transform.GetChild(1).gameObject;
        specialEarthGlamCam = transform.GetChild(2).gameObject;
        basicLightningGlamCam = transform.GetChild(3).gameObject;
        specialLightningGlamCam = transform.GetChild(4).gameObject;

        basicEarthGlamCam.GetComponent<CinemachineVirtualCamera>().LookAt = GameObject.FindGameObjectWithTag("EarthUnit").transform.GetChild(0);
        specialEarthGlamCam.GetComponent<CinemachineVirtualCamera>().LookAt = GameObject.FindGameObjectWithTag("EarthUnit").transform.GetChild(0);

        basicLightningGlamCam.GetComponent<CinemachineVirtualCamera>().LookAt = GameObject.FindGameObjectWithTag("LightningUnit").transform.GetChild(0);
        specialLightningGlamCam.GetComponent<CinemachineVirtualCamera>().LookAt = GameObject.FindGameObjectWithTag("LightningUnit").transform.GetChild(0);


        basicEarthGlamCam.SetActive(false);
        specialEarthGlamCam.SetActive(false);
        basicLightningGlamCam.SetActive(false);
        specialLightningGlamCam.SetActive(false);
    }

    private void GameStateChanged(GameState _oldstate, GameState _newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == GameState.overworld);
    }

    private void Update()
    {
        // Don't update if in any other game state

        if (cinemachine == true)
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
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, targetPos) < 1f)
                    {

                        canMove = true;
                    }
                    transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * overworldSpeed);
                }
            }
            transform.position = Vector3.Slerp(transform.position, new Vector3(transform.position.x, targetY, transform.position.z), Time.deltaTime * scrollSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * RotationalSpeed);
        }

    }

    public void Init()
    {
        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");

        if (earthGO)
        {
            earthUnit = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
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


        transform.position += transform.right * vel.x;
        transform.position += transform.forward * vel.z;




        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (targetY > 1)
                targetY -= scrollSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (targetY < 10)
                targetY += scrollSpeed * Time.deltaTime;
        }


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
            targetPos = earthUnit.transform.position;
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * overworldSpeed);
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

    public void LookAtPosition(Vector3 a_position, float time = 0)
    {
        Vector3 oldPos = transform.position;

        if (time == 0)
        {
            targetPos = a_position;
            canMove = false;
        }
        else
        {
            lookAtObject = true;
            targetPos = a_position;
            StartCoroutine(LookAtTimer(time, oldPos));
        }
    }

    private IEnumerator LookAtTimer(float timer, Vector3 a_pos)
    {
        yield return new WaitForSeconds(timer);

        lookAtObject = false;
        targetPos = a_pos;
    }


    public void PlayEarthBasicAttackGlamCam(Vector3 a_pos, Vector3 a_vecBetween)
    {
        Vector3 rightPerp = a_pos + Vector3.Cross(a_vecBetween.normalized, Vector3.up) * glamCamDistance;

        transform.position = rightPerp;
        basicEarthGlamCam.SetActive(true);

    }

    public void PlayEarthSpecialAttackGlamCam(Vector3 a_vecBetween)
    {

    }
    public void PlayLightningBasicAttackGlamCam(Vector3 a_vecBetween)
    {

    }
    public void PlayLightingSpecialAttackGlamCam(Vector3 a_vecBetween)
    {

    }

}
