using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// BuffCase
/// 
/// Unity 게임이 실행될 때 외부 GitHub raw URL에서 스크립트나 문서 텍스트를 가져와
/// 로컬에 저장하고, 필요 시 자동으로 갱신하는 기능을 제공합니다.
/// 
/// 사용 방법:
/// 1. BuffCase.cs를 Unity 프로젝트의 Assets 폴더에 복사합니다.
/// 2. 빈 GameObject를 만들고 BuffCase 컴포넌트를 추가합니다.
/// 3. GitHub raw 파일 URL을 githubRawUrl에 넣습니다.
///    예: https://raw.githubusercontent.com/namwoo321-a11y/cardGame/main/BuffCase.cs
/// 4. 게임 실행 시 인터넷 연결이 가능하면 지정된 URL에서 데이터를 다운로드합니다.
/// 
/// 이 스크립트는 빌드된 게임에서도 동작하며, 모든 사용자가 같은 URL에서 동일한 내용을 가져옵니다.
/// </summary>
public class BuffCase : MonoBehaviour
{
    [Header("GitHub 업데이트 설정")]
    [Tooltip("GitHub 의 raw 파일 주소. 예: https://raw.githubusercontent.com/owner/repo/branch/path/to/file.cs")]
    public string githubRawUrl = "https://raw.githubusercontent.com/namwoo321-a11y/cardGame/main/BuffCase.cs";

    [Tooltip("다운로드된 내용을 저장할 로컬 파일 이름. 빌드 환경에서는 persistentDataPath를 사용합니다.")]
    public string localFileName = "BuffCaseDownloaded.txt";

    [Tooltip("앱 시작 시 자동으로 다운로드를 시도합니다.")]
    public bool downloadOnStart = true;

    [Tooltip("마지막 다운로드 후 이 시간(초) 이내에 재다운로드를 하지 않습니다.")]
    public float refreshIntervalSeconds = 3600f;

    [Tooltip("다운로드한 파일이 변경되었을 때만 저장합니다.")]
    public bool saveOnlyIfChanged = true;

    private string LocalPath => Path.Combine(Application.persistentDataPath, localFileName);
    private string lastSavedHash;
    private float lastDownloadTime = -9999f;

    private void Start()
    {
        if (downloadOnStart)
        {
            StartCoroutine(UpdateFromGitHub());
        }
    }

    /// <summary>
    /// GitHub raw URL에서 텍스트를 가져와 로컬에 저장합니다.
    /// </summary>
    public IEnumerator UpdateFromGitHub()
    {
        if (string.IsNullOrEmpty(githubRawUrl))
        {
            Debug.LogWarning("BuffCase: GitHub raw URL이 비어있습니다.");
            yield break;
        }

        if (Time.realtimeSinceStartup - lastDownloadTime < refreshIntervalSeconds)
        {
            Debug.Log("BuffCase: 마지막 다운로드 이후 refreshIntervalSeconds가 지나지 않았습니다.");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(githubRawUrl))
        {
            request.timeout = 20;
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
            {
                Debug.LogError($"BuffCase: GitHub 다운로드 실패 - {request.error}");
                yield break;
            }

            string content = request.downloadHandler.text;
            if (string.IsNullOrEmpty(content))
            {
                Debug.LogWarning("BuffCase: GitHub에서 가져온 내용이 비어 있습니다.");
                yield break;
            }

            bool shouldSave = true;
            if (saveOnlyIfChanged)
            {
                string incomingHash = ComputeHash(content);
                if (incomingHash == lastSavedHash || (File.Exists(LocalPath) && ComputeHash(File.ReadAllText(LocalPath, Encoding.UTF8)) == incomingHash))
                {
                    shouldSave = false;
                    Debug.Log("BuffCase: 기존 내용과 동일합니다. 저장하지 않습니다.");
                }
                else
                {
                    lastSavedHash = incomingHash;
                }
            }

            if (shouldSave)
            {
                SaveTextToLocalFile(content);
                Debug.Log($"BuffCase: GitHub 업데이트 완료. 로컬 경로: {LocalPath}");
            }

            lastDownloadTime = Time.realtimeSinceStartup;
            OnUpdateCompleted(content);
        }
    }

    /// <summary>
    /// 로컬 파일로 텍스트를 저장합니다.
    /// </summary>
    private void SaveTextToLocalFile(string content)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LocalPath) ?? Application.persistentDataPath);
            File.WriteAllText(LocalPath, content, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Debug.LogError($"BuffCase: 로컬 파일 저장 오류 - {ex.Message}");
        }
    }

    /// <summary>
    /// 저장된 로컬 파일을 불러옵니다.
    /// </summary>
    public string LoadLocalFile()
    {
        if (!File.Exists(LocalPath))
        {
            Debug.LogWarning("BuffCase: 로컬 파일이 존재하지 않습니다.");
            return string.Empty;
        }

        try
        {
            return File.ReadAllText(LocalPath, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            Debug.LogError($"BuffCase: 로컬 파일 읽기 오류 - {ex.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// GitHub에서 가져온 데이터로 사용자 커스텀 로직을 실행할 수 있는 콜백입니다.
    /// </summary>
    protected virtual void OnUpdateCompleted(string content)
    {
        // 필요한 경우 서브클래스에서 오버라이드하여 콘텐츠를 바로 처리할 수 있습니다.
    }

    /// <summary>
    /// 간단한 해시를 계산하여 이전 컨텐츠와 비교합니다.
    /// </summary>
    private static string ComputeHash(string input)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in input)
            {
                hash = hash * 31 + c;
            }
            return hash.ToString();
        }
    }
}
