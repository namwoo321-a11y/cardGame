using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated
/// </summary>

public class NewBuff : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NewBuff";
    public override string BnameKR => "새 버프";
    public override string Description => $"";

    public NewBuff(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

}

