using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class NetPlayerMovment : MonoBehaviour
{
    private PlayerAnimator _animator;

    private void Awake()
    {
        _animator = GetComponent<PlayerAnimator>();
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void UpdatePositionAndRotation(Vector3 pos, Quaternion rotation)
    {
        transform.SetPositionAndRotation(pos, rotation);
    }
}
