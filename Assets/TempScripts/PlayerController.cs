using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

	#region Fields
	
	private Rigidbody2D rb2D;

	[SerializeField] private float speed = 8f;

	private Vector2 currVelocity;
	private float currHorizontalSpeed;
	private bool isFacingLeft;
	
	// Jump support
	[SerializeField] private float jumpForce = 200;
	private bool isGrounded;

	#endregion


	#region Methods

	private void Awake()
	{
		rb2D = GetComponent<Rigidbody2D>();
		currVelocity = new Vector2();
	}
	
	private void Update() 
	{
		currVelocity.y = rb2D.velocity.y;
		currVelocity.x = currHorizontalSpeed;
		rb2D.velocity = currVelocity;
		
		if (rb2D.velocity.x > 0 && isFacingLeft) 
		{
			FlipSprite();
		}
		else if (rb2D.velocity.x < 0 && !isFacingLeft) 
		{
			FlipSprite();
		}
		
	}
	
	private void OnCollisionEnter2D(Collision2D coll2D) 
	{
		if (coll2D.gameObject.tag == "Ground") 
		{
			isGrounded = true;
		}
	}
	
	private void OnCollisionExit2D(Collision2D coll2D) 
	{
		if (coll2D.gameObject.tag == "Ground") 
		{
			isGrounded = false;
		}
	}
	
	private void FlipSprite() 
	{
		Vector3 currScale = gameObject.transform.localScale;
		currScale.x *= -1;
		gameObject.transform.localScale = currScale;
		
		isFacingLeft = !isFacingLeft;
	}

	#endregion


	#region Event Handlers

	public void OnMove(InputAction.CallbackContext context) 
	{
		currHorizontalSpeed = context.ReadValue<float>() * speed;
	}
	
	public void OnJump(InputAction.CallbackContext context) 
	{
		if (isGrounded) 
		{
			float inVal = context.ReadValue<float>();
			// Debug.Log(inVal);
			rb2D.AddForce(Vector2.up * jumpForce * inVal);
		}
	}

	#endregion
}
