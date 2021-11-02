namespace Platformer.Gameplay
{
    using Platformer.Core;
    using Platformer.Model;

    public class PlayerHurt : Simulation.Event<PlayerHurt>
    {
        /// <summary>
        /// The platformer model.
        /// </summary>
        PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        /// <summary>
        /// The execute implementation.
        /// </summary>
        public override void Execute()
        {
            var player = model.player;

            if (player != null)
            {
                player.Hurt();
            }
        }
    }
}