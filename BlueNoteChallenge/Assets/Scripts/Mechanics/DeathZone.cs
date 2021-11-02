namespace Platformer.Mechanics
{
    using Platformer.Gameplay;
    using UnityEngine;
    using static Platformer.Core.Simulation;

    /// <summary>
    /// DeathZone components mark a collider which will schedule a
    /// PlayerEnteredDeathZone event when the player enters the trigger.
    /// </summary>
    public class DeathZone : MonoBehaviour
    {
        /// <summary>
        /// The zone id.
        /// </summary>
        [SerializeField]
        private int zoneId;

        /// <summary>
        /// The Unity OnTriggerEnter2D
        /// </summary>
        /// <param name="collider">The other collider.</param>
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                var p = collider.gameObject.GetComponent<PlayerController>();
                if (p != null)
                {
                    var ev = Schedule<PlayerEnteredDeathZone>();
                    ev.deathzone = this;
                }
            }
            else if (collider.CompareTag("Enemy"))
            {
                var e = collider.gameObject.GetComponent<EnemyController>();
                if (e != null)
                {
                    Schedule<EnemyEnteredDeathZone>().Enemy = e;
                }
            }
        }
    }
}