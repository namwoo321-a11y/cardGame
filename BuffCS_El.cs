using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated
/// </summary>

public class Chill : Buff
{
    public override BuffType BuffType => owner.name is "El" ? BuffType.Good : BuffType.Bad;
    public override string Bname => "Chill";
    public override string BnameKR => "한기";
    public override string Description => $"피해량 -20%, 공격, 턴 종료 |1/2 감소\n받는 한기 피해 + 수치 x 5% ({stack*5}%)";

    public Chill(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

    public override void OnTurnEnd()
    {
        stack /= 2;
        stackCheck();
    }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        if (DC.DT == DmgT.Frost)
        {
            // 받는 냉기 피해 n x 5% (최대 20스택 / 100)
            float stackH = Math.Min(stack,20) * 0.05f;
            DC.PercentDamage += stackH * 0.05f;
        }
        return DC;
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage -= 0.2f;
        // 피해량 -20%, 최소값 나중에 만들어야지. -값이면 없애던가.
        return DC;
    }

    public override void AfterAttack(DamContext DC)
    {
        stack /= 2;
        stackCheck();
    }

}

public class IceShard : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "IceShard";
    public override string Description => $"[El] 자동생성 버프";

    public IceShard(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class IceArmor : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "IceArmor";
    public override string Description => $"[El] 자동생성 버프";

    public IceArmor(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class NextChillAura : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "NextChillAura";
    public override string Description => $"";

    public NextChillAura(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class HeatAnomaly : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "HeatAnomaly";
    public override string Description => $"[El] 자동생성 버프";

    public HeatAnomaly(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class NorthWind : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "NorthWind";
    public override string Description => $"[El] 자동생성 버프";

    public NorthWind(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class ChillDamageUp : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "ChillDamageUp";
    public override string Description => $"[El] 자동생성 버프";

    public ChillDamageUp(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

public class NextWill : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextWill";
    public override string Description => $"[El] 자동생성 버프";

    public NextWill(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

