using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuffCS_Kira
{
    // Kira 캐릭터 전용 버프들 - 참격 공격 시스템 기반
    // 핵심: 날카로움 상태에서 참격 공격 시 상처 부여, 낙인으로 방어력 감소
}

// ============================================
// === 낙인 시스템 (Stigma) ===
// ============================================

/// <summary>
/// 낙인 - 상대의 방어력을 감소시킴
/// 방어력 -n, 특정 카드 사용 가능
/// 턴 종료 시 제거
/// </summary>
public class Stigma : Buff
{
    public override string Bname => "낙인";
    public override string Description => $"방어력 -{stack}\n턴 종료시 제거";



    public Stigma(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 방어력 -stack 적용
        owner.AddBuff(new DefPower(owner, caster, -stack));
    }

    public override void OnDeactivate()
    {
        // 방어력 +stack 복구
        owner.AddBuff(new DefPower(owner, caster, stack));
    }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

// ============================================
// === 날카로움 시스템 (Sharpness) ===
// ============================================

/// <summary>
/// 날카로움 - 참격 공격 강화
/// 참격 공격 적중 시 상처 n 부여
/// 턴 종료 시 제거
/// </summary>
public class Sharpness : Buff
{
    public override string Bname => "날카로움";
    public override string Description => $"참격 공격 | 상처 {stack} 부여\n턴 종료시 제거";
   


    public Sharpness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // 참격 공격일 때만 상처 부여
        // 실제 카드 사용 시 대상에게 상처 버프 적용
        if (DC.IsPreview && DC.Target != null)
        {
			// 날카로움 상태에서 공격했으므로 상처 부여
			DC.Target.AddBuff(new Wound(DC.Target, owner, stack));
        }

        return DC;
    }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

// ============================================
// === 상처 시스템 (Wound) ===
// ============================================

/// <summary>
/// 상처 - 누적 피해 디버프
/// 피격 시 상처 수치만큼 추가 피해
/// 턴 종료 시 1 감소, 0이 되면 제거
/// </summary>
public class Wound : Buff
{
    public override string Bname => "상처";
    public override string Description => $"피격 시 추가 피해 +{stack}\n턴 종료시 1 감소";



    public Wound(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        // 피격 시 상처 수치만큼 추가 피해
        DC.PlusDamage += stack;
        return DC;
    }

    public override void OnTurnEnd()
    {
        // 턴 종료 시 1 감소
        stack--;
        if (stack <= 0)
        {
            owner.RemoveBuff(this);
        }
    }
}

// ============================================
// === 피해 제한 시스템 (DamageLimit) ===
// ============================================

/// <summary>
/// 피해 제한 - 이번 턴 받는 피해의 최대값 제한
/// 받는 피해를 수치로 제한 (초과하면 제한)
/// 턴 종료 시 제거
/// </summary>
public class DamageLimit : Buff
{
    public override string Bname => "피해 제한";
    public override string Description => $"이번 턴 최대 피해 제한: {stack}\n턴 종료시 제거";
   


    private int damageThisTurn = 0; // 이번 턴 받은 누적 피해

    public DamageLimit(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        // 
        if (DC.IsPreview) return DC;

        // 이번 턴에 받을 수 있는 최대 피해 = stack - 이미 받은 피해
        int remainingCapacity = stack - damageThisTurn;

        if (remainingCapacity <= 0)
        {
            DC.FixDamage = 0;            // 제한 초과: 피해 무효
            return DC;
        }
        else if (DC.PlusDamage > remainingCapacity)
        {
            // 피해 제한: 제한값까지만 받음
            DC.FixDamage = remainingCapacity;
            return DC;
        }
        else
        {
            // 제한 이내: 정상 피해
            return DC;
        }
    }

    public override void OnTurnStart()
    {
        // 턴 시작 시 누적 피해 초기화
        damageThisTurn = 0;
    }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}

// ============================================
// === 흡혈 관련 헬퍼 ===
// ============================================

/// <summary>
/// 흡혈 효과는 카드 효과(CardEffect)로 직접 구현되지만,
/// 이 버프는 흡혈 상태를 표시하는 용도로 사용될 수 있습니다.
/// 실제 흡혈 피해와 회복은 카드의 DamageEf와 HealEf에서 처리됩니다.
/// 
/// 예시: 카드가 "적에게 피해+자신 체력 회복" 효과를 가지면,
/// CardEffect에서:
/// - new DamageEf(10, Target.Target) 
/// - new HealEf(10, Target.User) 로 구현
/// 
/// 이 Vampire 버프는 UI/시각적 표시용도로 남겨둡니다.
/// </summary>
public class Vampire : Buff
{
    public override string Bname => "흡혈";
    public override string Description => $"공격 피해 수치만큼 체력 회복\n(현재 활성화 중)";
   


    public Vampire(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 실제 효과는 카드 효과에서 처리 (DamageEf + HealEf 조합)
    // 이 버프는 상태 표시만 담당
}

// ============================================
// === 자해 & 관통 관련 ===
// ============================================

/// <summary>
/// 자해 - 자신에게 피해를 입히는 효과
/// 카드 효과로 직접 구현되며, 이 버프는 상태 표시용도입니다.
/// 
/// 예시: CardEffect에서
/// new DamageEf(value, Target.User) 로 직접 구현
/// </summary>
public class Selfdamage : Buff
{
    public override string Bname => "자해";
    public override string Description => "자신에게 피해를 입힌 수치";



    public Selfdamage(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    // 턴 종료시 사라짐
    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }
}
