using UnityEngine;


public interface IDamageable // 데미지를 입을 수 있는가
{
    bool TakeDamage(int damageAmount);
}


public interface IHealable // 회복이 가능한가
{
    void HealEntity(int healAmount);
}


public interface IInteractable // 상호작용이 가능한가
{
    void InteractEntity();
}