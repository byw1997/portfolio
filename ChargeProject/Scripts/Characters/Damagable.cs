using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void Damage(float damage, Vector2 dir, float knockback, float knockbacktime);
    void Die();
    IEnumerator KnockBack(Vector2 dir, float knockback, float knockbacktime);
    void Slow(float degree, float time, int spellid);
}
