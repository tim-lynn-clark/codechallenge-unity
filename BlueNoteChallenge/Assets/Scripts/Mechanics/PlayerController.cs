namespace Platformer.Mechanics
{
    using System.Collections;
    using UnityEngine;
    using Platformer.Gameplay;
    using static Platformer.Core.Simulation;
    using Platformer.Model;
    using Platformer.Core;

    /// <summary>
    /// The player controller.
    /// Controls player animations and audio events.
    /// </summary>
    public class PlayerController : KinematicObject
    {
        #region Animator Hashes

        /// <summary>
        /// The hurt animator hash.
        /// </summary>
        private int hurtHash = Animator.StringToHash("hurt");

        /// <summary>
        /// The victory animator hash.
        /// </summary>
        private int victoryHash = Animator.StringToHash("victory");

        /// <summary>
        /// The hurt tag.
        /// </summary>
        private string hurtTag = "hurt";

        #endregion 

        #region Inspector

        #region Audio

        /// <summary>
        /// The audio source.
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        /// <summary>
        /// The jump audio clip.
        /// </summary>
        [SerializeField]
        private AudioClip jumpAudio;

        /// <summary>
        /// The respawn audio clip.
        /// </summary>
        [SerializeField]
        private AudioClip respawnAudio;

        /// <summary>
        /// The ouch audio clip.
        /// </summary>
        [SerializeField]
        private AudioClip ouchAudio;

        #endregion

        /// <summary>
        /// Max horizontal speed of the player.
        /// </summary>
        [SerializeField]
        private float maxSpeed = 7;

        /// <summary>
        /// Initial jump velocity at the start of a jump.
        /// </summary>
        [SerializeField]
        private float jumpTakeOffSpeed = 7;

        /// <summary>
        /// Jump state of the player.
        /// </summary>
        [SerializeField]
        private JumpState jumpState = JumpState.Grounded;

        /// <summary>
        /// The collider 2d.
        /// </summary>
        [SerializeField]
        private Collider2D collider2d;

        /// <summary>
        /// The health.
        /// </summary>
        [SerializeField]
        private Health health;

        /// <summary>
        /// Control enabled flag.
        /// </summary>
        [SerializeField]
        private bool controlEnabled = true;

        /// <summary>
        /// The player jump count. The number of times the player can jump.
        /// </summary>
        [SerializeField]
        private int maxJumpCount = 2;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the max speed.
        /// </summary>
        public float MaxSpeed
        {
            get { return maxSpeed; }
            set { maxSpeed = value; }
        }

        /// <summary>
        /// Gets or sets the invincible flag.
        /// </summary>
        public bool IsInvincible 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the collider 2d bounds
        /// </summary>
        public Bounds Bounds
        {
            get { return collider2d.bounds; }
        }

        public Health Health
        {
            get { return health; }
        }

        /// <summary>
        /// The jump state enum.
        /// </summary>
        public enum JumpState
        {
            Grounded,
            PrepareToJump,
            Jumping,
            InFlight,
            Landed
        }

        #endregion

        #region Variables

        /// <summary>
        /// The platformer model.
        /// </summary>
        private readonly PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        /// <summary>
        /// The stop jump flag.
        /// </summary>

        private bool stopJump;

        /// <summary>
        /// The jump count.
        /// </summary>
        private int jumpCount;

        /// <summary>
        /// The jump flag.
        /// </summary>
        private bool jump;

        /// <summary>
        /// The move vector 2.
        /// </summary>
        private Vector2 move;

        /// <summary>
        /// The sprite renderer.
        /// </summary>
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// The animator.
        /// </summary>
        private Animator animator;

        #endregion

        #region Unity Methods

        /// <summary>
        /// The Unity Awake.
        /// </summary>
        private void Awake()
        {
            health = GetComponent<Health>();
            audioSource = GetComponent<AudioSource>();
            collider2d = GetComponent<Collider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// The Unity Start.
        /// </summary>
        protected override void Start()
        {
            base.Start();

            jumpCount = maxJumpCount;
            IsInvincible = false;
        }

        /// <summary>
        /// Overrides the Unity update.
        /// </summary>
        protected override void Update()
        {
            if (controlEnabled)
            {
                move.x = Input.GetAxis("Horizontal");
                if (jumpCount > 0 && Input.GetButtonDown("Jump"))
                    jumpState = JumpState.PrepareToJump;
            }
            else
            {
                move.x = 0;
            }
            UpdateJumpState();
            base.Update();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The update jump state.
        /// Controls jump state logic statemachine.
        /// </summary>
        private void UpdateJumpState()
        {
            jump = false;
            switch (jumpState)
            {
                case JumpState.PrepareToJump:
                    jumpState = JumpState.Jumping;
                    jump = true;
                    stopJump = false;
                    jumpCount--;
                    break;
                case JumpState.Jumping:
                    if (!IsGrounded)
                    {
                        Schedule<PlayerJumped>().Player = this;
                        jumpState = JumpState.InFlight;
                    }
                    break;
                case JumpState.InFlight:
                    if (IsGrounded)
                    {
                        Schedule<PlayerLanded>().player = this;
                        jumpState = JumpState.Landed;
                    }
                    break;
                case JumpState.Landed:
                    jumpState = JumpState.Grounded;
                    jumpCount = maxJumpCount;
                    break;
            }
        }

        /// <summary>
        /// Computes player velocity.
        /// </summary>
        protected override void ComputeVelocity()
        {
            if (jump && jumpCount < maxJumpCount)
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

            if (move.x > 0.01f)
                spriteRenderer.flipX = false;
            else if (move.x < -0.01f)
                spriteRenderer.flipX = true;

            animator.SetBool("grounded", IsGrounded);
            animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

            targetVelocity = move * maxSpeed;
        }

        /// <summary>
        /// Sets the spawn state.
        /// </summary>
        public void SetSpawnState()
        {
            collider2d.enabled = true;
            controlEnabled = false;
            if (audioSource && respawnAudio)
            {
                audioSource.PlayOneShot(respawnAudio);
            }
            health.Increment();
            Teleport(model.spawnPoint.transform.position);
            jumpState = JumpState.Grounded;
            animator.SetBool("dead", false);
            jumpCount = maxJumpCount;
            IsInvincible = false;
        }

        /// <summary>
        /// Sets the player's invulnerability frames.
        /// </summary>
        public void SetIFrames()
        {
            StartCoroutine(DoPlayerIFrames());
        }

        /// <summary>
        /// Performs the invulnerability frames to prevent player from getting damaged during hurt.
        /// </summary>
        private IEnumerator DoPlayerIFrames()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(hurtTag));

            controlEnabled = false;
            IsInvincible = true;

            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(hurtTag));

            if (animator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
            {
                controlEnabled = true;

                IsInvincible = false;
            }
        }

        /// <summary>
        /// The hurt method.
        /// Performs hurt audio and animator.
        /// Sets invulnerability frames.
        /// </summary>
        public void Hurt()
        {
            if (audioSource && ouchAudio)
            {
                audioSource.PlayOneShot(ouchAudio);
            }
            animator.SetTrigger(hurtHash);
            SetIFrames();
        }

        /// <summary>
        /// The jump method.
        /// Plays the jump audio.
        /// </summary>
        public void Jump()
        {
            if (audioSource != null && jumpAudio != null)
            {
                audioSource.PlayOneShot(jumpAudio);
            }
        }

        /// <summary>
        /// The victory method.
        /// </summary>
        public void Victory()
        {
            animator.SetTrigger(victoryHash);
            controlEnabled = false;
        }

        /// <summary>
        /// The enable player control.
        /// </summary>
        /// <param name="enabled"></param>
        public void EnablePlayerControl(bool enabled)
        {
            controlEnabled = enabled;
        }

        /// <summary>
        /// The player death.
        /// </summary>
        public void PlayerDeath()
        {
            EnablePlayerControl(false);
            animator.SetBool("dead", true);
        }

        #endregion
    }
}