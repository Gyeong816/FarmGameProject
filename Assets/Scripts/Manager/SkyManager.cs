using System;
using System.Collections;
using UnityEngine;

public class SkyManager : MonoBehaviour
{
    
    
    public float blendDuration;
    
    [Header("Sky Materials")]
    public Material dawnSky;
    public Material daySky;
    public Material sunsetSky;
    public Material eveningSky;

    [Header("Directional Light")]
    public Light directionalLight;
    public Color dawnLightColor;
    public Color dayLightColor;
    public Color sunsetLightColor;
    public Color eveningLightColor;

    private Material blendedSky;
    private Color startLightColor;
    private Color targetLightColor;
    private string currentPhase = "Evening";

    private void Start()
    {
        // 스카이박스 초기 설정
        blendedSky = new Material(eveningSky);
        RenderSettings.skybox = blendedSky;

        // 라이트 초기 컬러 세팅
        directionalLight.color = eveningLightColor;
        startLightColor = eveningLightColor;
        targetLightColor = eveningLightColor;

        // 시간대 변경 이벤트 구독
        TimeManager.Instance.OnTimePeriodChanged += OnPeriodChanged;
    }

    public void OnPeriodChanged(string newPhase)
    {
        Material oldSky = GetSkyMaterial(currentPhase);
        Material nextSky = GetSkyMaterial(newPhase);
        startLightColor = directionalLight.color;
        targetLightColor = GetLightColor(newPhase);
        currentPhase = newPhase;
        StartCoroutine(BlendSkyAndLight(oldSky, nextSky, blendDuration));
    }
    

    private IEnumerator BlendSkyAndLight(Material oldM, Material newM, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            LerpSky(oldM, newM, t);
            directionalLight.color = Color.Lerp(startLightColor, targetLightColor, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 최종 보간값 확정
        LerpSky(oldM, newM, 1f);
        directionalLight.color = targetLightColor;
    }

    private void LerpSky(Material oldM, Material newM, float t)
    {
        blendedSky.SetColor("_SunDiscColor", Color.Lerp(oldM.GetColor("_SunDiscColor"), newM.GetColor("_SunDiscColor"), t));
        blendedSky.SetFloat("_SunDiscMultiplier", Mathf.Lerp(oldM.GetFloat("_SunDiscMultiplier"), newM.GetFloat("_SunDiscMultiplier"), t));
        blendedSky.SetFloat("_SunDiscExponent", Mathf.Lerp(oldM.GetFloat("_SunDiscExponent"), newM.GetFloat("_SunDiscExponent"), t));

        blendedSky.SetColor("_SunHaloColor", Color.Lerp(oldM.GetColor("_SunHaloColor"), newM.GetColor("_SunHaloColor"), t));
        blendedSky.SetFloat("_SunHaloExponent", Mathf.Lerp(oldM.GetFloat("_SunHaloExponent"), newM.GetFloat("_SunHaloExponent"), t));
        blendedSky.SetFloat("_SunHaloContribution", Mathf.Lerp(oldM.GetFloat("_SunHaloContribution"), newM.GetFloat("_SunHaloContribution"), t));

        blendedSky.SetColor("_HorizonLineColor", Color.Lerp(oldM.GetColor("_HorizonLineColor"), newM.GetColor("_HorizonLineColor"), t));
        blendedSky.SetFloat("_HorizonLineExponent", Mathf.Lerp(oldM.GetFloat("_HorizonLineExponent"), newM.GetFloat("_HorizonLineExponent"), t));
        blendedSky.SetFloat("_HorizonLineContribution", Mathf.Lerp(oldM.GetFloat("_HorizonLineContribution"), newM.GetFloat("_HorizonLineContribution"), t));

        blendedSky.SetColor("_SkyGradientTop", Color.Lerp(oldM.GetColor("_SkyGradientTop"), newM.GetColor("_SkyGradientTop"), t));
        blendedSky.SetColor("_SkyGradientBottom", Color.Lerp(oldM.GetColor("_SkyGradientBottom"), newM.GetColor("_SkyGradientBottom"), t));
        blendedSky.SetFloat("_SkyGradientExponent", Mathf.Lerp(oldM.GetFloat("_SkyGradientExponent"), newM.GetFloat("_SkyGradientExponent"), t));
    }

    private Material GetSkyMaterial(string phase)
    {
        switch (phase)
        {
            case "Dawn":    return dawnSky;
            case "Day":     return daySky;
            case "Sunset":  return sunsetSky;
            case "Evening": return eveningSky;
            default:         return eveningSky;
        }
    }

    private Color GetLightColor(string phase)
    {
        switch (phase)
        {
            case "Dawn":    return dawnLightColor;
            case "Day":     return dayLightColor;
            case "Sunset":  return sunsetLightColor;
            case "Evening": return eveningLightColor;
            default:         return eveningLightColor;
        }
    }
    
    // 저장용
    public SkySaveData GetSaveData()
    {
        return new SkySaveData { phase = currentPhase };
    }
    
    
    public void SetPhaseImmediate(string phase)
    {
        // 1) 머티리얼 즉시 복사
        var m = GetSkyMaterial(phase);
        RenderSettings.skybox.CopyPropertiesFromMaterial(m);

        // 2) 라이트 즉시 설정
        directionalLight.color = GetLightColor(phase);

        // 3) 내부 상태만 교체
        currentPhase = phase;
    }
    
   
    
}
