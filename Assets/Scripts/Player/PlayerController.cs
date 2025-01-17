/*
 * Authors: Tony 
 */
using UnityEngine;

namespace Assets.Scripts.Player
{
    /// <summary>
    /// Main player controller, handles physics, movement and respawning.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        #region MovementVars
        /// <summary>
        /// Direction of the character
        /// </summary>
        private float horizontal;
        /// <summary>
        /// Speed of the character
        /// </summary>
        public float speed = 8f;
        /// <summary>
        /// Jumping force of the character
        /// </summary>
        public float jumpingPower = 26f;
        /// <summary>
        /// How fast the character accelerates
        /// </summary>
        public float acceleration = 7;
        /// <summary>
        /// How fast the character decelerates
        /// </summary>
        public float deceleration = 7;
        /// <summary>
        /// Controls overall acceleration and deceleration speed
        /// </summary>
        public float velPower = 0.9f;
        /// <summary>
        /// Amount of friction applied when not inputting while grounded
        /// </summary>
        public float frictionAmount = 0.2f;

        /// <summary>
        /// How much vertical velocity the jump pad applies to the player
        /// </summary>
        public float jumpPadPower = 50f;

        /// <summary>
        /// How long the character can get a jump in, after walking off an edge
        /// </summary>
        public float hangTime = 0.15f;
        /// <summary>
        /// Counts how long since the character was grounded
        /// </summary>
        private float hangCounter = 0.2f;

        /// <summary>
        /// Direction of the character
        /// </summary>
        private bool isFacingRight = true;

        /// <summary>
        /// How many additional jumps the player can make
        /// </summary>
        public int numberOfAdditionalJumps = 0;

        /// <summary>
        /// How mnay additional jumps the player has left
        /// </summary>
        public int jumpsLeft = 0;

        #endregion

        #region PhysicsVars
        /// <summary>
        /// Rigidbody belonging to character
        /// </summary>
        [SerializeField] private Rigidbody2D rb;
        /// <summary>
        /// GroundCheck object belonging to character
        /// </summary>
        [SerializeField] private Transform groundCheck;
        /// <summary>
        /// Ground layer
        /// </summary>
        [SerializeField] private LayerMask groundLayer;
        /// <summary>
        /// Jump pad layer
        /// </summary>
        [SerializeField] private LayerMask jumpLayer;
        #endregion

        #region MiscVars
        /// <summary>
        /// Respawn prefab of the character
        /// </summary>
        public GameObject respawnPrefab;
        /// <summary>
        /// Respawn point of the character
        /// </summary>
        public GameObject respawnPoint;
        /// <summary>
        /// Dynamic enemy layer for hitboxes
        /// </summary>
        public LayerMask dynamicEnemy;
        #endregion

        void Update()
        {
            // checking direction of character
            horizontal = Input.GetAxisRaw("Horizontal");

            CalcHangTime();
            
            // check for flip, and flip the sprite
            Flip();
            // TODO: change later to reflect proper level y positions
            if (rb.position.y <= -5)
            {
                Respawn();
            }
 
            Jump();
        }

        private void FixedUpdate()
        {
            // calculates the speed the user desires 
            float targetSpeed = horizontal * speed;
            // finds the difference in speed between the two
            float speedDif = targetSpeed - rb.velocity.x;
            // change the acceleration rate, depending on situation
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            // calculates final accleration to apply, with higher velocity values applying less, and vice versa
            // finally multiplies by sign to reapply direction
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            rb.AddForce(movement * Vector2.right);

            // artificial friction, checking if we're grounded and not inputting
            if (hangCounter == hangTime && horizontal == 0)
            {
                // check which one is lower, cur vel or friction amount
                float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));
                // sets to current movement direction
                amount *= Mathf.Sign(rb.velocity.x);
                // applies force against the current movement direction
                rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }

            // increase gravity when falling, to make it feel better
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = 9;
            }
            else
            {
                rb.gravityScale = 6;
            }

            // apply jump pad power if we are on one
            if (IsOnObject(jumpLayer))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPadPower);
            }
        }

        /// <summary>
        /// Checks if groundcheck is currently on top of an object
        /// </summary>
        /// <param name="layer">The layer of the object</param>
        /// <returns>A bool representing if they are on a object </returns>
        private bool IsOnObject(LayerMask layer)
        {
            return Physics2D.OverlapCircle(groundCheck.position, 0.2f, layer);
        }

        /// <summary>
        /// Checks if character is going to flip, and if so changes the sprite to reflect it
        /// </summary>
        private void Flip()
        {
            if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        /// <summary>
        /// Calculates how much hang time the player has
        /// </summary>
        private void CalcHangTime()
        {
            // checks if its grounded, and if so reset their hang counter and their jump counter
            if (IsOnObject(groundLayer))
            {
                hangCounter = hangTime;
                jumpsLeft = numberOfAdditionalJumps;
            }
            // otherwise, minus the elapsed time
            else
            {
                hangCounter -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Allows the player to jump, checking for hangtime as well as velocity
        /// </summary>
        private void Jump()
        {
            // allows them to jump if they are hanging, and don't already have a positive y velocity
            if (Input.GetButtonDown("Jump") && hangCounter > 0f && rb.velocity.y <= 0.3f)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            // mid air jumps
            else if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
            {
                jumpsLeft -= 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            // checking if they let go during their jump, if so, make them lose y velocity
            if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }

        /// <summary>
        /// Respawn character to its respawn point
        /// </summary>
        public void Respawn()
        {
            rb.position = respawnPoint.transform.position;
            rb.velocity = Vector2.zero;
        }
    }
}

// Citations:
// Bendux, �2D player movement in Unity,� YouTube, https://www.youtube.com/watch?v=K1xZ-rycYY8
// Dawnosaur, �Improve your platformer with forces | examples in unity,� YouTube, https://www.youtube.com/watch?v=KbtcEVCM7bw
// gamesplusjames, �5 tips for better platformers in Unity (with code)!,� YouTube, https://www.youtube.com/watch?v=8QPmhDYn6rk 
// �Unity documentation,� Unity Documentation, https://docs.unity.com/ 
