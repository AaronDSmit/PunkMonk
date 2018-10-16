using System.Collections;
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

    public CharacterController Controller
    {
        set
        {
            controller = value;
            animator = controller.GetComponentInChildren<Animator>();
        }
    }

    #endregion


    private void Awake()
    {
        Manager.instance.StateController.OnGameStateChanged += GameStateChanged;
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
        if (inOverworld)
        {
            if (useMouseInput)
                ProcessMouseInput();
            if (useKeyboardInput)
                ProcessKeyboardInput();

            Movement();
        }
    }

    private void Movement()
    {
        if ((mouseInput || keyboardInput) && direction.sqrMagnitude != 0)
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

        if (earthGO)
        {
            Controller = GameObject.FindGameObjectWithTag("EarthUnit").GetComponent<CharacterController>();
        }
        else
        {
            Debug.LogError("No Earth unit found!");
        }

    }
}