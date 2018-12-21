using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    private CharacterMover playerMover;
    [SerializeField]
    private float xMovementf = 0.0f, yMovementf = 0.0f;


    // Use this for initialization
    void Start()
    {
        playerMover = GetComponent<CharacterMover>();
    }



    // Update is for scanning for inputs
    void Update()
    {

        xMovementf = Input.GetAxisRaw("Horizontal");
        yMovementf = Input.GetAxisRaw("Vertical");

    }

    // FixedUpdate is for passing inputs along to the controller script. 
    private void FixedUpdate()
    {

        playerMover.MoveCharacter(xMovementf, yMovementf);
    }


}
