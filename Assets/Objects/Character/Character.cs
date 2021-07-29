using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Rigidbody2D characterRigidBody;
    private const float characterVelocity = 2f;

    private bool side = true; // Right

    private CharacterAnimationController characterAnimator;

    private bool isGrounded = false;
    private bool isRigibodyFrozen =  false;
    private Transform groundChecker;
    private LayerMask groundLayerMask;

    // private bool hasJump = false;
    private float jumpForce = 200f;

    // Start is called before the first frame update
    private void Awake()
	{
		characterRigidBody = GetComponent<Rigidbody2D>();
		characterAnimator = GetComponent<CharacterAnimationController>();
        groundChecker = transform.Find("GroundChecker").GetComponent<Transform>();
        groundLayerMask = LayerMask.GetMask("ground");
    }

    // Update is called once per frame
    void Update() {
        HorizontalMovementCheck();
        VerticalMovementCheck();
    }

    void HorizontalMovementCheck() {
        float axisRaw = Input.GetAxisRaw("Horizontal");

        if (axisRaw != 0) {
            if ((axisRaw == 1) && (!side)) {
                FlipAxis();
            } if ((axisRaw == -1) && (side)) {
                FlipAxis();
            }

            Move(Input.GetAxis("Horizontal"));
            characterAnimator.AnimateWalk();
        } else if (Input.GetButtonUp("Horizontal")) {
            Move(0f);
            characterAnimator.AnimateIddle();
        }
    }

    void VerticalMovementCheck() {
        Vector2 groundCheckerPosition = new Vector2(groundChecker.position.x, groundChecker.position.y);
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckerPosition, 0.05f, groundLayerMask);

        bool isInAir = true;
        bool shouldFreeze = false;
        bool isJump = false;

        // Debug.Log(colliders.Length);
        string colliderNames = "";

        foreach (Collider2D collider in colliders) {
            GameObject currentEntry = collider.gameObject;

            colliderNames += currentEntry.name + " | ";

            if ((currentEntry.tag == "ground") || (currentEntry.tag == "ground-diagonal")) {
                isInAir = false;
            }

            if ((currentEntry.tag == "ground-diagonal") && (Input.GetAxisRaw("Horizontal") == 0)) {
                shouldFreeze = true;
            }
        }

        if (isGrounded != !isInAir) {
            isGrounded = !isInAir;
        }

        if ((Input.GetButtonDown("Jump")) && (isGrounded)) {
            isGrounded = false;
            shouldFreeze = false;
            isJump = true;
        }

        FreezeRigidBody(shouldFreeze);

        if ((isJump) && (characterRigidBody.velocity.y == 0)) {
            characterRigidBody.AddForce(new Vector2(0f, jumpForce));
        }

    }

    void FlipAxis() {
        side = !side;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void Move(float movement) {
        Vector3 targetVelocity = new Vector2(movement * characterVelocity, characterRigidBody.velocity.y);
        characterRigidBody.velocity = targetVelocity;
    }

    void FreezeRigidBody(bool shouldFreeze) {
        if (isRigibodyFrozen != shouldFreeze) {
            isRigibodyFrozen = shouldFreeze;
            if (shouldFreeze) {
                if (characterRigidBody.velocity.y > 0) {
                    characterRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                } else {
                    characterRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                }
            } else {
                characterRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }
    }
}
