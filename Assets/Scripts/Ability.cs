using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name;
    public float CooldownTime;
    public float ActiveTime;
    public bool IsActive = false;
    public int LevelReq;

    public virtual void Activate(GameObject parent) { }
    public virtual void BeginCooldown(GameObject parent) { }

    public virtual bool Condition(GameObject parent) { return true; }
}

