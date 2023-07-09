using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CreditsCam : MonoBehaviour
{
    private CinemachineVirtualCamera cam;
    
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (cam.enabled)
        // {
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         
        //     }
        // }
    }
    
    private void OnEnable()
    {
        GameManager.StartNextSentence += OnStartNextSentence;
        GameManager.ResetGame += ResetSentence;
    }

    private void OnDisable()
    {
        GameManager.StartNextSentence -= OnStartNextSentence;
        GameManager.ResetGame -= ResetSentence;
    }

    private void OnStartNextSentence(int next)
    {
        if (next > 7)
        {
            cam.enabled = true;
        }
        else
        {
            cam.enabled = false;
        }
    }
    
    private void ResetSentence()
    {
        cam.enabled = false;
    }
}
