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
    public override string Description => $"[Speaker] 자동생성 버프";

    public Evolution(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

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
        DC.Attacker.TakeDamage(DC);
    }

}

public class MotherboardEnhance : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "MotherboardEnhance";
    public override string Description => $"[옵저버] 자동생성 버프";

    public MotherboardEnhance(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Retreat : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Retreat";
    public override string Description => $"[옵저버] 자동생성 버프";

    public Retreat(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Vulnerable : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Vulnerable";
    public override string Description => $"[메카라이저] 자동생성 버프";

    public Vulnerable(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Weak : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Weak";
    public override string Description => $"[메카라이저] 자동생성 버프";

    public Weak(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Paralysis : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Paralysis";
    public override string Description => $"[사막 독사] 자동생성 버프";

    public Paralysis(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Seed : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Seed";
    public override string Description => $"[개화의 시간-의] 자동생성 버프";

    public Seed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Flash : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Flash";
    public override string Description => $"[Moniter] 자동생성 버프";

    public Flash(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

