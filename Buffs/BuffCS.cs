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
    public override string BnameKR => "공격력";
    public override string Description => $"가하는 피해 {Stack} 증가";

    public Power(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // information

    public override void OnActivate()
    {
        owner.Gain("Power", Stack);
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.Gain("Power", val);
    }

    public override void OnDeactivate()
    {
        owner.Consume("Power", Stack);
    }

}

public class DefPower : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "DefPower";
    public override string BnameKR => "방어력";
    public override string Description => $"얻는 방어 {Stack} 감소";

    public DefPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // information

    public override void OnActivate()
    {
        owner.Gain("DefPower", Stack);
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.Gain("DefPower", val);
    }

    public override void OnDeactivate()
    {
        owner.Consume("DefPower", Stack);
    }

}

public class Power_1T : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Power_1T";
    public override string BnameKR => "Power_1T";
    public override string Description => $"피해량 {Stack} 증가 (1턴)";

    public Power_1T(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // information

    public override void OnActivate()
    {
        owner.Gain("Power", Stack);
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.Gain("Power", val);
    }

    public override void OnTurnStart()
    {
        owner.Consume("Power", Stack);
        owner.RemoveBuff(this);
    }

}

public class PowerNT : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "PowerNT";
    public override string BnameKR => "PowerNT";
    public override string Description => $"다음 턴 피해량 {Stack} 증가";

    public PowerNT(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // information

    public override void OnTurnStart()
    {
        owner.AddBuff(new Power_1T(owner, caster, stack));
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
    public override string Description => $"턴 종료 | {Stack} 피해, 1 감소";

    public Poison(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // information
                
                
                
                 // Poison 디버프.

    public override void OnTurnEnd()
    {
        // 스택만큼 데미지
        owner.TakeDamage(Stack);
        // 스택 1 감소
        Stack--;
        // 스택이 0이 되면 버프 제거
                        if (stack == 0) { owner.RemoveBuff(this); }
    }

}

public class Burn : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Burn";
    public override string BnameKR => "화상";
    public override string Description => $"턴 종료 | {Stack} 피해, 1 감소";

    public Burn(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeDamage(Stack);
        Stack -= 1;
        StackCheck();
    }

}

public class Bleed : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Bleed";
    public override string BnameKR => "Bleed";
    public override string Description => $"턴 종료 | {Stack} 피해, 제거";

    public Bleed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeDamage(Stack);
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

    public Hurt(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (Stack> 0)
        {
            // 마지막 부여자가 사용자.
            owner.AddBuff(new Bleed(owner, caster, Stack));
            // 수치의 절반만큼 스택 감소")
            owner.Consume(this, Stack/ 2);
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
    public override string BnameKR => "추위";
    public override string Description => $"추위";

    public Cold(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Freeze : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Freeze";
    public override string BnameKR => "빙결_";
    public override string Description => $"얼어붙은 상태.";

    public Freeze(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Ice : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Ice";
    public override string BnameKR => "얼음";
    public override string Description => $"얼어붙은 상태.";

    public Ice(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Cure : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cure";
    public override string BnameKR => "회복력";
    public override string Description => $"턴 시작 | {Stack} 회복";

    public Cure(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

