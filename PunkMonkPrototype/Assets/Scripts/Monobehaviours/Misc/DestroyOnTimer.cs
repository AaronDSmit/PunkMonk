using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DestroyOnTimer : MonoBehaviour
{
    [Header("Destroys the attached GameObject after time")]
    [SerializeField]
    private float time = 1f;
    
	void Awake()
    {
        Destroy(gameObject, time);
	}
	
}
