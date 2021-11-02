namespace Platformer.Gameplay
{
    using Platformer.Core;
    using Platformer.Mechanics;

    /// <summary>
    /// The enemy hurt event.
    /// </summary>
    public class EnemyHurt : Simulation.Event<EnemyHurt>
    {
        /// <summary>
        /// Gets or sets the enemy controller.
        /// </summary>
        public EnemyController Enemy { get; set; }

        /// <summary>
        /// The execute implementation.
        /// </summary>
        public override void Execute()
        {
            Enemy.EnemyHurt();
        }
    }
}