
namespace Platformer.Gameplay
{
    using Assets.Scripts.Mechanics;
    using Platformer.Core;

    public class AddPoints : Simulation.Event<AddPoints>
    {
        /// <summary>
        /// Gets or sets the points to add (can be negative).
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Gets or sets the player controller.
        /// </summary>
        public PointsManager pointsManager { get; set; }

        public override void Execute()
        {
            if (!pointsManager.AddPoints(Points))
            {
                Simulation.Schedule<PlayerDeath>();
            }
        }
    }
}