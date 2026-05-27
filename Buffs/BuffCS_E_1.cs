using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_E_1.cs
/// </summary>

public class Spore : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Spore";
    public override string BnameKR => "포자";
    public override string Description => $"[꽃개] 자동생성 버프";

    public Spore(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Rooted : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Rooted";
    public override string BnameKR => "속박";
    public override string Description => $"[덩굴] 자동생성 버프";

    public Rooted(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

