using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TheLastWizard
{
    public class HudView : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField]
        private Text _ScoreText;
        [SerializeField]
        private Text _HealthText;
        [SerializeField]
        private Text _KilledEnemies;
        #endregion Inspector Variables

        #region Public Variables
        public int Score
        {
            set
            {
                _ScoreText.text = "Score: " + value;
            }
        }
        public float Health
        {
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                _HealthText.text = "Health: " + (int)value + "%";
            }
        }

        public int KilledEnemies
        {
            set
            {
                _KilledEnemies.text = "Killed enemies: " + value;
            }
        }
        #endregion Public Variables
    }
}
