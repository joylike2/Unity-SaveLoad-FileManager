# Unity SaveLoad FileManager (v1.0.0)  
　
　
## ✅ 소개  
　
이 FileManager 시스템은 게임 데이터를 저장하고 로드하는 기능으로  JSON 형태로 직렬화하여 Base64 인코딩을 통해 데이터 보호를 제공합니다.
또한, 선택적으로 AES-256 암호화를 지원하여 게임 데이터의 보안을 강화할 수 있습니다.  
　
　
## ⚠️ Newtonsoft Json 필요
JSON형태 변환에 기본 유니티 JSON기능 대신 Newtonsoft Json 라이브러리를 사용하였습니다.
- 이중구조에 대한 문제 해결
　  
	Unity Package Manager 를 통해 가져올 수 있습니다.
	1. **Package Manager** 열기
	2. **Install package by name…** 선택 
	3. **com.unity.nuget.newtonsoft-json** 입력 후 설치
　	     
<img src="https://github.com/joylike2/Unity-SaveLoad-FileManager/blob/main/Img_PackageManager.png?raw=true" width="480px">


```none
com.unity.nuget.newtonsoft-json
```



　
## ✅ 주요 기능
### 1. JSON 직렬화 및 역직렬화
- 게임 데이터를 JSON 포맷으로 직렬화하여 저장 및 로드합니다.
　
### 2. Base64 인코딩 지원
- Base64 인코딩을 사용하여 데이터를 저장하며, 데이터가 일반 텍스트로 노출되지 않아 최소한의 보호를 제공합니다.
　
### 3. AES-256 암호화 지원
- 추가로 AES 키 설정 시 데이터는 자동으로 암호화되어 저장됩니다.
　
### 4. 동기/비동기 저장 및 로드
- 유니티 게임의 성능 최적화를 위해 동기 및 비동기 방식으로 데이터 저장과 로드를 지원합니다.
　
### 5. 플랫폼 저장 경로 열기 지원
- 유니티 에디터 상단 `Tools/FileSystem/Open Persistent Data Path`를 통하여 저장 경로로 바 접근 가능합니다.(단축키 ctrl + alt + . )

　
　
　
## 📌 사용 방법
### AES 키 설정 및 해제
```csharp
//AES 키 설정
FileSystem.Instance.SetAseKey("AESKey");

//AES 키 해제
FileSystem.Instance.RemoveAseKey();

//AES 설정 확인
bool isAESKey = FileSystem.Instance.IsAseKeySet();
```
　
　
### 데이터 저장 및 로드
#### - 동기 방식
```csharp
//저장
FileSystem.Instance.Save(data, "FileName.dat");

//로드
DataType result = FileSystem.Instance.Load<DataType>("FileName.dat");
```

#### - 비동기 방식
```csharp
//저장
await FileSystem.Instance.SaveAsync(data, "FileName.dat");
　
//로드
DataType result = await FileSystem.Instance.LoadAsync<DataType>("FileName.dat");
```
　
　
## 포함된 주요 파일
- `FileSystem.cs`: 데이터의 저장 및 로드 메서드 제공.
- `FileSystemAESCryptor.cs`: AES-256 암호화 및 복호화 로직 제공.
- `OpenPersistentDataPath.cs`: Unity Editor에서 데이터 저장 경로를 빠르게 열 수 있는 편의 기능 제공.
- `Demo.unity`: 사용 예제를 보여주는 데모 씬.
- `SampleCode.cs`: 사용 예제를 보여주는 샘플 코드.

　
　
## 지원 환경
- Unity 엔진 기반 프로젝트
- Newtonsoft Json 패키지 필요

　
　
 　
### 🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉🎉
　
