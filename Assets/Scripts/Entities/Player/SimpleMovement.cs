using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public float moveSpeed = 15;
    public float jumpSpeed = 10;

    private Vector3 _MoveDir;
    private Rigidbody _Body;
    float distanceToGround;

    public bool OnGround()
    {
        return Physics.Raycast(transform.position, -transform.up, distanceToGround + 0.5f);
    }
    
    void Start()
    {
        _Body = this.GetComponent<Rigidbody>();
        distanceToGround = GetComponent<Collider>().bounds.extents.y;

        //Lock and hide cursor
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Get the normal of the direction we're moving
        _MoveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
    }

    private void FixedUpdate()
    {
        _Body.MovePosition(_Body.position + transform.TransformDirection(_MoveDir) * moveSpeed * Time.deltaTime);

        //Space to jump
        if (Input.GetKeyDown(KeyCode.Space) && OnGround())
            _Body.velocity = transform.up * jumpSpeed;
    }
}
