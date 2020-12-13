using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPun
{

    [SerializeField]
    private GameObject crown;

    private float speed = 7f;

    private float lookSensitivity = 5f;// 100f;
    [SerializeField]
    private GameObject leading_text;

    private int PrestigePoints = 0;

    //private KeyCode up = KeyCode.Z;
    //private KeyCode down = KeyCode.S;
    //private KeyCode left = KeyCode.Q;
    //private KeyCode right = KeyCode.D;
    private KeyCode up = KeyCode.W;
    private KeyCode down = KeyCode.S;
    private KeyCode left = KeyCode.A;
    private KeyCode right = KeyCode.D;


    [SerializeField]
    private Camera camera;

    // Component caching
    private PlayerMotor motor;

    private int me = 0;
    private float desiredX;

    //Dictionary<int, GameObject> Player_objects;

    private void Start()
    {

        motor = GetComponent<PlayerMotor>();

        if(ScoreMenu.Keyboard_variable == true)
        { 
            up = KeyCode.W;
            down = KeyCode.S;
            left = KeyCode.A;
            right = KeyCode.D;
        }
    }

    void Update()
    {
        //zorgt ervoor dat je enkel gaat veranderen als het jou player is
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

      
        //Calculate movement velocity as a 3D vector
        //float _xMov = Input.GetAxis("Horizontal");
        //float _zMov = Input.GetAxis("Vertical");

        //Vector3 _movHorizontal = transform.right * _xMov;
        //Vector3 _movVertical = transform.forward * _zMov;

        //Calculate movement velocity as a 3D vector
        bool _xMovRight = Input.GetKey(right) || Input.GetKey(KeyCode.RightArrow);
        bool _xMovLeft = Input.GetKey(left) || Input.GetKey(KeyCode.LeftArrow);

        bool _zMovUp = Input.GetKey(up) || Input.GetKey(KeyCode.UpArrow);
        bool _zMovDown = Input.GetKey(down) || Input.GetKey(KeyCode.DownArrow);

        Vector3 _movHorizontal = Vector3.zero;
        Vector3 _movVertical = Vector3.zero;

        if (_xMovRight == true)
        {
            _movHorizontal = transform.right;
        }
        else
        {
            if (_xMovLeft == true)
            {
                 _movHorizontal = -transform.right;
            }
        }

        if (_zMovUp == true)
        {
            _movVertical = transform.forward;
        }
        else
        {
            if (_zMovDown == true)
            {
                _movVertical = -transform.forward;
            }
        }



        // Get size input
        //bool _grow = Input.GetKey(KeyCode.R);
        //bool _shrink = Input.GetKey(KeyCode.E);
        //int _sizeChange = Convert.ToInt32(_grow) - Convert.ToInt32(_shrink);

        //Apply size change
        //motor.ChangeSize(_sizeChange);


        // Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical) * speed;



        //Apply movement
        motor.Move(_velocity);

        ////////////////////////////////////////////////////////////////////////other try////////:
        ////Calculate rotation as a 3D vector (turning around)
        //float _yRot = Input.GetAxis("Mouse X") * lookSensitivity;//* Time.fixedDeltaTime; 

       

        ////Find current look rotation
        
        //Vector3 _rotation = new Vector3(0f, _yRot, 0f);
        ///////////////////////////////////////////////////////////////////////////////////////////

        //Calculate rotation as a 3D vector (turning around)
        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        //Apply rotation
        motor.Rotate(_rotation);


        //Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity * Time.fixedDeltaTime;

        //Apply rotation
        motor.Rotate(_rotation);
    }
}