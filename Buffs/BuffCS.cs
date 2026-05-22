using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS.cs
/// </summary>

public class Power : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "Power";
    public override string BnameKR => "Power";
    public override string Description => $"가하는 피해량 증가";

    // information

    public Power(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 힘 + stack
        owner.Power += stack;
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        // stack 업데이트
        // 힘 + stack
        owner.Power += val;
    }

    public override void OnDeactivate()
    {
        // 힘 -1
        owner.Power -= stack;
    }

}

public class DefPower : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "DefPower";
    public override string BnameKR => "DefPower";
    public override string Description => $"받는 피해 {stack} 증가," +
        $"\n얻는 방어 {stack} 감소";

    // information

    public DefPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 힘 + stack
        owner.DefPower += stack;
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        // 힘 + stack
        owner.DefPower += val;
    }

    public override void OnDeactivate()
    {
        // 힘 - stack
        owner.DefPower -= stack;
    }

}

public class Power_1T : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Power_1T";
    public override string BnameKR => "Power_1T";
    public override string Description => $"피해량 증가";

    // information

    public Power_1T(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 마지막 부여자가 사용자.
        owner.AddBuff(new Power(owner, caster, stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        // 마지막 부여자가 사용자.
        owner.AddBuff(new Power(owner, caster, val));
    }

    public override void OnTurnStart()
    {
        // 마지막 부여자가 사용자.
        owner.AddBuff(new Power(owner, caster, -stack));
        // 효과 종료 시 버프 제거
        owner.RemoveBuff(this);
    }

}

public class PowerNT : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "PowerNT";
    public override string BnameKR => "PowerNT";
    public override string Description => $"다음 턴 피해량 증가";

    // information

    public PowerNT(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        // 마지막 부여자가 사용자.
        owner.AddBuff(new Power(owner, caster, stack));
        // 효과 종료 시 버프 제거
        owner.RemoveBuff(this);
    }

}

public class Energe : Buff
{
    public override BuffType BuffType => BuffType.Resource;
    public override string Bname => "Energe";
    public override string BnameKR => "Energe";
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
        // 스택만큼 데미지
        owner.TakeDamage(stack);
        // 스택 1 감소
        stack--;
        // 스택이 0이 되면 버프 제거
        if (stack == 0) { owner.RemoveBuff(this); }
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
        owner.TakeDamage(stack);
        stack--; StackCheck();
    }

}

public class Bleed : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Bleed";
    public override string BnameKR => "Bleed";
    public override string Description => $"턴 종료 | {stack} 피해, 제거";

    // information
        
        
        
        
         // 스택만큼 데미지

    public Bleed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeDamage(stack);
        owner.RemoveBuff(this);
    }

}

public class Hurt : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Hurt";
    public override string BnameKR => "Hurt";
    public override string Description => $"턴 시작 | 수치만큼 출혈 증가, 수치 1/2 감소" +
        $"\n피격 | 수치 1";

    // information
        
        
         // Hurt 디버프.

    public Hurt(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (stack > 0)
        {
            // 마지막 부여자가 사용자.
            owner.AddBuff(new Bleed(owner, caster, stack));
            // 수치의 절반만큼 스택 감소")
            owner.Consume(this, stack / 2);
        }
        } else
        {
            // 스택이 0이하가 되면 버프 제거
            owner.RemoveBuff(this);
        }
    }

    public override void AfterDamaged(DamContext DC)
    {
        if (stack > 0)
        {
            // 마지막 부여자가 사용자.
            owner.AddBuff(new Hurt(owner, caster, 1));
        }
    }

}

public class Cold : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cold";
    public override string BnameKR => "Cold";
    public override string Description => "";

    public Cold(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Freeze : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Freeze";
    public override string BnameKR => "Freeze";
    public override string Description => "";

    public Freeze(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Ice : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Ice";
    public override string BnameKR => "Ice";
    public override string Description => "";

    public Ice(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Cure : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cure";
    public override string BnameKR => "Cure";
    public override string Description => "";

    public Cure(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

