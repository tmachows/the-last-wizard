using UnityEngine;

namespace TheLastWizard {
    public abstract class InputSource : MonoBehaviour {
        [SerializeField] protected GestureInputReader gestureInputReader;
        [SerializeField] protected LineDrawer lineDrawer;

        public virtual void Initialize() {
            gameObject.SetActive(true);
        }
    }
}