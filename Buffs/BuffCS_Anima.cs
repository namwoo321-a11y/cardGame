using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Anima.cs
/// </summary>

public class GPRand : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPRand";
    public override string BnameKR => "무작위 화약";
    public override string Description => $"무작위 화약 부여 후 즉시 사라짐." +
        $"\n남아있다면 버그, 코드 부분 꼭 확인할 것.";

    public GPRand(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
    }

}

public class Explosion : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Explosion";
    public override string BnameKR => "폭발";
    public override string Description => $"적 대상 폭발, 피해";

    public Explosion(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 현재 화약 버프가 몇개 있는지 확인
        // 부여된 순서대로 나열되므로
        owner.Consume("GPY", 1);
        owner.AddBuff(new Power_1T(owner, caster, 1));
    }

}

public class GPY : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "GPY";
    public override string BnameKR => "노랑 [화약]";
    public override string Description => "";

    public GPY(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, stack){ Y = stack });
        owner.RemoveBuff(this);
    }

}

public class GPP : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPP";
    public override string BnameKR => "보라 [화약]";
    public override string Description => $"[Anima] 자동생성 버프";

    public GPP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, stack){ P = stack });
        owner.RemoveBuff(this);
    }

}

public class GPR : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPR";
    public override string BnameKR => "적색 [화약]";
    public override string Description => $"[Anima] 자동생성 버프";

    public GPR(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, stack){ R = stack });
        owner.RemoveBuff(this);
    }

}

public class GPSelf : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPSelf";
    public override string BnameKR => "자신 화약?";
    public override string Description => $"???";

    public GPSelf(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class LimitBreak : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "LimitBreak";
    public override string BnameKR => "한계 해제";
    public override string Description => $"의지 최대치 1," +
        $"\n공격, 피격, 폭발 피해 25%," +
        $"\n턴 종료 | 수치 1, 수치가 3가 되면 기절, 버프 제거";

    public LimitBreak(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.Gain("MaxWill", 1);
    }

    public override void OnDeactivate()
    {
        owner.Gain("MaxWill", -1);
    }

    public override void OnTurnStart()
    {
        stack += 1;
        if (stack == 4)
        {
            owner.RemoveBuff(this);
        }
    }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        DC.PercentDamage += 0.25f;
        return DC;
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage += 0.25f;
        return DC;
    }

}

public class GP : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "GP";
    public override string BnameKR => "화약";
    public override string Description => $"화약 {stack} |적 {Red} | 자 {Purple} | 황 {Yellow}" +
        $"\n예상 피해 {R*6  P*5  Y*4}";

    public int R = 0;
                public int P = 0;
                public int Y = 0;
                // public string[] GPC = new string[3]{null,null,null}
                // 최대 크기 3인 string이거 만듬

    public GP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 스택이 4 이상이면 폭발 발생. 같은 색일테지만 제대로 체크
        if (stack > 3)
        {
            stack -= 3;
            if (R > 0)
            {
                owner.TakeDamage(3 * 7);
                owner.AddBuff(new Burn(owner, caster, 3));
                } else if (P > 0)
            {
                owner.TakeDamage(3 * 6);
                caster.AddBuff(new Power_1T(caster, caster, 3));
                } else if (Y > 0)
            {
                owner.TakeDamage(3 * 5);
                owner.AddBuff(new DefPower(owner, caster, 3));
            }
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        // 스택이 4 이상이면 폭발 발생. 같은 색일테지만 제대로 체크
        if (stack > 3)
        {
            stack -= 3;
            if (R>0)
            {
                owner.TakeDamage(3 * 7);
                owner.AddBuff(new Burn(owner, caster, 3));
                } else if (P > 0)
            {
                owner.TakeDamage(3 * 6);
                caster.AddBuff(new Power_1T(caster, caster, 3));
                } else if (Y > 0)
            {
                owner.TakeDamage(3 * 5);
                owner.AddBuff(new DefPower(owner, caster, 3));
            }
    }

}

