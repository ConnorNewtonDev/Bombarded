using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController _controller;
    private PlayerAnimator _animator;
    private Rigidbody _rb3d;
    private ControlScheme _controlScheme;
    private float dashCD = 0f;
    private bool dashing = false;
    
    public float moveSpeed = 3f;
    [Header("Dash Values")]
    public float dashCooldown = 1.5f;
    public float dashDuration = 0.2f;
    public float dashForce = 3f;
    
    public Vector2 MoveDir { get; private set; } = Vector2.zero;
    public Vector2 LookDir { get; private set; } = Vector2.zero;
    
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<PlayerAnimator>();
        _rb3d = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var input = GetComponent<PlayerInput>();
        SetControls(input);
        input.onControlsChanged += SetControls;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dashing)
        {
            ApplyMove();
            if(dashCD > 0)
                dashCD -= Time.deltaTime;
        }
        ApplyLook();
    }

    private void ApplyMove()
    {
        var finalMove = new Vector3(MoveDir.x,Physics.gravity.y, MoveDir.y);
        _controller.Move(finalMove * (moveSpeed * Time.deltaTime));
    }

    private void ApplyLook()
    {
        if (_controlScheme == ControlScheme.KeyboardMouse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                var point = hit.point;
                // Debug.DrawLine (ray.origin, hit.point)
                var heading = (point -transform.position).normalized;
                heading.y = 0;

                transform.forward = heading;
            }
        }
        else
        {
            Debug.Log($"Stick Rotation {LookDir}");
        }
    }

    private void Dash()
    {
        dashCD = dashCooldown;
        dashing = true;
        var dir = new Vector3(MoveDir.x, 0, MoveDir.y) * dashForce;
        LeanTween.move(gameObject, transform.position + dir, dashDuration)
            .setOnComplete(() => dashing = false);
    }
   
    private void SetControls(PlayerInput input)
    {
        switch (input.currentControlScheme)
        {
            case "Keyboard&Mouse":
                _controlScheme = ControlScheme.KeyboardMouse;
                break;
            case "Gamepad":
                _controlScheme = ControlScheme.Gamepad;
                break;
        }
    }

#region PlayerInputs    
    private void OnMove(InputValue args)
    {
        MoveDir = args.Get<Vector2>();
    }

    private void OnLook(InputValue args)
    {
        LookDir = args.Get<Vector2>();
    }

    private void OnDash(InputValue args)
    {
        if (dashCD <= 0)
        {
            Dash();
        }
    }
#endregion

    public void ToggleRigidbodyMode(bool state)
    {
        _controller.enabled = !state;
        _rb3d.isKinematic = !state;
        GetComponent<CapsuleCollider>().enabled = state;
    }

    enum ControlScheme
    {
        KeyboardMouse,
        Gamepad
    }
}
