using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Prote.cs
/// </summary>

public class Featherdraw : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Featherdraw";
    public override string BnameKR => "Featherdraw";
    public override string Description => $"턴 시작 | '깃' 카드 수치장 드로우";

    public Featherdraw(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (owner is Ally al)
        {
            // '깃'이 포함된 카드를 드로우 (구현 필요: 카드 필터링 로직)
                        al.DrawCard(Stack); // 수치만큼 '깃' 카드 드로우
        }
    }

}

public class TSEnerge : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "TSEnerge";
    public override string BnameKR => "TSEnerge";
    public override string Description => $"턴 시작 | 에너지 +수치";

    public TSEnerge(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.AddBuff(new Energe(owner, caster, Stack));
    }

}

public class Reflect : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Reflect";
    public override string BnameKR => "Reflect";
    public override string Description => $"피격 | 최대 {Stack}만큼 반사 피해 \n턴 종료시 제거";

    public Reflect(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

    public override void AfterDamaged(DamContext DC)
    {
        if (DC.Attacker != null)
        {
            int reflectedDamage = Mathf.Min(Stack, DC.GetFinalDamage());
            DC.Attacker.TakeDamage(reflectedDamage);
        }
    }

}

public class Counter : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "Counter";
    public override string BnameKR => "Counter";
    public override string Description => $"피격 | 반격 피해 수치\n턴 종료시 제거";

    public Counter(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

    public override void AfterDamaged(DamContext DC)
    {
        if (DC.Attacker != null)
        {
            int reflectedDamage = Mathf.Min(Stack, DC.DamResult.finalDamage);
            DC.Attacker.TakeDamage(reflectedDamage);
        }
    }

}

public class Next2TSD : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Next2TSD";
    public override string BnameKR => "보호 생성 [2턴]";
    public override string Description => $"다음 2턴 시작 | 보호 +수치";

    public Next2TSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Gain("SD", Stack);
        owner.AddBuff(new NextTSD(owner, caster, Stack));
        owner.RemoveBuff(this);
    }

}

public class NextTSD : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextTSD";
    public override string BnameKR => "NextTSD";
    public override string Description => $"다음 턴 시작 | 보호 +수치";

    public NextTSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Gain("SD", Stack);
        owner.RemoveBuff(this);
    }

}

public class Prote_A : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Prote_A";
    public override string BnameKR => "자세 - 공세";
    public override string Description => $"힘 +{Stack}" +
        $"\n공격 전: 방어 1 소모 → 힘 +1 (1턴)" +
        $"\n턴 종료시 제거";

    public Prote_A(F_Cha o, F_Cha c, int s) : base(o, c, s) { }

    // BnameKR
         // BnameKR

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, Stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster, val));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new Power(owner, caster, -Stack));
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 턴 시작시 제거
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
            } else
            {
                // [미리보기] 텍스트만 확인할 때: 
                                // SD가 깎이고 힘이 1 늘어날 "예정"이므로, UI 데미지 값에 +1만 시켜줍니다.
                DC.PlusDamage += 1;
            }
        }
        return DC;
    }

}

public class Prote_B : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Prote_B";
    public override string BnameKR => "Prote_B";
    public override string Description => $"방어력 +{Stack + 1}" +
        $"\n최초 +2, 이후 +1씩" +
        $"\n턴 종료시 제거";

    public Prote_B(F_Cha o, F_Cha c, int s) : base(o, c, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, Stack + 1));
    }

    public override void OnUpdate(int val)
    {
        owner.AddBuff(new DefPower(owner, caster, 1));
        Stack += 1;
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, -(1 + Stack)));
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 턴 시작시 제거
    }

}

public class Prote_C : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Prote_C";
    public override string BnameKR => "Prote_C";
    public override string Description => $"최초 드로우 +2, 이후 추가로 얻을 때마다 드로우 +1" +
        $"\n턴 종료시 제거";

    public Prote_C(F_Cha o, F_Cha c, int s) : base(o, c, s) { }

    // information

    public override void OnActivate()
    {
        if (owner is Ally al) { al.DrawCard(2); }
    }

    public override void OnUpdate(int val)
    {
        if (owner is Ally al) { al.DrawCard(1); }
    }

    public override void OnDeactivate()
    {
    }

    public override void OnTurnStart()
    {
        owner.RemoveBuff(this); // 효과 종료 시 버프 제거
    }

}

public class FeatherDamUp : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "FeatherDamUp";
    public override string BnameKR => "FeatherDamUp";
    public override string Description => $"'깃'이 포함된 카드 피해 +수치\n(지속)";

    public FeatherDamUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // '깃'이 들어간 카드의 피해량 +Stack
                // 실제 구현: 현재 사용 중인 카드 이름에 '깃' 포함 여부 확인 (구현 필요)
                // 임시로 모든 공격에 적용
        DC.PlusDamage += Stack;
        return DC;
    }

}

public class DrawSD : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "DrawSD";
    public override string BnameKR => "DrawSD";
    public override string Description => $"드로우 시 | 방어(SD) +수치";

    public DrawSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class TSSD : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "TSSD";
    public override string BnameKR => "TSSD";
    public override string Description => $"턴 시작 | 방어(SD) +수치";

    public TSSD(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.SD += Stack;
    }

}

