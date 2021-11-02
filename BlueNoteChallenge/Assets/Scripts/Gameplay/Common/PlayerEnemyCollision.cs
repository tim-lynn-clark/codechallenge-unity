namespace Platformer.Gameplay
{
    using Platformer.Core;
    using Platformer.Mechanics;
    using Platformer.Model;
    using static Platformer.Core.Simulation;

    /// <summary>
    /// Fired when a Player collides with an Enemy.
    /// </summary>
    /// <typeparam name="EnemyCollision"></typeparam>
    public class PlayerEnemyCollision : Simulation.Event<PlayerEnemyCollision>
    {
        /// <summary>
        /// Gets or sets the enemy controller.
        /// </summary>
        public EnemyController Enemy { get; set; }

        /// <summary>
        /// Gets or sets the player controller.
        /// </summary>
        public PlayerController Player { get; set; }

        /// <summary>
        /// The platformer model.
        /// </summary>
        private PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        /// <summary>
        /// The execute implementation.
        /// </summary>
        public override void Execute()
        {
            var willHurtEnemy = Player.Bounds.center.y >= Enemy.Bounds.max.y;

            if (willHurtEnemy)
            {
                var enemyHealth = Enemy.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.Decrement();
                    if (!enemyHealth.IsAlive)
                    {
                        Schedule<EnemyDeath>().enemy = Enemy;
                        AddPoints(Enemy.PointsAward);
                        Player.Bounce(2);
                    }
                    else
                    {
                        Schedule<EnemyHurt>().Enemy = Enemy;
                        Player.Bounce(7);
                    }
                }
                else
                {
                    Schedule<EnemyDeath>().enemy = Enemy;
                    AddPoints(Enemy.PointsAward);
                    Player.Bounce(2);
                }
            }
            else
            {
                AddPoints(-1);
                Schedule<PlayerHurt>();
            }
        }

        /// <summary>
        /// The add points.
        /// </summary>
        /// <param name="points">The points to add (can be negative).</param>
        private void AddPoints(int points)
        {
            var ev = Schedule<AddPoints>();
            ev.pointsManager = model.pointsManager;
            ev.Points = points;
        }
    }
}