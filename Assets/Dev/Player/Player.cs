using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    PlayerInput input;
    Vector2 moveDir;

    private void OnEnable()
    {
        input.Main.Enable();
    }

    private void OnDisable()
    {
        input.Main.Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        input = new();
        input.Main.Move.performed += ctx => moveDir = ctx.ReadValue<Vector2>();
        input.Main.Move.canceled += ctx => moveDir = Vector2.zero;
    }

    private void Update()
    {
        var moveVec = moveDir * 7f;
        rb.velocity = new Vector3(moveVec.x, rb.velocity.y, moveVec.y);
    }
}
