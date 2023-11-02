using UnityEngine;
using ElectroBuilder.Testmode;

public class Player : MonoBehaviour
{
    //Basic Movment Values
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 20f;
    private float horizontal;

    //Components
    private Rigidbody rb;

    //Gravity Scale
    public float gravityScale = 1.0f;
    public static float globalGravity = -9.81f;

    private void Start()
    {
        //Getting Rigidbody Component
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector3 (rb.velocity.x, jumpingPower);
        }
    }

    private void FixedUpdate()
    {
        //Movement: velocity calculation
        rb.velocity = new Vector3(horizontal * speed, rb.velocity.y);

        //Gravity Scale
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
