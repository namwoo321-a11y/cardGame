using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS.cs
/// </summary>

public class Power_1T : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Power_1T";
    public override string Description => $"피해량 증가";

    // information

    public Power_1T(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster[^1], val)); // 마지막 부여자가 사용자.
    }

    public override void OnTurnStart()
    {
        owner.AddBuff(new Power(owner, caster[^1], -stack)); // 마지막 부여자가 사용자.
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }

}

public class PowerNT : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "PowerNT";
    public override string Description => $"다음 턴 피해량 증가";

    // information

    public PowerNT(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.AddBuff(new Power(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }

}

public class Energe : Buff
{
    public override BuffType BuffType => BuffType.Resource;
    public override string Bname => "Energe";
    public override string Description => $"자원, 특정 스킬 사용시 소모";

    public Energe(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Poison : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Poison";
    public override string BnameKR => "독";
    public override string Description => $"턴 종료 | 스택만큼 피해를 받고 1 감소";

    // information
        
        
        
         // Poison 디버프.

    public Poison(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Poison"); // 스택만큼 데미지
        stack--; // 스택 1 감소
        if (stack == 0) { owner.RemoveBuff(this); } // 스택이 0이 되면 버프 제거
    }

}

public class Burn : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Burn";
    public override string BnameKR => "화상";
    public override string Description => $"턴 종료 | {stack} 피해, 1 감소";

    // information

    public Burn(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Burn"); // 스택만큼 데미지
        stack--; // 스택 1 감소
        if (stack == 0) { owner.RemoveBuff(this); } // 스택이 0이 되면 버프 제거
    }

}

public class Bleed : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Bleed";
    public override string Description => $"턴 종료 | {stack} 피해, 제거";

    // information
        
        
        
        
         // 스택만큼 데미지

    public Bleed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Bleed");
        owner.RemoveBuff(this);
    }

}

public class Hurt : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Hurt";
    public override string Description => $"턴 시작 | 수치만큼 출혈 증가, 수치 1/2 감소" +
        $"\n피격 | 수치 1";

    // information
        
        
         // Hurt 디버프.

    public Hurt(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (stack > 0)
        {
            owner.AddBuff(new Bleed(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
            owner.Consume(this, stack / 2); // 수치의 절반만큼 스택 감소")
            }
            } else
        {
            owner.RemoveBuff(this); // 스택이 0이하가 되면 버프 제거
            }
    }

    public override void AfterDamaged(DamContext DC)
    {
        if (stack > 0)
        {
            owner.AddBuff(new Hurt(owner, caster[^1], 1)); // 마지막 부여자가 사용자.
            }
    }

}

public class Cold : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cold";
    public override string Description => "";

    public Cold(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Freeze : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Freeze";
    public override string Description => "";

    public Freeze(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Ice : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Ice";
    public override string Description => "";

    public Ice(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Cure : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cure";
    public override string Description => "";

    public Cure(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

