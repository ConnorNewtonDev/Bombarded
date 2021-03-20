using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class NetPlayerMovment : MonoBehaviour
{
    private PlayerAnimator _animator;
    public float posDampening = 1f;
    public float rotDampening = 1f;

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

    public void UpdatePositionAndRotation(Vector3 pos, Quaternion rot)
    {
        var position = Vector3.Lerp(transform.position, pos, posDampening);
        var rotation = Quaternion.Lerp(transform.rotation, rot, rotDampening);
        
        transform.SetPositionAndRotation(position, rotation);
    }
}
