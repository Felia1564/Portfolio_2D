using System.Collections.Generic;
using UnityEngine;

public class MotherBrain : MonoBehaviour
{
    public static MotherBrain Instance { get; set; }


    private void Awake()
    {
        Instance = this;
    }


    private void OnEnable()
    {
        
    }


    private void OnDIsable()
    {

    }


    private void Start()
    {
        
    }


    void FixedUpdate()
    {

    }


    void Update()
    {
        
    }

    void LateUpdate()
    {

    }
}
