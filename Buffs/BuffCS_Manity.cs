using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Manity.cs
/// </summary>

public class Durability : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Durability";
    public override string BnameKR => "내구도";
    public override string Description => $"장비 내구도: {Stack}" +
        $"\n강화 효과 발동 후 감소 (0 되면 모든 강화 제거)";

    /// <summary>
            /// 내구도 감소 - 강화 효과가 발동되었을 때 호출됨
            /// </summary>
            public void DecreaseOnceDurability()
            {
                if (Stack > 0)
                {
                    Stack--;
                    if (Stack <= 0)
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

    public Durability(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
    }

    public override void OnTurnEnd()
    {
        // 내구도는 전투 내내 유지됨 (명시적으로 제거될 때까지)
    }

}

public class DuraProtect : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "DuraProtect";
    public override string BnameKR => "내구 보호";
    public override string Description => $"이번 턴 동안 내구도 감소 보호" +
        $"\n(턴 종료시 제거)";

    public DuraProtect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

}

public class CounterUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CounterUp";
    public override string BnameKR => "반격 강화";
    public override string Description => $"다음 반격 피해 {Stack}";

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

    public CounterUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        DC.Attacker.TakeDamage(Stack);
        // 효과 발동 후: 내구도 감소 시도
        TryDecreaseEquipmentDurability();
    }

}

public class DamageUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "DamageUp";
    public override string BnameKR => "공격 강화";
    public override string Description => $"다음 공격 피해 {Stack}";

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

    public DamageUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PlusDamage += Stack;
        if (!DC.IsPreview)
        {
            // [실행] 실제 카드 사용: 내구도 감소 시도
            TryDecreaseEquipmentDurability();
            }
        return DC;
    }

}

public class BlockUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "BlockUp";
    public override string BnameKR => "방어 강화";
    public override string Description => $"다음 방어 부여 {Stack}";

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

    public BlockUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class GravUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GravUp";
    public override string BnameKR => "중력 강화";
    public override string Description => $"힘 -1" +
        $"\n중력 약화 상대 피해 -50%" +
        $"\n턴 종료시 감소";

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

    public override void OnTurnStart()
    {
        // 스택 1 감소
        Stack--;
        if (Stack <= 0)
        {
            owner.RemoveBuff(this);
        }
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // 상대가 중력 약화를 가지고 있으면 피해 -50%
        if (DC.Target.HasCValue("GravDown"))
        {
            DC.PercentDamage -= 0.5f;
        }
        return DC;
    }

}

public class GravDown : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "GravDown";
    public override string BnameKR => "중력 약화";
    public override string Description => $"방어력 1, 힘 1" +
        $"\n턴 종료시 감소";

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
        Stack--;
        if (Stack <= 0) { owner.RemoveBuff(this); }
    }

}

public class CounterReflect : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CounterReflect";
    public override string BnameKR => "CounterReflect";
    public override string Description => $"피격 후 {Stack} 반사 피해" +
        $"\n1턴 지속";

    public CounterReflect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this);
    }

    public override void AfterDamaged(DamContext DC)
    {
        int reflectedDamage = Mathf.Min(Stack, DC.Damage);
        DC.Attacker.TakeDamage(reflectedDamage);
    }

}

