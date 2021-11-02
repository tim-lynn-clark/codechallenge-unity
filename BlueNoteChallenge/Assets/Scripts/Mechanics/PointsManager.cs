
namespace Assets.Scripts.Mechanics
{
    using TMPro;
    using UnityEngine;

    public class PointsManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI pointsText;

        [SerializeField]
        private int points;

        /// <summary>
        /// The Unity start.
        /// </summary>
        private void Start()
        {
            points = 0;
            pointsText.text = points.ToString();
        }

        /// <summary>
        /// Adds points to the point system.
        /// </summary>
        /// <param name="pointsToAdd">The points to add (can be negative).</param>
        /// <returns>Whether the points is greater than 0.</returns>
        public bool AddPoints(int pointsToAdd)
        {
            points += pointsToAdd;

            if (points < 0)
            {
                points = 0;
                return false;
            }

            pointsText.text = points.ToString();
            return true;
        }
    }
}