using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuffCS_Manity
{
    // Manity 캐릭터 전용 버프들 - 장비 시스템 기반
    // 핵심: 내구도(Durability)가 존재하면 강화 효과가 제거되지 않고 내구도만 감소
}

// ============================================
// === 내구도 시스템 (Durability Equipment) ===
// ============================================

/// <summary>
/// 내구도 - Manity의 장비 시스템 핵심
/// 모든 강화 효과(Up 버프)는 발동 후 제거되지만, 
/// 내구도가 있으면 내구도가 1 감소하고 효과는 유지됩니다.
/// 내구도가 0이 되면 모든 강화 효과가 제거됩니다.
/// </summary>
public class Durability : Buff
{
    public override string Bname => "Durability";
    public override string BnameKR => "내구도";
    public override string Description => $"장비 내구도: {stack}\n강화 효과 발동 후 감소 (0 되면 모든 강화 제거)";
   

    public Durability(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
    }

    /// <summary>
    /// 내구도 감소 - 강화 효과가 발동되었을 때 호출됨
    /// </summary>
    public void DecreaseOnceDurability()
    {
        if (stack > 0)
        {
            stack--;
            if (stack <= 0)
            {
                // 내구도 0 = 모든 강화 효과 제거
                RemoveAllEnhancementBuffs();
            }
        }
    }

    private void RemoveAllEnhancementBuffs()
    {
        // 모든 "Up" 버프를 찾아서 제거
        var buffsToRemove = owner.activeBuffs
            .Where(b => b.Bname.Contains("강화") || b.Bname.Contains("반격"))
            .ToList();

        foreach (var buff in buffsToRemove)
        {
            owner.RemoveBuff(buff);
        }
    }

    public override void OnTurnEnd()
    {
        // 내구도는 전투 내내 유지됨 (명시적으로 제거될 때까지)
    }
}

/// <summary>
/// 내구도 감소 보호 - DuraProtect 카드에서 부여
/// 이번 턴 동안은 내구도가 제거되지 않습니다.
/// 턴 종료 시 제거됩니다.
/// </summary>
public class DuraProtect : Buff
{
    public override string Bname => "DuraProtect";
    public override string BnameKR => "내구 보호";
    public override string Description => "이번 턴 동안 내구도 감소 보호\n(턴 종료시 제거)";
   

    public DuraProtect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

// ============================================
// === 강화 효과 (Enhancement Buffs) ===
// ============================================

/// <summary>
/// 반격 강화 - CounterUp
/// 다음 반격 피해 +수치
/// 발동 후 제거 (내구도 있으면 유지)
/// </summary>
public class CounterUp : Buff
{
    public override string Bname => "CounterUp";
    public override string BnameKR => "반격 강화";
    public override string Description => $"다음 반격 피해 +{stack}";
   

    public CounterUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        DC.Attacker.TakeDamage(stack);

        // 효과 발동 후: 내구도 감소 시도
        TryDecreaseEquipmentDurability();
    }

    private void TryDecreaseEquipmentDurability()
    {
        // 내구도 감소 보호 확인
        Buff protectBuff = owner.activeBuffs.FirstOrDefault(b => b.Bname == "내구도 감소 보호");
        if (protectBuff != null)
        {
            return; // 보호 중이므로 감소하지 않음
        }

        // 내구도 감소
        Durability durability = owner.activeBuffs.FirstOrDefault(b => b is Durability) as Durability;
        if (durability != null)
        {
            durability.DecreaseOnceDurability();
        }
        else
        {
            // 내구도 없으면 이 버프 제거
            owner.RemoveBuff(this);
        }
    }
}

/// <summary>
/// 공격 강화 - DamageUp
/// 다음 공격 피해 +수치
/// 발동 후 제거 (내구도 있으면 유지)
/// </summary>
public class DamageUp : Buff
{
    public override string Bname => "DamageUp";
    public override string BnameKR => "공격 강화";
    public override string Description => $"다음 공격 피해 +{stack}";
   
    public DamageUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PlusDamage += stack;

        if (!DC.IsPreview)
        {
            // [실행] 실제 카드 사용: 내구도 감소 시도
            TryDecreaseEquipmentDurability();
        }
        return DC;
    }

    private void TryDecreaseEquipmentDurability()
    {
        // 내구도 감소 보호 확인
        Buff protectBuff = owner.activeBuffs.FirstOrDefault(b => b.Bname == "내구도 감소 보호");
        if (protectBuff != null)
        {
            return; // 보호 중이므로 감소하지 않음
        }

        // 내구도 감소
        Durability durability = owner.activeBuffs.FirstOrDefault(b => b is Durability) as Durability;
        if (durability != null)
        {
            durability.DecreaseOnceDurability();
        }
        else
        {
            // 내구도 없으면 이 버프 제거
            owner.RemoveBuff(this);
        }
    }
}

/// <summary>
/// 방어 강화 - BlockUp
/// 다음 방어 부여 +수치
/// 발동 후 제거 (내구도 있으면 유지)
/// </summary>
public class BlockUp : Buff
{
    public override string Bname => "BlockUp";
    public override string BnameKR => "방어 강화";
    public override string Description => $"다음 방어 부여 +{stack}";
   
    public BlockUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    private void TryDecreaseEquipmentDurability()
    {
        // 내구도 감소 보호 확인
        Buff protectBuff = owner.activeBuffs.FirstOrDefault(b => b.Bname == "DuraProtect");
        if (protectBuff != null)
        {
            return; // 보호 중이므로 감소하지 않음
        }

        // 내구도 감소
        Durability durability = owner.activeBuffs.FirstOrDefault(b => b is Durability) as Durability;
        if (durability != null)
        {
            durability.DecreaseOnceDurability();
        }
        else
        {
            // 내구도 없으면 이 버프 제거
            owner.RemoveBuff(this);
        }
    }
}

// ============================================
// === 중력 시스템 (Gravity System) ===
// ============================================

/// <summary>
/// 중력 강화 - GravUp
/// 힘 -1, 턴 종료 시 1 감소
/// 중력 약화를 가진 상대에게 주는 피해 -50%
/// </summary>
public class GravUp : Buff
{
    public override string Bname => "GravUp";
    public override string BnameKR => "중력 강화";
    public override string Description => $"힘 -1\n중력 약화 상대 피해 -50%\n턴 종료시 감소";
   

    public GravUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 힘 -1 적용
        owner.AddBuff(new Power(owner, caster, -1));
    }

    public override void OnDeactivate()
    {
        // 힘 +1 복구
        owner.AddBuff(new Power(owner, caster, 1));
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // 상대가 중력 약화를 가지고 있으면 피해 -50%
        if (DC.Target != null)
        {
            Buff gravDownBuff = DC.Target.activeBuffs.FirstOrDefault(b => b.Bname == "중력 약화");
            if (gravDownBuff != null)
            {
                DC.PlusDamage = Mathf.RoundToInt(DC.PlusDamage * 0.5f);
            }
        }
        return DC;
    }

    public override void OnTurnStart()
    {
        // 스택 1 감소
        stack--;
        if (stack <= 0)
        {
            owner.RemoveBuff(this);
        }
    }
}

/// <summary>
/// 중력 약화 - GravDown
/// 방어 +1, 턴 종료 시 1 감소
/// </summary>
public class GravDown : Buff
{
    public override string Bname => "GravDown";
    public override string BnameKR => "중력 약화";
    public override string Description => $"방어력 +1, 힘 +1\n턴 종료시 감소";

    public override BuffType BuffType => BuffType.Power;

    public GravDown(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, 1));
        owner.AddBuff(new Power(owner, caster, 1));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, -1));
        owner.AddBuff(new Power(owner, caster, -1));
    }

    public override void OnTurnStart()
    {
        // 스택 1 감소
        stack--;
        if (stack <= 0) { owner.RemoveBuff(this); }
    }
}

/// <summary>
/// 반격 반사 - CounterReflect
/// 반격 시 반사 피해 추가 (Reflect와 CounterUp 복합)
/// 턴 종료 시 제거
/// </summary>
public class CounterReflect : Buff
{
    public override string Bname => "반격 반사";
    public override string Description => $"피격 후 {stack} 반사 피해\n 1턴 지속";
   
    public CounterReflect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        int reflectedDamage = Mathf.Min(stack, DC.Damage);
        DC.Attacker.TakeDamage(reflectedDamage);

    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this);
    }
}
