using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Hana.cs
/// </summary>

public class PowerUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "PowerUp";
    public override string BnameKR => "PowerUp";
    public override string Description => $"[Hana] 자동생성 버프";

    public PowerUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class CostUp : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CostUp";
    public override string BnameKR => "CostUp";
    public override string Description => $"[Hana] 자동생성 버프";

    public CostUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

