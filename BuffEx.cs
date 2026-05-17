using System;

/// <summary>
/// BuffEx
///
/// GitHub raw 파일 예시로 사용할 수 있는 버프 클래스 모음입니다.
/// Unity 프로젝트에서 이 파일을 그대로 복사하거나 GitHub raw 주소로 불러와 사용할 수 있습니다.
/// </summary>

// ------------------ 기본 버프
공개 class Power : Buff // 
{
    // information
    공개 override string Bname => "힘";
    공개 override string Description => $"가하는 피해량 증가";
    공개 override BuffType BuffType => BuffType.Power;
    공개 Power(F_Cha o, F_Cha c, int s) : base(o, c, s) { }
    공개 override void OnActivate()
    {
        owner.Power += stack; // 힘 + stack
    }
    공개 override void OnUpdate(int val)
    {
        base.OnUpdate(val); // stack 업데이트
        owner.Power += val; // 힘 + stack
    }
    공개 override void OnDeactivate()
    {
        owner.Power -= stack; // 힘 -1
    }
}

공개 class DefPower : Buff
{
    // information
    공개 override string Bname => "방어력 약화";
    public override string Description => $"받는 피해 {stack} 증가," +
        $"\n얻는 방어 {stack} 감소";
    public override BuffType BuffType => BuffType.Bad;
    public DefPower(F_Cha o, F_Cha c, int s) : base(o, c, s) { }
    public override void OnActivate()
    {
        owner.DefPower += stack; // 힘 + stack
    }
    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.DefPower += val; // 힘 + stack
    }
    public override void OnDeactivate()
    {
        owner.DefPower -= stack; // 힘 - stack
    }
}

// 1턴 힘 | 피해량 증가, 턴 시작 시 제거
public class Power_1T : Buff
{
    // information
    public override string Bname => "힘";
    public override string Description => $"피해량 증가";
    public override BuffType BuffType => BuffType.Good;
    public Power_1T(F_Cha o, F_Cha c, int s) : base(o, c, s) { }
    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
    }
    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster[^1], val)); // 마지막 부여자가 사용자.
    }
    public override void OnTurnStart()
    {
        owner.AddBuff(new Power(owner, caster[^1], -stack)); // 마지막 부여자가 사용자.
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }
}

// 다음 턴 힘 | 다음 턴 피해량 증가, 턴 시작 시 제거
public class PowerNT : Buff
{
    // information
    public override string Bname => "다음 턴 힘";
    public override string Description => $"다음 턴 피해량 증가";
    public override BuffType BuffType => BuffType.Good;
    public PowerNT(F_Cha o, F_Cha c, int s) : base(o, c, s) { }
    public override void OnTurnStart()
    {
        owner.AddBuff(new Power(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }
}

//---------------자원 버프, 대체로 효과 없음

public class Energe : Buff
{
    public override string Bname => "Energe";
    public override string Description => "자원, 특정 스킬 사용시 소모";
    public override BuffType BuffType => BuffType.Resource;
    public Energe(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
}

// 턴 종료 피해 버프
public class Poison : Buff
{
    // information
    public override string Bname => "Poison";
    public override string BnameKR => "독";
    public override string Description => $"턴 종료 | 스택만큼 피해를 받고 1 감소";
    public override BuffType BuffType => BuffType.Bad; // Poison 디버프.
    public Poison(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Poison"); // 스택만큼 데미지
        stack--; // 스택 1 감소
        if (stack == 0) { owner.RemoveBuff(this); } // 스택이 0이 되면 버프 제거
    }
}

// 턴 종료 피해, 스택 감소
public class Burn : Buff
{
    // information
    public override string Bname => "Burn";
    public override string BnameKR => "화상";
    public override string Description => $"턴 종료 | {stack} 피해, 1 감소";
    public override BuffType BuffType => BuffType.Bad;

    public Burn(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Burn"); // 스택만큼 데미지
        stack--; // 스택 1 감소
        if (stack == 0) { owner.RemoveBuff(this); } // 스택이 0이 되면 버프 제거
    }
}

// 턴 종료 시 피해
public class Bleed : Buff
{
    // information
    public override string Bname => "출혈";
    public override string Description => $"턴 종료 | {stack} 피해, 제거";
    public override BuffType BuffType => BuffType.Bad;
    public Bleed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    public override void OnTurnEnd()
    {
        owner.TakeAddDamage(stack, DamageType.HP, caster, "Bleed");
        owner.RemoveBuff(this);
    } // 스택만큼 데미지
}

// 턴 시작 시 피해, 피격 시 피해 및 스택 증가
public class Hurt : Buff
{
    // information
    public override string Bname => "상처";
    public override string Description => $" 턴 시작 | 수치만큼 출혈 증가, 수치 1/2 감소" +
        $"피격 | 수치 +1";
    public override BuffType BuffType => BuffType.Bad; // Hurt 디버프.
    public Hurt(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    public override void OnTurnStart()
    {
        if (stack > 0)
        {
            owner.AddBuff(new Bleed(owner, caster[^1], stack)); // 마지막 부여자가 사용자.
            owner.Consume(this, stack / 2); // 수치의 절반만큼 스택 감소")
        }
        else
        {
            owner.RemoveBuff(this); // 스택이 0이하가 되면 버프 제거
        }
    }
    public override void AfterDamaged(int damage, F_Cha[] attackers)
    {
        if (stack > 0)
        {
            owner.AddBuff(new Hurt(owner, caster[^1], 1)); // 마지막 부여자가 사용자.
        }
    }
}

public class Cold : Buff { public Cold(F_Cha target, F_Cha user, int s) : base(target, user, s) { } }
public class Freeze : Buff { public Freeze(F_Cha target, F_Cha user, int s) : base(target, user, s) { } }
public class Ice : Buff { public Ice(F_Cha target, F_Cha user, int s) : base(target, user, s) { } }

// DefP
public class Cure : Buff { public Cure(F_Cha o, F_Cha c, int s) : base(o, c, s) { } }
