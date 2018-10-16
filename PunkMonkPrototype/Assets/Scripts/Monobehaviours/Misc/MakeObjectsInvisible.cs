using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeObjectsInvisible : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> objects = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "MainCamera")
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(true);
            }
        }
    }
}
