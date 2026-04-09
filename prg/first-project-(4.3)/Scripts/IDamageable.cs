using System;

public interface IDamageable
{
    event Action<float> OnHealthChanged;

    float Health { get; set; }
    float MaxHealth { get; set; }

    void TakeDamage(float damage);
    void Heal(float hp);
    void Die();
}