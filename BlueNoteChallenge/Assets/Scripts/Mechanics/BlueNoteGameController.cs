namespace Assets.Scripts.Mechanics
{
    using Platformer.Core;
    using Platformer.Model;
    using UnityEngine;

    public class BlueNoteGameController : MonoBehaviour
    {
        /// <summary>
        /// Gets or set the blue note game controller instance.
        /// </summary>
        public static BlueNoteGameController Instance { get; private set; }

        /// <summary>The platformer model.</summary>
        [SerializeField]
        private PlatformerModel Model = Simulation.GetModel<PlatformerModel>();

        /// <summary>
        /// The Unity On Enable.
        /// Initializes the instance to this game controller.
        /// </summary>
        private void OnEnable()
        {
            Instance = this;
        }

        /// <summary>
        /// The Unity on disable.
        /// Destroys the instance of the game controller singleton.
        /// </summary>
        private void OnDisable()
        {
            if (Instance == this)
            {
                Destroy(this);
            }
        }

        /// <summary>
        /// The Unity update.
        /// </summary>
        private void Update()
        {
            if (Instance == this)
            {
                Simulation.Tick();
            }
        }
    }
}