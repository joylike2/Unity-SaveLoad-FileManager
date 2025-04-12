using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FileManager.Demo {
    public class SampleCode : MonoBehaviour {
        public string aesKey = "";

        private void Start() {
            if (!string.IsNullOrEmpty(aesKey)) {
                FileSystem.Instance.SetAseKey(aesKey);
                Debug.Log("AES Key Set");
            }
            else {
                if (FileSystem.Instance.IsAseKeySet()) {
                    FileSystem.Instance.RemoveAseKey();
                    Debug.Log("AES Key Removed");
                }
            }
        }

        public void SaveSyncCode() {
            List<string> saveData = new List<string> { "A", "B", "C" };

            bool result = FileSystem.Instance.Save(saveData, "TestFile.dat");
            if (result) {
                Debug.Log("Saved!");
            }
            else {
                Debug.Log("Failed to save data!");
            }
        }

        public void LoadSyncCode() {
            OperationResult<List<string>> resultData = FileSystem.Instance.Load<List<string>>("TestFile.dat");
            if (resultData.IsSuccess) {
                List<string> result = resultData.Result;
                Debug.Log("Loaded!: " + result[0] + result[1] + result[2]);
            }
            else {
                Debug.Log("Failed to load data!: " + resultData.ErrorMessage);
            }
        }

        public void SaveAsyncCode() {
            _ = SaveAsync().ContinueWith(task => {
                if (task.IsFaulted) {
                    Debug.Log(task.Exception);
                }
            });
        }

        public async Task SaveAsync() {
            List<string> saveData = new List<string> { "A", "B", "C" };

            OperationResult result = await FileSystem.Instance.SaveAsync(saveData, "TestFile.dat");
            if (result.IsSuccess) {
                Debug.Log("Saved!");
            }
            else {
                Debug.Log("Failed to save data!");
            }
        }

        public void LoadAsyncCode() {
            _ = LoadAsync().ContinueWith(task => {
                if (task.IsFaulted) {
                    Debug.Log(task.Exception);
                }
            });
        }

        public async Task LoadAsync() {
            OperationResult<List<string>> resultData = await FileSystem.Instance.LoadAsync<List<string>>("TestFile.dat");
            if (resultData.IsSuccess) {
                List<string> result = resultData.Result;
                Debug.Log("Loaded!: " + result[0] + result[1] + result[2]);
            }
            else {
                Debug.Log("Failed to load data!: " + resultData.ErrorMessage);
            }
        }
    }
}