namespace Platformer.Mechanics
{
    using System.Collections;
    using Platformer.Gameplay;
    using UnityEngine;
    using static Platformer.Core.Simulation;

    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        #region Inspector

        /// <summary>
        /// The patrol path.
        /// </summary>
        [SerializeField]
        private PatrolPath path;

        /// <summary>
        /// The "ouch" audio clip.
        /// </summary>
        [SerializeField]
        private AudioClip ouch;

        /// <summary>
        /// The "bounce" audio clip.
        /// </summary>
        [SerializeField]
        private AudioClip bounce;

        /// <summary>
        /// The animator.
        /// </summary>
        [SerializeField]
        private Animator animator;

        /// <summary>
        /// The health.
        /// </summary>
        [SerializeField]
        private Health health;

        /// <summary>
        /// The enemy scriptable object.
        /// </summary>
        [SerializeField]
        private EnemySO enemySO;

        #endregion

        #region Animator Hashes

        /// <summary>
        /// The death animator hash.
        /// </summary>
        private int deathHash = Animator.StringToHash("death");

        /// <summary>
        /// The death animation tag.
        /// </summary>
        private string deathTag = "death";

        /// <summary>
        /// The hurt animator hash.
        /// </summary>
        private int hurtHash = Animator.StringToHash("hurt");

        /// <summary>
        /// The hurt animation tag.
        /// </summary>
        private string hurtTag = "hurt";

        #endregion

        #region Variables

        /// <summary>
        /// The patrol path mover.
        /// </summary>
        private PatrolPath.Mover mover;

        /// <summary>
        /// The animation controller.
        /// </summary>
        private AnimationController control;

        /// <summary>
        /// The 2d collider.
        /// </summary>
        private Collider2D enemyCollider;

        /// <summary>
        /// The audio source.
        /// </summary>
        private AudioSource audioSource;

        /// <summary>
        /// The sprite renderer.
        /// </summary>
        private SpriteRenderer spriteRenderer;

        /// <summary>
        /// The should move flag.
        /// </summary>
        private bool shouldMove;

        #endregion

        #region Accessors

        /// <summary>
        /// Gets or sets the points awarded.
        /// </summary>
        public int PointsAward
        {
            get; private set;
        }

        /// <summary>
        /// Gets the bounds of the collider.
        /// </summary>
        public Bounds Bounds => enemyCollider.bounds;

        #endregion

        #region Unity Methods

        /// <summary>
        /// The Unity awake.
        /// </summary>
        private void Awake()
        {
            control = GetComponent<AnimationController>();
            enemyCollider = GetComponent<Collider2D>();
            audioSource = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// The Unity start.
        /// </summary>
        private void Start()
        {
            if (health != null)
            {
                health.maxHP = enemySO.HitPoints;
            }
            PointsAward = enemySO.PointAward;
            shouldMove = true;
        }

        /// <summary>
        /// The Unity update.
        /// </summary>
        private void Update()
        {
            if (path != null && shouldMove)
            {
                if (mover == null)
                { 
                    mover = path.CreateMover(control.MaxSpeed * 0.5f); 
                }
                control.Move.Set(Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1), control.Move.y);
            }
        }

        /// <summary>
        /// The Unity OnCollisionEnter2D.
        /// </summary>
        /// <param name="collision">The other collider.</param>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && !player.IsInvincible)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.Player = player;
                ev.Enemy = this;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The enemy hurt.
        /// Enemy hurt related logic.
        /// </summary>
        public void EnemyHurt()
        {
            shouldMove = false;
            animator.SetTrigger(hurtHash);
            StartCoroutine(DoEnemyHurt());
        }

        /// <summary>
        /// The do enmey hurt coroutine.
        /// Perform the enemy hurt animation and audio.
        /// </summary>
        private IEnumerator DoEnemyHurt()
        {
            if (audioSource != null && bounce != null)
            {
                audioSource.PlayOneShot(bounce);
            }

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(hurtTag));
            yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(hurtTag));

            shouldMove = true;
        }

        /// <summary>
        /// The enemy die logic.
        /// </summary>
        public void Die()
        {
            shouldMove = false;
            gameObject.layer = LayerMask.NameToLayer("Ground");
            control.StopPhysics = true;

            StartCoroutine(DoEnemyDeath());
        }

        /// <summary>
        /// Performs the enemy death animations and audio.
        /// </summary>
        private IEnumerator DoEnemyDeath()
        {
            if (audioSource && ouch)
            {
                audioSource.PlayOneShot(ouch);
            }
            animator.SetTrigger(deathHash);

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsTag(deathTag));
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

            Despawn();
        }

        /// <summary>
        /// Despawns the game object.
        /// </summary>
        public void Despawn()
        {
            if (this != null)
            {
                if (path != null)
                {
                    Destroy(path.gameObject);
                }
                Destroy(gameObject);
            }
        }

        #endregion
    }
}