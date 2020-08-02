using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarwinsController : MonoBehaviour
{
    public KeyCode walkLeft;
    public KeyCode walkRight;
    public KeyCode Jump;
    private Rigidbody2D curRGB;
    public float speed = 10.0f;


    // Start is called before the first frame update
    void Start()
    {
        this.curRGB = GetComponent<Rigidbody2D>(); ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Vertical") != 0.0f)
        {
            float moveDirection = 0;
            moveDirection = Input.GetAxis("Vertical") * this.speed;
            this.curRGB.AddRelativeForce(new Vector2(0, moveDirection));
        }

        if (Input.GetAxis("Horizontal") != 0.0f)
        {
            float jumpDirection = 0;
            jumpDirection = Input.GetAxis("Horizontal") * this.speed;
            this.curRGB.AddRelativeForce(new Vector2(jumpDirection, 0));

        }

    }
}
