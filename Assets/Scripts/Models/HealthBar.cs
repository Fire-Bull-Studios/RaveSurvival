using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  public Image fill;
  public Slider healthBar;
  private float currentValue = 1;
  private Color currentColor = Color.cyan;
  private IEnumerator colorAnim;
  private IEnumerator sliderAnim;
  void Start()
  {
    currentValue = healthBar.value;
    currentColor = fill.color;
  }

  public void HandleHealthChange(float val)
  {
    Color color = Color.cyan;
    if (val <= 1 && val > .50)
    {
      color = Color.cyan;
    }
    if (val <= .50 && val > .25)
    {
      color = Color.yellow;
    }
    else if (val <= .25)
    {
      color = Color.violetRed;
    }
    else
    {
      Debug.Log($"Bro so cringe this shouldn't be possible and is invalid!");
    }

    if (color != currentColor)
    {
      if (colorAnim != null)
      {
        StopCoroutine(colorAnim);
      }
      colorAnim = ColorAnim(color);
      StartCoroutine(colorAnim);
    }
    if (sliderAnim != null)
    {
      StopCoroutine(sliderAnim);
    }
    sliderAnim = SliderAnim(val);
    StartCoroutine(sliderAnim);
  }

  private IEnumerator SliderAnim(float value)
  {
    while (value != currentValue)
    {
      currentValue = Mathf.Lerp(currentValue, value, 0.05f);
      if (Math.Abs(currentValue - value) < 0.01)
      {
        currentValue = value;
      }
      healthBar.value = currentValue;
      yield return new WaitForSeconds(0.01f);
    }
    //currentValue = value;
    yield return null;
  }

  private IEnumerator ColorAnim(Color color)
  {
    while (color.r != currentColor.r)
    {
      currentColor.r = Mathf.Lerp(currentColor.r, color.r, 1f);
      fill.color = currentColor;
      yield return new WaitForSeconds(0.1f);
    }
    while (color.g != currentColor.g)
    {
      currentColor.g = Mathf.Lerp(currentColor.g, color.g, 1f);
      fill.color = currentColor;
      yield return new WaitForSeconds(0.1f);
    }
    while (color.b != currentColor.b)
    {
      currentColor.b = Mathf.Lerp(currentColor.b, color.b, 1f);
      fill.color = currentColor;
      yield return new WaitForSeconds(0.1f);
    }
    //currentColor.r = color.r;
    //currentColor.g = color.g;
    //currentColor.b = color.b;
    yield return null;
  }
}
