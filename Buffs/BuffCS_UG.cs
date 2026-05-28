using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_UG.cs
/// </summary>

public class NextBlock : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextBlock";
    public override string BnameKR => "지연 방어";
    public override string Description => $"다음 턴 시작 | 방어 {stack}," +
        $"\n버프 제거";

    public NextBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Gain("Block", Stack);
        owner.RemoveBuff(this);
    }

}

public class NextHeal : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextHeal";
    public override string BnameKR => "지연 치유";
    public override string Description => $"다음 턴 시작 | 회복 {stack}," +
        $"\n버프 제거";

    public NextHeal(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Heal(Stack);
        owner.RemoveBuff(this);
    }

}

public class TempPower : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "TempPower";
    public override string BnameKR => "TempPower";
    public override string Description => $"[UG] 자동생성 버프";

    public TempPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class MaxHP : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "MaxHP";
    public override string BnameKR => "쵀대 체력 상승";
    public override string Description => $"[UG] 자동생성 버프";

    public MaxHP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class PermBlock : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "PermBlock";
    public override string BnameKR => "다음 턴 방어";
    public override string Description => $"[UG] 자동생성 버프";

    public PermBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class NextPower : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextPower";
    public override string BnameKR => "다음 턴 공격 강화";
    public override string Description => $"1턴 뒤 피해량 +{stack}";

    public NextPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class KeepBlock : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "KeepBlock";
    public override string BnameKR => "방어 유지";
    public override string Description => $"{stack} 턴간 방어도 유지";

    public KeepBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

