using System.Collections;
using System.Collections.Generic;
using System.Timers;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

	#region Fields
	
	private Rigidbody2D rb2D;

	// Movement support
	[SerializeField] private float speed = 8f;
	private Vector2 currVelocity;
	private float currHorizontalSpeed;
	private bool isFacingLeft;
	
	// Jump support
	[SerializeField] private float jumpForce = 200f;
	private bool isGrounded;
	
	// Dash support
	[SerializeField] private float dashForce = 3000f;
	private bool isDashAvailable = true;
	private float dashCooldown = 1.2f;

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
		
		if (!isDashAvailable) 
		{
			dashCooldown -= Time.deltaTime;
		}
		if (dashCooldown <= 0) 
		{
			isDashAvailable = true;
			dashCooldown = 2f;
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
			rb2D.AddForce(Vector2.up * jumpForce);
		}
	}
	
	public void OnDash(InputAction.CallbackContext context) 
	{
		if (isDashAvailable) 
		{
			Vector2 forceToAdd = isFacingLeft 
				? Vector2.left * dashForce
				: Vector2.right * dashForce; 
			rb2D.AddForce(forceToAdd);
			isDashAvailable = false;
		}
	}

	#endregion
}
