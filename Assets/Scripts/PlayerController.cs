using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rigidBody;
    public float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float xVel = Input.GetAxis("Horizontal");
        float yVel = Input.GetAxis("Vertical");

        Vector2 moveVector = new Vector2(xVel, yVel).normalized;
        rigidBody.linearVelocity = moveVector * moveSpeed * Time.deltaTime;
        //gameObject.transform.position += moveVector * moveSpeed * Time.deltaTime;
    }
}
