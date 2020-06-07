using UnityEngine;

public class KW_MenuInstructions : MonoBehaviour {
    void OnGUI() {
        GUI.Label(new Rect(10, 10, 760, 60), "Every time you get to this scene, your score will be reset to zero.");

        if (GUI.Button(new Rect(10, 50, 760, 60), "Start game")) {
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
            Application.LoadLevel(2);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
#endif
        }
    }
}
