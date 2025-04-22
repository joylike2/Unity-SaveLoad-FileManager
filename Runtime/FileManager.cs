using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using LifeLogs.FileSystem.Utils;

namespace LifeLogs.FileSystem {
    public class OperationResult {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public static OperationResult Success() => new OperationResult { IsSuccess = true };
        public static OperationResult Fail(string message) => new OperationResult { IsSuccess = false, ErrorMessage = message };
    }

    public class OperationResult<T> {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public string ErrorMessage { get; set; }

        public static OperationResult<T> Success(T data) => new OperationResult<T> { IsSuccess = true, Result = data };
        public static OperationResult<T> Fail(string message) => new OperationResult<T> { IsSuccess = false, ErrorMessage = message };
    }

    public class FileManager {
        private static FileManager _instance;

        public static FileManager Instance {
            get {
                if (_instance == null) {
                    _instance = new FileManager();
                }

                return _instance;
            }
        }

        /// <summary> Application.persistentDataPath </summary>
        private readonly string _folderPath;

        private string _aesKey = "";

        private FileManager() => _folderPath = Application.persistentDataPath;

        /// <summary> 경로 검사 </summary>
        private string EnsureDirectoryExists(string fileName) {
            string userFilePath = Path.Combine(_folderPath, fileName);
            string directory = Path.GetDirectoryName(userFilePath);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory!);
            }

            return userFilePath;
        }

        #region AES Key Setting

        public void SetAesKey(string providedKey) {
            if (string.IsNullOrWhiteSpace(providedKey)) {
                Debug.LogError("AES key cannot be empty!");
                return;
            }

            _aesKey = providedKey;
        }

        public void RemoveAesKey() => _aesKey = "";

        public bool IsAesKeySet() => !string.IsNullOrEmpty(_aesKey);

        #endregion

        #region Data Load & Save

        /// <summary> 비동기 저장 처리 </summary>
        public async Task<OperationResult> SaveAsync<T>(T t, string fileName) => await JsonSerializeAsync(t, fileName);

        private async Task<OperationResult> JsonSerializeAsync<T>(T t, string fileName) {
            try {
                string userFilePath = EnsureDirectoryExists(fileName);

                string finalData = await Task.Run(() => {
                    string serializedData = JsonConvert.SerializeObject(t, Formatting.Indented);

                    if (string.IsNullOrEmpty(_aesKey)) { //AES Key가 설정된 경우 AES암호화. 아니면 Base64만 인코딩
                        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serializedData));
                    }
                    else {
                        return FileSystemAESCryptor.Encrypt(serializedData, _aesKey);
                    }
                });

                await File.WriteAllTextAsync(userFilePath, finalData);
                return OperationResult.Success();
            }
            catch (Exception error) {
                return OperationResult.Fail(error.Message);
            }
        }

        /// <summary> 동기 저장 처리 </summary>
        public bool Save<T>(T data, string fileName) {
            try {
                string userFilePath = EnsureDirectoryExists(fileName);

                string serializedData = JsonConvert.SerializeObject(data, Formatting.Indented);

                string finalData;
                if (string.IsNullOrEmpty(_aesKey)) { //AES Key가 설정된 경우 AES암호화. 아니면 Base64만 인코딩
                    finalData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(serializedData));
                }
                else {
                    finalData = FileSystemAESCryptor.Encrypt(serializedData, _aesKey);
                }

                File.WriteAllText(userFilePath, finalData);

                return true;
            }
            catch (Exception) {
                return false;
            }
        }

        /// <summary> 비동기 로드 처리 </summary>
        public async Task<OperationResult<T>> LoadAsync<T>(string fileName) => await JsonDeserializeAsync<T>(fileName);

        private async Task<OperationResult<T>> JsonDeserializeAsync<T>(string fileName) {
            try {
                string userFilePath = Path.Combine(_folderPath, fileName);
                if (!File.Exists(userFilePath)) return OperationResult<T>.Fail("File not found.");

                T obj = await Task.Run(() => {
                    string fileContent = File.ReadAllText(userFilePath);

                    string jsonData;
                    if (string.IsNullOrEmpty(_aesKey)) {
                        jsonData = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileContent));
                    }
                    else {
                        jsonData = FileSystemAESCryptor.Decrypt(fileContent, _aesKey);
                    }

                    return JsonConvert.DeserializeObject<T>(jsonData);
                });

                return obj == null ? OperationResult<T>.Fail("Deserialized object is null.") : OperationResult<T>.Success(obj);
            }
            catch (Exception ex) {
                return OperationResult<T>.Fail(ex.Message);
            }
        }

        /// <summary> 동기 로드 처리 </summary>
        public OperationResult<T> Load<T>(string fileName) {
            try {
                string userFilePath = Path.Combine(_folderPath, fileName);
                if (!File.Exists(userFilePath)) return OperationResult<T>.Fail("File not found.");

                string fileContent = File.ReadAllText(userFilePath);

                string jsonData;
                if (string.IsNullOrEmpty(_aesKey)) {
                    jsonData = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(fileContent));
                }
                else {
                    jsonData = FileSystemAESCryptor.Decrypt(fileContent, _aesKey);
                }

                T obj = JsonConvert.DeserializeObject<T>(jsonData);

                return obj == null ? OperationResult<T>.Fail("Deserialized object is null.") : OperationResult<T>.Success(obj);
            }
            catch (Exception ex) {
                return OperationResult<T>.Fail(ex.Message);
            }
        }

        #endregion

        private void SampleCodeSync() { //동기
            //저장
            Save(new List<int> { 1, 2, 3 }, "UserData.json");

            // 로드
            var resultData = Load<List<int>>("UserData.json");
            if (resultData.IsSuccess) {
                List<int> result = resultData.Result;
            }
            else {
                string eMessage = resultData.ErrorMessage;
            }
        }

        private async Task SampleCode() { //비동기
            //저장
            _ = Save(new List<int> { 0, 1, 2 }, "UserData.json"); //비동기 실행이지만 결과 안기다림
            await SaveAsync(new List<int> { 0, 1, 2 }, "UserData.json"); //비동기 실행이지만 결과 안기다림

            //로드
            var resultData = await LoadAsync<List<int>>("UserData.json"); //비동기 처리를 위해서 async 함수로 로드 하길 권장.
            if (resultData.IsSuccess) {
                List<int> result = resultData.Result;
            }
            else {
                string eMessage = resultData.ErrorMessage;
            }
        }
    }
}