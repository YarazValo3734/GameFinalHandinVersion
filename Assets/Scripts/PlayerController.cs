//These here are called libararies, which use pre-existing methods/code to basically make things work
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Adding the UI library to access and update the text for coins on the canvas
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //You should assign your variables at the top before any methods
    //Variables can be considered as containers that store specific data
    //There's a lot of different types of varaibles, most basic one are:
    //float - stores accurate numbers/numbers with a decimal point (used for movement etc.)
    //int - stores numbers without a decimal (could be used to store data for things like currency or player health)
    //bool - states if something is true or false (used for 'if statements' to determine if something should happen depending on ... so for example to check if player is grounded)
    //string - stores information/text (could be used for creating dialogue, or pretty much any text you want to be displayed in the game)

    //To set a variable you have to state which one you want to use, and then name it.      Example:     float currency;
    //before stating a variable you could call it public or private. public allows you to see the variable inside the engine and edit it, and private makes it not accessible from other codes

    [Header("Movement")]
    public float playerSpeed;

    public float movementInputDirection;    
    public float movementSpeed;
    public float jumpForce;
    private float fallMultiplier = 2.5f;
    private float lowJumpMultiplier = 10f;

    [Header("Checks")]
    public bool isFacingRight;
    public bool canJump;

    public bool isGrounded;
    public Transform groundCheck;
    public float groundCheckDistance;
    public LayerMask whatIsGround;

    [Header("Checks")]
    public int coinsCollected;
    public Text coinsCollectedTxt;

    [Header("Components")]
    public Rigidbody2D rb;
    public GameObject onFlip;
    public Animator anim;

    public Transform spawnPoint;

    private int facingDirection = 1;


    // Start is called before the first frame update
    void Start()
    {
        //Code inside of here will run only once you press play
        rb = GetComponent<Rigidbody2D>();
        
        //Making sure the player is facing right on start
        isFacingRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Setting things in the Update method will be checking for code every frame
        CheckInput();
        ApplyMovement();
        CheckSurroundings();
        CheckMovementDirection();
        BetterJump();
        UpdateAnimations();

        //Update the value of coins collected to the text on UI
        coinsCollectedTxt.text = "Coins: " + coinsCollected.ToString();
    }









    //Methods outside of the existing libraries (ones you create yourself, meaning they will not do anything if not put inside of Start or Update method)

    //CheckInput method
    private void CheckInput()
    {
        //Setting movementInputDirection variable as Input recieved from set "Horizontal" in Input System
        //There can either be 1 or -1 depending on input you recieve from the horizontal axis (left or right input)
        movementInputDirection = Input.GetAxisRaw("Horizontal");

        //If given "Jump" Input...
        if (Input.GetButtonDown("Jump"))
        {
            //...Activate Jump method
            Jump();
        }

        //Making canJump variable equal to isGrounded variable (so if you're grounded, it will also mean you can jump) if one is true, the other will be set as true
        canJump = isGrounded;
    }

    //Applying movement according to the input using movementInputDirection variable (again, could be 1 or -1, meaning left or right)
    //Look at Jump method for better explanation
    private void ApplyMovement()
    {
        if (movementInputDirection > 0.001f)
        {
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }
        else if (movementInputDirection < -0.001f)
        {
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }
        else if (movementInputDirection == 0f)
        {
            rb.velocity = new Vector2(movementSpeed * movementInputDirection, rb.velocity.y);
        }
    }

    //Checking which direction the player is facing
    private void CheckMovementDirection()
    {
        if (isFacingRight && movementInputDirection < 0)
        {
            Flip();
        }
        else if (!isFacingRight && movementInputDirection > 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        isFacingRight = !isFacingRight;
        onFlip.transform.Rotate(0.0f, 180.0f, 0.0f);
    }










    //Jump method that alters players Rigidbody component, increasing your y axis velocity (vertical) and makes the player go up, basically creates a jump
    private void Jump()
    {
        //So if you can jump (if isGrounded)
        if (canJump)
        {
            //Players Rigidbody velocity is now equal to the new set velocity for y axis (using jumpForce variable)
            //It's only chaning y axis as in brackets we're using Rigidbody's original x axis velocity (rb.velocity.x) and new y axis velocity (jumpForce)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    //Don't worry about this part, this is a litte bit more advance
    //This method creates a better jump, and creates a possiblity of reaching higher if you hold the jump button, so if you jsut tap the jump button it won't jump as high
    private void BetterJump()
    {
        if (!isGrounded)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }







    //Setting up animations
    private void UpdateAnimations()
    {
        playerSpeed = rb.velocity.x;
        anim.SetFloat("Speed", playerSpeed);
        
        anim.SetBool("isGrounded", isGrounded);

        if (movementInputDirection == 0)
        {
            anim.SetBool("isWalking", false);
        }
        else if ((movementInputDirection < 0 || movementInputDirection > 0))
        {
            anim.SetBool("isWalking", true);
        }
    }





    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Spikes")
        {
            transform.position = spawnPoint.position;
        }

        if (other.tag == "Collectable")
        {
            coinsCollected++;
            Destroy(other.gameObject);
        }
    }





    //Creating groundCheck sphere
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
    }
}
