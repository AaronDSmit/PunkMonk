﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldController : MonoBehaviour
{
    #region Inspector Fields

    [SerializeField]
    private float raycastPlaneYLevel = 0.0f;

    [SerializeField]
    private float movementSpeed = 1;

    [SerializeField]
    private bool useKeyboardInput = true;
    [SerializeField]
    private bool useMouseInput = true;

    [SerializeField]
    public bool startWithClade = false;

    #endregion

    #region References

    private CharacterController controller = null;
    private Animator animator = null;

    #endregion

    #region Private Fields

    private Vector3 currentNode;

    private bool inOverworld;

    private Vector3 direction;

    private bool mouseInput = false;
    private bool keyboardInput = false;
    private bool canMove = false;
    private bool movingToHex = false;

    public CharacterController Controller
    {
        set
        {
            controller = value;
            animator = controller.GetComponentInChildren<Animator>();
        }
        get
        {
            return controller;
        }
    }

    #endregion

    public bool CanMove { get { return canMove; } set { canMove = value; } }

    public void RunToHex(Hex a_hex)
    {
        movingToHex = true;
        direction = (a_hex.transform.position - controller.transform.position).normalized;
    }

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void Start()
    {
        StartCoroutine(WaitForIntro());
    }

    private IEnumerator WaitForIntro()
    {
        yield return new WaitForSeconds(GameObject.FindGameObjectWithTag("CameraRig").GetComponentInChildren<Cinemachine.CinemachineBrain>().m_CustomBlends.m_CustomBlends[1].m_Blend.m_Time);
        canMove = true;
    }

    private void OnDestroy()
    {
        Manager.instance.StateController.OnGameStateChanged -= GameStateChanged;
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (a_newstate == GameState.overworld);
    }

    private void Update()
    {
        // Don't update if in any other game state
        if (inOverworld && canMove)
        {
            if (movingToHex == false)
            {
                if (useMouseInput)
                    ProcessMouseInput();
                if (useKeyboardInput)
                    ProcessKeyboardInput();
            }

            Movement();
        }
    }

    private void Movement()
    {
        if ((mouseInput || keyboardInput || movingToHex) && direction.sqrMagnitude != 0)
        {
            if (animator)
                animator.SetBool("Running", true);

            controller.Move(direction.normalized * movementSpeed * Time.deltaTime);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, Quaternion.LookRotation(direction.normalized, Vector3.up), 10.0f * Time.deltaTime);
        }
        else
        {
            if (animator)
                animator.SetBool("Running", false);
        }
    }

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {
        //keyboardInput = false;

        //if (!mouseInput)
        //{
        //    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) // UP
        //        direction.x += 1.0f;
        //    if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) // DOWN
        //        direction.x -= 1.0f;

        //    if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) // LEFT
        //        direction.z -= 1.0f;
        //    if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) // RIGHT
        //        direction.z += 1.0f;

        //    if (direction.z != 0f && direction.z != 0f)
        //    {
        //        keyboardInput = true;

        //        direction = Camera.main.transform.TransformDirection(direction);
        //        direction.y = 0;
        //        direction = direction.normalized;
        //    }
        //}
    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {
        mouseInput = false;

        if (inOverworld && controller)
        {
            if (Input.GetMouseButton(0))
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new Plane(Vector3.up, raycastPlaneYLevel);
                //RaycastHit hit;
                //int layerMask = 0;
                //layerMask |= (1 << LayerMask.NameToLayer("Ground"));

                //if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                //{
                //Vector3 vecBetween = hit.transform.position - earthUnit.transform.position;
                //vecBetween.y = 0;

                float distance = 0.0f;
                if (plane.Raycast(ray, out distance))
                {
                    Vector3 hitPoint = ray.GetPoint(distance);
                    Vector3 vecBetween = hitPoint - controller.transform.position;
                    vecBetween.y = 0;

                    if (vecBetween.sqrMagnitude > 1)
                    {
                        mouseInput = true;
                        direction = vecBetween;
                    }
                }
            }
        }
    }

    // Initialisation function called when the scene is ready, sets up unit references
    public void Init()
    {
        GameObject earthGO = GameObject.FindGameObjectWithTag("EarthUnit");
        GameObject lightninghGO = GameObject.FindGameObjectWithTag("LightningUnit");
        if (startWithClade)
        {

            if (earthGO)
            {
                Controller = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<CharacterController>();
                earthGO.GetComponent<OverworldFollower>().enabled = false;
                lightninghGO.GetComponent<OverworldFollower>().enabled = true;
            }
            else
            {
                Debug.LogError("No Earth unit found!", gameObject);
            }
        }
        else
        {

            if (lightninghGO)
            {
                Controller = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<CharacterController>();
                earthGO.GetComponent<OverworldFollower>().enabled = true;
                lightninghGO.GetComponent<OverworldFollower>().enabled = false;
            }
            else
            {
                Debug.LogError("No Lightning unit found!", gameObject);
            }
        }

    }
}