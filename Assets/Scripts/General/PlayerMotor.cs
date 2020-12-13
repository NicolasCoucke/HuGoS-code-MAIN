using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{

    //[SerializeField]
    //private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;


    private Rigidbody rb;
    //public GameObject Graphics;
    private Rigidbody rbChild;

    public Transform groundcheck;
    public float groundDistance = 0.4f; 
    public LayerMask groundMask;

    public Transform thisTransform;
    bool IsColliding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //rbChild = Graphics.GetComponent<Rigidbody>();
        thisTransform = this.transform;

    }

    private void Update()
    {
        //bool isGrounded = Physics.CheckSphere(groundcheck.position, groundDistance, groundMask);

        //if (!isGrounded)
        //{
        //    Vector3 temppos = rb.position; temppos.y = 0; rb.position = temppos;
        //}

        if(IsColliding == false && rotation == Vector3.zero)
            rb.freezeRotation = true;


        if (IsColliding == false && velocity == Vector3.zero)
            rb.velocity = Vector3.zero;

    }

    // Gets a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    // Gets a rotational vector
    public void Rotate(Vector3 _rotation)
    { 
        rotation = _rotation;
      
    }

    //public void ChangeSize(int _sizeChange)
    //{
    //    if(_sizeChange == 1)
    //    {
    //        rbChild.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
    //    }

    //    if(_sizeChange == -1)
    //    {
    //        rbChild.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
    //    }
    //}



    // Run every physics iteration
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    //Perform movement based on velocity variable
    void PerformMovement()
    {
  
        if (velocity != Vector3.zero)
        {

            rb.MovePosition(rb.position + velocity * Time.deltaTime);
        }

        //
        
       

    }

    //Perform rotation
    void PerformRotation()
    {
        if(rotation != Vector3.zero)
         rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

        //thisTransform
        //       void PerformRotation()
        //{
        //    rb.transform.localRotation = Quaternion.Euler(rotation);
        //    //thisTransform.transform.localRotation = Quaternion.Euler(rotation);

      // Vector3 rot = this.transform.localRotation.eulerAngles;
       
    //}
    }

    void OnCollisionStay()
    {
        IsColliding = true;
    }







}
