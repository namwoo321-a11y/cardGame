using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_E_0.cs
/// </summary>

public class Evolution : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Evolution";
    public override string BnameKR => "진화";
    public override string Description => $"임시 - 수치만큼 힘 +{stack}";

    public Evolution(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster, val));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new Power(owner, caster, -stack));
    }

}

public class Thorns : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Thorns";
    public override string BnameKR => "가시";
    public override string Description => $"받은 피해 반사 | {stack}회 남음";

    public Thorns(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        DC.Attacker.TakeDamage(DC.GetFinalDamage());
        stack -= 1;
        StackCheck();
    }

}

public class MotherboardEnhance : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "MotherboardEnhance";
    public override string BnameKR => "마더보드 강화";
    public override string Description => $"임시 - 수치만큼 힘 +{stack}, 마더보드 영구 강화";

    public MotherboardEnhance(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Retreat : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Retreat";
    public override string BnameKR => "Retreat";
    public override string Description => $"[옵저버] 자동생성 버프";

    public Retreat(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Vulnerable : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Vulnerable";
    public override string BnameKR => "취약";
    public override string Description => $"받는 피해 {stack*10}% 상승 (최대 10)";

    private int Max_stack = 10;

    public Vulnerable(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        if (stack > Max_stack)
        {
            owner.AddBuff(new DefPower(owner, caster, -Max_stack));
            } else
        {
            owner.AddBuff(new DefPower(owner, caster, -stack));
        }
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        if (stack > Max_stack)
        {
            owner.AddBuff(new DefPower(owner, caster, -(val + Max_stack - stack)));
            } else
        {
            owner.AddBuff(new DefPower(owner, caster, -val));
        }
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, stack));
    }

}

public class Weak : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Weak";
    public override string BnameKR => "약화";
    public override string Description => $"주는 피해 {stack*10}% 감소 (최대 10)";

    public Weak(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        DC.PlusDamage += 1;
        return DC;
    }

}

public class Paralysis : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Paralysis";
    public override string BnameKR => "마비";
    public override string Description => $"다음 공격 / 방어 효과 50% 감소 ({stack}회)";

    public Paralysis(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage -= 0.5f;
        return DC;
    }

    public override void AfterAttack(DamContext DC)
    {
        stack -= 1;
        StackCheck();
    }

}

public class Seed : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Seed";
    public override string BnameKR => "Seed";
    public override string Description => $"수치가 5가 되면 '발아' 상태 돌입.." +
        $"\n발아: 턴 시작 | 수치만큼 의지 감소, 의지가 0이 되면 기절.";

    public Seed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Flash : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Flash";
    public override string BnameKR => "섬광";
    public override string Description => $"공격이 빗나갈 확률 + 10%," +
        $"\n공격, 턴 종료 : 1 감소";

    public Flash(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

