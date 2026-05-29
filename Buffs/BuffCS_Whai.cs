using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Whai.cs
/// </summary>

public class DarkMagic : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "DarkMagic";
    public override string BnameKR => "마력";
    public override string Description => $"수치 4당 주는 피해 1 증가 ({Mathf.Clamp(Stack/4, 0, 5)} / 최대 5)\n" +
        $"\n수치 5당 받는 피해 1 증가 ({Mathf.Clamp(Stack / 5, 0, 4)} / 최대 4)";

    public DarkMagic(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 주는 피해 증가: Stack당 1, 최대 4

    public override DamContext BeforeDamaged(DamContext DC)
    {
        int damageIncrease = (Stack / 5); // Stack 5당 받는 피해 1 증가
                        damageIncrease = Mathf.Clamp(damageIncrease, 0, 4); // 최대 4까지 증가
        DC.PlusDamage += damageIncrease;
        return DC;
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        int damageBonus = Mathf.Clamp(Stack/4, 0, 5); // Stack 4당 1 증가, 최대 5
        DC.PlusDamage += damageBonus;
        return DC;
    }

}

public class Villain : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Villain";
    public override string BnameKR => "악당";
    public override string Description => "";

    public Villain(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Evade : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Evade";
    public override string BnameKR => "회피";
    public override string Description => $"[Whai] 자동생성 버프";

    public Evade(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Blind : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Blind";
    public override string BnameKR => "실명";
    public override string Description => $"빗나갈 확률 {Stack}0% 증가";

    public Blind(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class CostDown : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CostDown";
    public override string BnameKR => "순화";
    public override string Description => $"다음 사용 카드 Cost 감소";

    public CostDown(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Darkness : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Darkness";
    public override string BnameKR => "암흑";
    public override string Description => $"[Whai] 자동생성 버프";

    public Darkness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Lightness : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Lightness";
    public override string BnameKR => "빛";
    public override string Description => $"[Whai] 자동생성 버프";

    public Lightness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

