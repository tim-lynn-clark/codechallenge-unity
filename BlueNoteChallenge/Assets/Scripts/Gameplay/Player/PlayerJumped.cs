namespace Platformer.Gameplay
{
    using Platformer.Core;
    using Platformer.Mechanics;

    /// <summary>
    /// Fired when the player performs a Jump.
    /// </summary>
    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
        /// <summary>
        /// Gets or sets the player controller.
        /// </summary>
        public PlayerController Player { get; set; }

        /// <summary>
        /// The execute implementation.
        /// </summary>
        public override void Execute()
        {
            if (Player != null)
            {
                Player.Jump();
            }
        }
    }
}