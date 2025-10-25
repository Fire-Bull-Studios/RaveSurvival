using System.Collections;
using Mirror.BouncyCastle.Bcpg.Sig;
using UnityEngine;
using UnityEngine.UI;

public class DamageEffectHandler : MonoBehaviour
{
  public Image dmgFx;
  public float maxAlpha;
  private bool takingDamage = false;
  private IEnumerator co = null;
  
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    co = StartDamageFx();
    Color temp = dmgFx.color;
    temp.a = 0.0f;
    dmgFx.color = temp;
  }

  public void CreateDamageFx()
  {
    if (takingDamage)
    {
      StopCoroutine(co);
      Color temp = dmgFx.color;
      temp.a = 0;
      dmgFx.color = temp;
    }
    co = StartDamageFx();
    StartCoroutine(co);
  }

  public IEnumerator StartDamageFx()
  {
    takingDamage = true;
    Color temp = dmgFx.color;
    for (int i = 0; i <= ((int)(maxAlpha * 100)); i += 5)
    {
      if (i > ((int)(maxAlpha * 100)))
      {
        i = (int)(maxAlpha * 100);
      }
      temp.a = (float)(i / 100f);
      dmgFx.color = temp;
      yield return new WaitForSeconds(0.01f);
    }
    for (int i = (int)(maxAlpha * 100); i > 0; i -= 5)
    {
      if (i < 0)
      {
        i = 0;
      }
      temp.a = (float)(i / 100f);
      dmgFx.color = temp;
      yield return new WaitForSeconds(0.005f);
    }
    temp = dmgFx.color;
    temp.a = 0;
    dmgFx.color = temp;
    takingDamage = false;
    yield return null;
  }
}
