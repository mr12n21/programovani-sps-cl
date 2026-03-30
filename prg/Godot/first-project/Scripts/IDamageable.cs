using System;

public interface IDamageable
{
    event Action<float> OnHealthChanged;
    float MaxHealth { get; set; }
    float Health { get; set; }
    void TakeDamage(float damage);
    void Heal(float amount);
    void Die();
}