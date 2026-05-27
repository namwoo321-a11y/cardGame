using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Kira.cs
/// </summary>

public class Stigma : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Stigma";
    public override string BnameKR => "낙인";
    public override string Description => $"방어력 -{Stack}" +
        $"\n턴 시작 | 제거";

    public Stigma(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, -Stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new DefPower(owner, caster, -val));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, Stack));
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this);
    }

}

public class Sharpness : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Sharpness";
    public override string BnameKR => "날카로움";
    public override string Description => $"참격 적중 | 상처 {Stack} 부여" +
        $"\n턴 시작 | 제거";

    public Sharpness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this);
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        if (!DC.IsPreview && DC.Target != null)
        {
            // 날카로움 상태에서 공격했으므로 상처 부여
            DC.Target.AddBuff(new Wound(DC.Target, owner, Stack));
        }
        return DC;
    }

}

public class Wound : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Wound";
    public override string BnameKR => "상처";
    public override string Description => $"피격 | 추가 {Stack} 피해" +
        $"\n턴 시작 | 1 감소";

    public override int StackL => 0;

    public Wound(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        Stack--;
    }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        DC.PlusDamage += Stack;
        return DC;
    }

}

public class DamageLimit : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "DamageLimit";
    public override string BnameKR => "피해 제한";
    public override string Description => $"이번 턴 최대 피해 제한: [{damageThisTurn} / {Stack}]";

    private int damageThisTurn = 0;

    public DamageLimit(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        // 턴 시작 시 누적 피해 초기화
        damageThisTurn = 0;
    }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        if (DC.IsPreview) return DC;
        // 이번 턴에 받을 수 있는 최대 피해 = Stack - 이미 받은 피해
                int remainingCapacity = Stack - damageThisTurn;
        if (remainingCapacity <= 0)
        {
            DC.FixDamage = 0;            // 제한 초과: 피해 무효
            return DC;
        } else if (DC.PlusDamage > remainingCapacity)
        {
            // 피해 제한: 제한값까지만 받음
            DC.FixDamage = remainingCapacity;
            return DC;
        } else
        {
            // 제한 이내: 정상 피해
            return DC;
        }
    }

}

public class Vampire : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Vampire";
    public override string BnameKR => "흡혈";
    public override string Description => $"{stack}만큼 입힌 피해 수치에 비례해 체력 회복";

    public Vampire(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 실제 효과는 카드 효과에서 처리 (DamageEf + HealEf 조합)
        // 이 버프는 상태 표시만 담당

}

public class Selfdamage : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Selfdamage";
    public override string BnameKR => "Selfdamage";
    public override string Description => $"자신에게 피해를 입힌 수치";

    public Selfdamage(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 턴 종료시 사라짐

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

}

