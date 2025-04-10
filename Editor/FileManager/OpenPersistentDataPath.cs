#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace FileManager {
    internal class OpenPersistentDataPath : MonoBehaviour {
        [MenuItem("Tools/FileSystem/Open Persistent Data Path %&.", priority = 1)]   //Ctrl(Cmd) + Alt + .
        internal static void OpenPersistentDataFolder() {
            string path = Application.persistentDataPath + "/";

            try {
                if (!Directory.Exists(path)) {  //경로 존재하지 않으면 폴더 생성.
                    Directory.CreateDirectory(path);
                }

                EditorUtility.RevealInFinder(path); //경로 폴더 열기.
            }
            catch (System.Exception e) {
                Debug.LogError($"경로 열기 실패: {e.Message}");
            }
        }
    }
}
#endif