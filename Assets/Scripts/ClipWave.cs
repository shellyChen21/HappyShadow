using System;
using UnityEngine;

public class ClipWave : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private Transform target; 
    [SerializeField] private float scaleFactor = 10.0f; // 縮放因子，用來控制縮放範圍
    [SerializeField] private float smoothSpeed = 0.1f; // 平滑速度

    private float[] data = new float[256]; // 用來存儲音頻數據的數組
    private float currentScale = 0.1f; // 當前縮放值

    void Update()
    {
        // 獲取音頻數據
        audioSource.GetOutputData(data, 0);

        // 計算音量
        var currentVolume = CalculateRMSValue(data);

        // 根據音量計算目標縮放值
        var targetScale = Mathf.Clamp(currentVolume * scaleFactor, 0.1f, 1.0f);

        // 平滑地插值到目標縮放值
        currentScale = Mathf.Lerp(currentScale, targetScale, smoothSpeed);

        // 更新物件的縮放
        target.localScale = new Vector3(currentScale, currentScale, currentScale);
    }

    // 計算均方根音量
    float CalculateRMSValue(float[] samples)
    {
        var sum = 0.0f;
        for (var i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        return Mathf.Sqrt(sum / samples.Length);
    }
}