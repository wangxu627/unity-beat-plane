/*! \cond PRIVATE */

using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Combat/Click To Kill Or Damage 2D")]
    // ReSharper disable once CheckNamespace
    public class ClickToKillOrDamage2D : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        public bool killObjects = true;
        public int damagePointsToInflict = 1;
        // ReSharper restore InconsistentNaming

        // ReSharper disable once UnusedMember.Local
        private void Update() {
            var mouseDown = Input.GetMouseButtonDown(0);
            var fingerDown = Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began;

            if (!mouseDown && !fingerDown) {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var hit2D = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit2D.collider != null) {
                KillOrDamage(hit2D.collider.gameObject);
            }
        }

        private void KillOrDamage(GameObject go) {
            var kill = go.GetComponent<Killable>();
            if (kill == null) {
                return;
            }

            if (killObjects) {
                kill.DestroyKillable();
            } else {
                kill.TakeDamage(damagePointsToInflict, null);
            }
        }
    }
}

/*! \endcond */