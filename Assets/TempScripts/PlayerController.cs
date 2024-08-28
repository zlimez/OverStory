using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

	#region Fields

	[SerializeField] private float speed = 8f;

	private Vector2 currVelocity;
	private float currHorizontalSpeed;
	private Rigidbody2D rb2D;

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
	}

	#endregion


	#region Event Handlers

	public void OnMove(InputAction.CallbackContext context) 
	{
		currHorizontalSpeed = context.ReadValue<float>() * speed;
	}

	#endregion
}
