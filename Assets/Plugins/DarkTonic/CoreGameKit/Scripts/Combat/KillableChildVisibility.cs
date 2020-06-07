/*! \cond PRIVATE */
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace DarkTonic.CoreGameKit {
    [AddComponentMenu("Dark Tonic/Core GameKit/Combat/Killable Child Visibility")]
    // ReSharper disable once CheckNamespace
    public class KillableChildVisibility : MonoBehaviour {
        // ReSharper disable InconsistentNaming
        public Killable killableWithRenderer;
        // ReSharper restore InconsistentNaming

        private bool _isValid = true;

        private Killable KillableToAlert {
            get {
                if (killableWithRenderer != null) {
                    return killableWithRenderer;
                }

                if (transform.parent != null) {
                    var parentKill = transform.parent.GetComponent<Killable>();

                    if (parentKill != null) {
                        killableWithRenderer = parentKill;
                    }
                }

                if (killableWithRenderer != null) {
                    return killableWithRenderer;
                }
                LevelSettings.LogIfNew(
                    "Could not locate Killable to alert from KillableChildVisibility script on GameObject '" + name +
                    "'.");
                _isValid = false;
                return null;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private void OnBecameVisible() {
            if (!_isValid) {
                return;
            }

            var killable = KillableToAlert;
            if (!_isValid) {
                return;
            }

            killable.BecameVisible();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnBecameInvisible() {
            if (!_isValid) {
                return;
            }

            var killable = KillableToAlert;
            if (!_isValid) {
                return;
            }
            killable.BecameInvisible();
        }
    }
}
/*! \endcond */