using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldController : MonoBehaviour
{
    private Unit earthUnit = null;

    private Unit lightningUnit = null;

    private OverworldFollower follower;

    private Vector3 currentNode;

    private bool inOverworld;

    private Vector3 direction;

    private bool mouseInput = false;
    private bool keyboardInput = false;

    [SerializeField]
    private float newNodeDistance = 1;

    [SerializeField]
    private float movementSpeed = 1;

    [SerializeField]
    private bool useKeyboardInput = true;
    [SerializeField]
    private bool useMouseInput = true;

    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
    }

    private void Start()
    {
        follower = lightningUnit.GetComponent<OverworldFollower>();
        DropNode();
    }

    private void GameStateChanged(GameState a_oldstate, GameState a_newstate)
    {
        // ensure this script knows it's in over-world state
        inOverworld = (a_newstate == GameState.overworld);
    }

    private void Update()
    {
        // Don't update if in any other game state
        if (inOverworld)
        {
            if (useMouseInput)
                ProcessMouseInput();
            if (useKeyboardInput)
                ProcessKeyboardInput();

            Movement();

            if (Vector3.Distance(earthUnit.transform.position, currentNode) >= newNodeDistance)
            {
                DropNode();
            }
        }
    }

    private void Movement()
    {
        if ((mouseInput || keyboardInput) && direction.sqrMagnitude != 0)
        {
            earthUnit.transform.position += direction.normalized * movementSpeed * Time.deltaTime;
            earthUnit.transform.rotation = Quaternion.Slerp(earthUnit.transform.rotation, Quaternion.LookRotation(direction.normalized, Vector3.up), 10.0f * Time.deltaTime);
        }
    }

    // Process Keyboard Input
    private void ProcessKeyboardInput()
    {
        if (!mouseInput)
        {
            keyboardInput = true;
            direction.x = Input.GetAxis("Horizontal");
            direction.z = Input.GetAxis("Vertical");
            direction = Camera.main.transform.TransformDirection(direction);
            direction.y = 0;
            direction = direction.normalized;
        }
        else
        {
            keyboardInput = false;
        }
    }

    // Process Mouse Input
    private void ProcessMouseInput()
    {
        if (inOverworld && earthUnit && lightningUnit)
        {
            if (Input.GetMouseButton(0))
            {
                mouseInput = true;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layerMask = 0;
                layerMask |= (1 << LayerMask.NameToLayer("Ground"));

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    Vector3 vecBetween = hit.transform.position - earthUnit.transform.position;
                    vecBetween.y = 0;
                    if (vecBetween.sqrMagnitude > 1)
                    {
                        direction = vecBetween;
                    }
                }
            }
            else
            {
                mouseInput = false;
            }
        }
    }

    private void DropNode()
    {
        currentNode = earthUnit.transform.position;
        follower.Nodes.Enqueue(currentNode);
    }

    // Initialisation function called when the scene is ready, sets up unit references
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

        GameObject lightningGO = GameObject.FindGameObjectWithTag("LightningUnit");

        if (lightningGO)
        {
            lightningUnit = GameObject.FindGameObjectWithTag("LightningUnit").GetComponent<Unit>();
        }
        else
        {
            Debug.LogError("No lightning unit found!");
        }
    }
}