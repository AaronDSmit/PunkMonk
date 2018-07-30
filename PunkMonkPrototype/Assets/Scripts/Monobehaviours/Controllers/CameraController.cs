using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Unit earthUnit;

    private bool inOverworld;

    private GameObject cameraGO;

    private Vector3 targetPos;

    private Quaternion targetRot;

    private bool lockedMovement = false;

    [SerializeField] private float lerpSpeed;
    [SerializeField] private float RotationalSpeed;
    [SerializeField] private float cameraSpeed;


    private void Awake()
    {
        StateManager.OnGameStateChanged += GameStateChanged;

    }

    private void Start()
    {
        cameraGO = Camera.main.gameObject;
        targetPos = transform.position;
        targetRot = transform.rotation;
    }

    private void GameStateChanged(Game_state _oldstate, Game_state _newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (_newstate == Game_state.overworld);
    }

    private void Update()
    {
        // Don't update if in any other game state
        if (inOverworld)
        {
            ProcessOverworldCamera();
        }
        else
        {
            ProcessKeyboardInput();
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * RotationalSpeed);

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

        if (Input.GetKey(KeyCode.W))
        {
            targetPos += transform.forward * cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            targetPos += -transform.right * cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            targetPos += transform.right * cameraSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            targetPos += -transform.forward * cameraSpeed * Time.deltaTime;
        }



        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            targetPos += -Vector3.up * cameraSpeed * Time.deltaTime;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            targetPos += Vector3.up * cameraSpeed * Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.E))
        {
            targetRot *= Quaternion.Euler(new Vector3(0, -60, 0));
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetRot *= Quaternion.Euler(new Vector3(0, 60, 0));
        }





    }

    private void ProcessOverworldCamera()
    {
        if (earthUnit != null)
        {
            targetPos = earthUnit.transform.position;
        }
    }
}
