namespace Platformer.Gameplay
{
    using Platformer.Core;
    using Platformer.Mechanics;

    /// <summary>
    /// The enemy entered death zone simulation event.
    /// </summary>
    public class EnemyEnteredDeathZone : Simulation.Event<EnemyEnteredDeathZone>
    {
        /// <summary>
        /// Gets or sets the enemy controller.
        /// </summary>
        public EnemyController Enemy { get; set; }

        /// <summary>
        /// Executes the simulation task.
        /// </summary>
        public override void Execute()
        {
            Enemy.Despawn();
        }
    }
}