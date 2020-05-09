using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
	public float moveTime = 0.1f;
	private bool currentlyMoving;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;

	void Start()
    {
		currentlyMoving = false;
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
	}

    void Update()
    {
		if (currentlyMoving)
		{
			return;
		}
		currentlyMoving = true;

		float horizontal = 0;
		float vertical = 0;

		horizontal = Input.GetAxisRaw("Horizontal");
		vertical = Input.GetAxisRaw("Vertical");

		if (horizontal != 0)
		{
			vertical = 0;
		}

		RaycastHit2D hit;

		Move(horizontal, vertical, out hit);
	}

	protected void Move(float xDir, float yDir, out RaycastHit2D hit)
	{
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xDir, yDir);

		boxCollider.enabled = false;
		hit = Physics2D.Linecast(start, end, blockingLayer);
		boxCollider.enabled = true;

		if(hit.transform == null)
		{ 
			StartCoroutine(SmoothMovement(end));
			return;
		}
		currentlyMoving = false;
	}

	protected IEnumerator SmoothMovement(Vector3 end)
	{
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		currentlyMoving = false;
	}
}
