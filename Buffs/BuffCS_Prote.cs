using UnityEngine;

public class BuffCS_Prote
{
    //
    
}
// 1. 프로테 

/// <summary>
/// 에너지 깃털 - 깃부르미 카드에서 부여
/// 턴 시작 시 '깃'이 포함된 카드 1장 드로우
/// </summary>
public class Featherdraw : Buff
{
    public override string Bname => "에너지 깃털";
    public override string Description => $"턴 시작 | '깃' 카드 수치장 드로우";

    public Featherdraw(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (owner is Ally al)
        {
            // '깃'이 포함된 카드를 드로우 (구현 필요: 카드 필터링 로직)
            al.DrawCard(stack); // 수치만큼 '깃' 카드 드로우
        }
    }
}
/// <summary>
/// 충전 - TSEnerge 버프
/// 턴 시작 시 에너지 획득 (무환동력 등에서 부여)
/// </summary>
public class TSEnerge : Buff
{
    public override string Bname => "충전";
    public override string Description => $"턴 시작 | 에너지 +수치";
    public override BuffType BuffType => BuffType.Power;

    public TSEnerge(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.AddBuff(new Energe(owner, caster, stack));
    }
}

/// <summary>
/// 반사 - 수호장 전개 카드에서 부여
/// 피격 시 수치만큼 피해 반사, 턴 종료시 제거
/// </summary>
public class Reflect : Buff
{
    public override string Bname => "반사";
    public override string Description => $"피격 | 최대 {stack}만큼 반사 피해 \n턴 종료시 제거";
    public override BuffType BuffType => BuffType.Good;

    public Reflect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        if (DC.Attacker != null)
        {
            int reflectedDamage = Mathf.Min(stack, DC.GetFinalDamage());
            DC.Attacker.TakeDamage(reflectedDamage);
        }
    }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

/// <summary>반격 -  피격 시 수치만큼 반격 피해, 턴 종료 제거/// </summary>
public class Counter : Buff
{
    public override string Bname => "반격";
    public override string Description => $"피격 | 반격 피해 수치\n턴 종료시 제거";
    public override BuffType BuffType => BuffType.Bad;

    public Counter(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext DC)
    {
        if (DC.Attacker != null)
        {
            int reflectedDamage = Mathf.Min(stack, DC.DamResult.finalDamage);
            DC.Attacker.TakeDamage(reflectedDamage);
        }
    }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

/// <summary>
/// 보호 생성 [2턴] - 보호 생성기 카드에서 부여
/// 다음 2턴 시작 시마다 수치만큼 방어(SD) 획득
/// </summary>
public class Next2TSD : Buff
{
    public override string Bname => "Next2TSD";
    public override string BnameKR => "보호 생성 [2턴]";
    public override string Description => $"다음 2턴 시작 | 보호 +수치";
    public override BuffType BuffType => BuffType.Good;

    public Next2TSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.SD += stack; // 수치만큼 SD 얻음
        owner.AddBuff(new NextTSD(owner, caster, stack)); // 다음 턴에도 같은 효과 부여
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }
}

/// <summary>
/// 보호 생성 [1턴] - Next2TSD에서 연쇄되어 부여
/// 다음 턴 시작 시 수치만큼 방어(SD) 획득
/// </summary>
public class NextTSD : Buff
{
    public override string Bname => "보호 생성 [1턴]";
    public override string Description => $"다음 턴 시작 | 보호 +수치";
    public override BuffType BuffType => BuffType.Good;

    public NextTSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.SD += stack; // 수치만큼 SD 얻음
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }
}

// ---------------- 자세 버프


/// <summary>
/// 자세 - 공세 (깃날 베기, 최선의 방어는.. 등에서 부여)
/// 힘 + 수치, 공격 시마다 방어(SD) 1 소모해 힘 +1 (1턴)
/// 턴 종료시 제거
/// </summary>
public class Prote_A : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Prote_A"; // BnameKR
    public override string BnameKR => "자세 - 공세"; // BnameKR
    public override string Description => $"힘 +{stack}" +
        $"\n공격 전: 방어 1 소모 → 힘 +1 (1턴)" +
        $"\n턴 종료시 제거";

    public Prote_A(F_Cha o, F_Cha c, int s) : base(o, c, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster, val));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new Power(owner, caster, -stack));
    }
    public override DamContext BeforeAttack(DamContext DC)
    {
        if (owner.SD > 0)
        {
            if (!DC.IsPreview)
            {
                // [실행] 진짜 카드를 냈을 때: SD를 깎고 진짜 버프를 줍니다.
                owner.SD -= 1;
                owner.AddBuff(new Power_1T(owner, caster, 1));
            }
            else
            {
                // [미리보기] 텍스트만 확인할 때: 
                // SD가 깎이고 힘이 1 늘어날 "예정"이므로, UI 데미지 값에 +1만 시켜줍니다.
                DC.PlusDamage += 1;
            }
        }
        return DC;
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 턴 시작시 제거
    }
}

//  <summary>
/// 자세 - 수비 (방패 들기, 이번에는 쉬겠어 등에서 부여)
/// 방어력 + (수치+1)
/// 최초 방어력 +2, 이후 추가로 얻을 때마다 방어력 +1
/// 턴 종료시 제거
/// </summary>
public class Prote_B : Buff
{
    public override string Bname => "자세 - 수비";
    public override string Description => $"방어력 +{stack + 1}" +
        $"\n최초 +2, 이후 +1씩" +
        $"\n턴 종료시 제거";
    public override BuffType BuffType => BuffType.Good;

    public Prote_B(F_Cha o, F_Cha c, int s) : base(o, c, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, stack + 1));
    }

    public override void OnUpdate(int val)
    {
        owner.AddBuff(new DefPower(owner, caster, 1));
        stack += 1;
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, -(1 + stack)));
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 턴 시작시 제거
    }
}

// Fly 비행 **자세 - 비행** 즉시 | 드로우 + 2, 효과가 있을때 비행 발동시 드로우 +1 | 턴 종료시 제거
public class Prote_C : Buff
{
    // information
    public override string Bname => "자세 - 비행";
    public override string Description => $"최초 드로우 +2, 이후 추가로 얻을 때마다 드로우 +1" +
        $"\n턴 종료시 제거";
    public override BuffType BuffType => BuffType.Good;


    public Prote_C(F_Cha o, F_Cha c, int s) : base(o, c, s) { }
    public override void OnActivate() { if (owner is Ally al) { al.DrawCard(2); } }
    public override void OnUpdate(int val)    { if (owner is Ally al) { al.DrawCard(1); } }
    public override void OnDeactivate() { }
    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }
}

/// <summary>
/// 깃날 강화 - 깃날 강화 카드에서 부여 (지속)
/// 이름에 '깃'이 포함된 카드의 피해량 +수치
/// </summary>
public class FeatherDamUp : Buff
{
    public override string Bname => "깃날 강화";
    public override string Description => $"'깃'이 포함된 카드 피해 +수치\n(지속)";
    public override BuffType BuffType => BuffType.Power;

    public FeatherDamUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // '깃'이 들어간 카드의 피해량 +stack
        // 실제 구현: 현재 사용 중인 카드 이름에 '깃' 포함 여부 확인 (구현 필요)
        // 임시로 모든 공격에 적용
        DC.PlusDamage += stack;
        return DC;
    }
}

/// <summary>
/// 드로우 시 방어 증가 - 어게인스트 윈드 카드에서 부여
/// 드로우할 때마다 방어(SD) +수치
/// </summary>
public class DrawSD : Buff
{
    public override string Bname => "드로우 시 방어 증가";
    public override string Description => $"드로우 시 | 방어(SD) +수치";
    public override BuffType BuffType => BuffType.Power;

    public DrawSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
}

/// <summary>
/// 턴 시작 방어 생성 - 무환동력 카드에서 부여
/// 턴 시작 시마다 방어(SD) +수치
/// </summary>
public class TSSD : Buff
{
    public override string Bname => "턴 시작 방어 생성";
    public override string Description => $"턴 시작 | 방어(SD) +수치";
    public override BuffType BuffType => BuffType.Power;

    public TSSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.SD += stack;
    }
}
