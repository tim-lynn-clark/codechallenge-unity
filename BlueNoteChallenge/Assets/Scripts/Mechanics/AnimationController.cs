namespace Platformer.Mechanics
{
    using Platformer.Core;
    using Platformer.Model;
    using UnityEngine;

    /// <summary>
    /// AnimationController integrates physics and animation. It is generally used for simple enemy animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class AnimationController : KinematicObject
    {
        #region Inspector

        /// <summary>
        /// Max horizontal speed.
        /// </summary>
        [SerializeField]
        private float maxSpeed = 7;

        /// <summary>
        /// Max jump velocity
        /// </summary>
        [SerializeField]
        private float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Gets or sets the move value
        /// </summary>
        public Vector2 Move;

        /// <summary>
        /// Set to true to initiate a jump.
        /// </summary>
        [SerializeField]
        private bool jump;

        /// <summary>
        /// Set to true to set the current jump velocity to zero.
        /// </summary>
        [SerializeField]
        private bool stopJump;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the max speed.
        /// </summary>
        public float MaxSpeed
        {
            get { return maxSpeed; }
        }

        /// <summary>
        /// The stop physics flag.
        /// </summary>
        public bool StopPhysics
        {
            get; set;
        }

        #endregion

        #region Variables

        /// <summary>
        /// The sprite renderer.
        /// </summary>
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// The animator.
        /// </summary>
        private Animator animator;

        /// <summary>
        /// The platformer model.
        /// </summary>
        private PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        #endregion

        #region Unity Methods

        /// <summary>
        /// The Unity Awake.
        /// </summary>
        protected virtual void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        #endregion

        #region Overrides

        protected override void FixedUpdate()
        {
            if (StopPhysics)
            {
                return;
            }

            base.FixedUpdate();
        }

        /// <summary>
        /// Overrides the computer velocity.
        /// Computes the velocity of the enemy object.
        /// </summary>
        protected override void ComputeVelocity()
        {
            if (jump && IsGrounded)
            {
                velocity.y = jumpTakeOffSpeed * model.jumpModifier;
                jump = false;
            }
            else if (stopJump)
            {
                stopJump = false;
                if (velocity.y > 0)
                {
                    velocity.y = velocity.y * model.jumpDeceleration;
                }
            }

            if (Move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (Move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = Move * maxSpeed;
        }

        #endregion
    }
}