using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[CustomEditor(typeof(Gun))]
public class GunInspector : Editor
{
  VisualElement myInspector;

  FloatField damage;
  FloatField range;
  FloatField fireRate;
  FloatField soundRange;
  FloatField velocity;
  EnumField weaponType;
  EnumField gunType;
  Vector3Field startPos;
  Vector3Field startRot;
  ObjectField fireSound;
  ObjectField bulletStart;
  ObjectField muzzleFlash;
  ObjectField impactEffect;
  ObjectField projectile;

  public override VisualElement CreateInspectorGUI()
  {
    // Create a new VisualElement to be the root of our Inspector UI.
    myInspector = new VisualElement();

    damage = new("Damage") { bindingPath = "damage" };
    range = new("Range") { bindingPath = "range" };
    fireRate = new("FireRate") { bindingPath = "fireRate" };
    soundRange = new("Sound Range") { bindingPath = "soundRange" };
    velocity = new("Velocity") { bindingPath = "velocity" };
    weaponType = new("Bullet Type") { bindingPath = "bulletType" };
    gunType = new("Gun Type") { bindingPath = "gunType" };
    startPos = new("Starting Position") { bindingPath = "startingPosition" };
    startRot = new("Starting Rotation") { bindingPath = "startingRotation" };
    fireSound = new("Fire Sound") { bindingPath = "fireSound" };
    bulletStart = new("Bullet Start") { bindingPath = "bulletStart" };
    muzzleFlash = new("Muzzle Flash") { bindingPath = "muzzleFlash" };
    impactEffect = new("Impact Effect") { bindingPath = "impactEffect" };
    projectile = new("Projectile") { bindingPath = "projectile" };

    // Add a simple label.
    myInspector.Add(damage);
    myInspector.Add(range);
    myInspector.Add(fireRate);
    myInspector.Add(soundRange);
    myInspector.Add(weaponType);
    myInspector.Add(gunType);
    myInspector.Add(fireSound);
    myInspector.Add(startPos);
    myInspector.Add(startRot);
    myInspector.Add(bulletStart);
    myInspector.Add(muzzleFlash);
    myInspector.Add(impactEffect);
    myInspector.Add(projectile);
    myInspector.Add(velocity);

    handler(weaponType);

    return myInspector;
  }
  public void handler(EnumField field)
  {
    field.RegisterCallback<ChangeEvent<Enum>>(showFields);
  }
  public void showFields(ChangeEvent<Enum> evt)
  {
    EnumField field = evt.currentTarget as EnumField;

    if (field.value.Equals(Gun.BulletType.PROJECTILE))
    {
      myInspector.Remove(impactEffect);
      myInspector.Add(projectile);
      myInspector.Add(velocity);
    }
    else
    {
      myInspector.Add(impactEffect);
      myInspector.Remove(projectile);
      myInspector.Remove(velocity);
    }
  }
}
