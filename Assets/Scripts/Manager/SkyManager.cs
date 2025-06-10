using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SkyManager : MonoBehaviour
{
  [Header("SkyMateraial")] 
  public Material dawnSky;
  public Material daySky;
  public Material sunsetSky;
  public Material eveningSky;

  
  private Material blendedSky;

  private void Start()
  {
    blendedSky = new Material(eveningSky);
    RenderSettings.skybox = blendedSky;
    
    TimeManager.Instance.OnTimePeriodChanged += OnPeriodChanged;
  }

  private void OnPeriodChanged(string period)
  {
    if (period == "Dawn")
    {
      StartCoroutine(BlendSkybox(eveningSky, dawnSky, 10f));
    }
    else if (period == "Day")
    {
      StartCoroutine(BlendSkybox(dawnSky, daySky, 10f));
    }
    else if (period == "Sunset")
    {
      StartCoroutine(BlendSkybox(daySky, sunsetSky, 10f));
    }
    else 
    {
      StartCoroutine(BlendSkybox(sunsetSky, eveningSky, 10f));
    }
  }

  private IEnumerator BlendSkybox(Material oldM, Material newM, float duration)
  {
    float elapsed = 0f;
    while (elapsed < duration)
    {
      float t = elapsed / duration;
      LerpSky(oldM, newM, t);
      elapsed += Time.deltaTime;
      yield return null;
    }
    LerpSky(oldM, newM, 1f);
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


}
