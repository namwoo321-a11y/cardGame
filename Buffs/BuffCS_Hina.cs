using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_Hina.cs
/// </summary>

public class LP : Buff
{
    public override BuffType BuffType => BuffType.Resource;
    public override string Bname => "LP";
    public override string BnameKR => "LP";
    public override string Description => $"자원";

    public LP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class Phenylethylamine : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Phenylethylamine";
    public override string BnameKR => "Phenylethylamine";
    public override string Description => $"방어 +{Stack}, 힘 +{Stack}";

    public Phenylethylamine(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, Stack));
        owner.AddBuff(new Power(owner, caster, Stack));
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new DefPower(owner, caster, val));
        owner.AddBuff(new Power(owner, caster, val));
    }

    public override void OnDeactivate()
    {
        owner.AddBuff(new DefPower(owner, caster, Stack));
        owner.AddBuff(new Power(owner, caster, Stack));
        owner.RemoveBuff(this);
    }

}

public class Dopamine : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Dopamine";
    public override string BnameKR => "Dopamine";
    public override string Description => $"턴 시작시 코스트 +{Stack}\n이후 제거됨";

    public Dopamine(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Will += Stack;
        owner.RemoveBuff(this);
    }

}

public class Oxytocin : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Oxytocin";
    public override string BnameKR => "Oxytocin";
    public override string Description => $"턴 시작 | LP +{Stack}";

    public Oxytocin(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.AddBuff(new LP(owner, caster, Stack));
    }

}

public class Cortisol : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Cortisol";
    public override string BnameKR => "Cortisol";
    public override string Description => $"턴 시작 | 체력 피해 3 x 수치, 슬픔 +1" +
        $"\n1/2 감소";

    public Cortisol(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.TakeDamage(Stack * 3);
        if (owner is Ally al)
        {
            if (al.HasCValue("Depress")) { al.AddBuff(new Depress(owner, caster, 1)); }
        }
        Stack /= 2; StackCheck();
    }

}

public class Enkephalin : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Enkephalin";
    public override string BnameKR => "엔케팔린";
    public override string Description => $"의지 +1, 수치 -1, 기쁨 +1" +
        $"\n1/2 감소";

    public Enkephalin(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.DrawCard(Stack);
        if (owner is Ally al)
        {
            al.HasCValue("Joy");
            al.AddBuff(new Joy(owner, caster, 1));
        }
        Stack--; if (Stack <= 0) { owner.RemoveBuff(this); }
    }

}

public class Steroid : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Steroid";
    public override string BnameKR => "Steroid";
    public override string Description => $"턴 종료시 체력 4 x {Stack} 회복, 수치 -1\n(지속)" +
        $"\n1/2 감소";

    public Steroid(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.Heal(Stack * 4);
        Stack--; StackCheck(); // [수치--, 0 제거]
    }

}

public class CogDecline : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CogDecline";
    public override string BnameKR => "인지 저하";
    public override string Description => $"다음 행동 능률 -{Stack*10}%, 2 소모." +
        $"\n최대값 10";

    public CogDecline(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        Stack--; if (Stack <= 0) { owner.RemoveBuff(this); }
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        // 자신에게 피해 입히기
        if (DC.IsPreview)
        {
            float reduction = Stack / 10f; // 행동 능률 감소 (예: Stack=1이면 10% 감소)
                        DC.PercentDamage *= reduction; // 행동 효과 반감
        } else
        {
            float reduction = Stack / 10f; // 행동 능률 감소 (예: Stack=1이면 10% 감소)
                        DC.PercentDamage *= reduction; // 행동 효과 반감
                        Stack /= 2; if (Stack <= 0) { owner.RemoveBuff(this); }
        }
        return DC;
    }

}

public class CogIncrease : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "CogIncrease";
    public override string BnameKR => "인지 증가";
    public override string Description => $"행동 능률 +{Stack}%" +
        $"\n(행동시 위 확률로 발현, 대성공: 행동 효과 1.5배, 스택 2 감소)";

    public CogIncrease(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class NextDraw : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextDraw";
    public override string BnameKR => "NextDraw";
    public override string Description => $"다음 턴 카드 수치만큼 추가 드로우\n턴 시작시 발동 후 제거";

    public NextDraw(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        if (owner is Ally al)
        {
            al.DrawCard(Stack);
        }
        owner.RemoveBuff(this);
    }

}

public class Stun : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Stun";
    public override string BnameKR => "Stun";
    public override string Description => $"수치만큼 턴동안 기절 | 행동 불가";

    public Stun(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        Stack--;
        if (Stack <= 0)
        {
            owner.RemoveBuff(this);
        }
    }

}

public class RHormone : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "RHormone";
    public override string BnameKR => "RHormone";
    public override string Description => $"수치만큼 무작위 호르몬 부여";

    public RHormone(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        // 무작위로 감정 선택: 0=기쁨, 1=우울, 2=분노, 3=울분
                int randomEmotion = UnityEngine.Random.Range(0, 4);
        switch (randomEmotion)
        {
            case 0: // 기쁨
            owner.AddBuff(new Joy(owner, caster, Stack));
            break;
            case 1: // 우울
            owner.AddBuff(new Depress(owner, caster, Stack));
            break;
            case 2: // 분노
            owner.AddBuff(new Anger(owner, caster, Stack));
            break;
            case 3: // 울분
            owner.AddBuff(new Distress(owner, caster, Stack));
            break;
        }
    }

}

public class Joy : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Joy";
    public override string BnameKR => "Joy";
    public override string Description => $"수치만큼 기쁨 | 힘/방어 +수치\n(5 이상) 황홀경: 코스트 드로우 +2";

    public Joy(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, Stack));
        owner.AddBuff(new DefPower(owner, caster, Stack));
        CheckEmoExplosion();
    }

    public override void OnUpdate(int val)
    {
        owner.AddBuff(new Power(owner, caster, val));
        owner.AddBuff(new DefPower(owner, caster, val));
        Stack += val;
        CheckEmoExplosion();
    }

    public override void OnDeactivate()
    {
        owner.RemoveBuff(this);
    }

    private void CheckEmoExplosion()
    {
        if (Stack >= 5 && owner is Ally al)
        {
            al.DrawCard(2); // 황홀경: 코스트 드로우 +2
        }
    }

}

public class Depress : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Depress";
    public override string BnameKR => "우울";
    public override string Description => $"힘 - 수치/2 ({Stack / 2}), 방어 + 수치 ({Stack})" +
        $"\n";

    public Depress(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, -Stack));
        owner.AddBuff(new DefPower(owner, caster, Stack / 2));
        CheckEmoExplosion();
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster, -val));
        owner.AddBuff(new DefPower(owner, caster, val / 2));
        CheckEmoExplosion();
    }

    public override void OnDeactivate()
    {
    }

    private void CheckEmoExplosion()
    {
        if (Stack >= 5)
        {
            owner.AddBuff(new Despair(owner, caster, 2));
            owner.RemoveBuff(this);
        }
    }

}

public class Anger : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Anger";
    public override string BnameKR => "Anger";
    public override string Description => $"방어 -{Stack}, 힘 +{Stack / 2}" +
        $"\n10 이상이면 감정 폭발 - 야수화";

    public Anger(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new DefPower(owner, caster, -Stack));
        owner.AddBuff(new Power(owner, caster, Stack / 2));
        CheckEmoExplosion();
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new DefPower(owner, caster, -val));
        owner.AddBuff(new Power(owner, caster, val / 2));
        CheckEmoExplosion();
    }

    public override void OnDeactivate()
    {
    }

    private void CheckEmoExplosion()
    {
        if (Stack >= 5)
        {
            owner.AddBuff(new Berserk(owner, caster, 1));
        }
    }

}

public class Distress : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Distress";
    public override string BnameKR => "Distress";
    public override string Description => $"수치만큼 울분 | 힘/방어 -수치\n(5 이상) 과부하: 기절, 행동 취소";

    public Distress(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new Power(owner, caster, -Stack));
        owner.AddBuff(new DefPower(owner, caster, -Stack));
        CheckEmoExplosion();
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        owner.AddBuff(new Power(owner, caster, -val));
        owner.AddBuff(new DefPower(owner, caster, -val));
        CheckEmoExplosion();
    }

    public override void OnDeactivate()
    {
    }

    private void CheckEmoExplosion()
    {
        if (Stack >= 5)
        {
            owner.AddBuff(new Stun(owner, caster, 1)); // 과부하: 기절
        }
    }

}

public class Despair : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Despair";
    public override string BnameKR => "Despair";
    public override string Description => $"절망 상태\n피해량 -50% (감정 폭발)";

    public Despair(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage /= 2; // 피해량 50% 감소
        return DC;
    }

}

public class Berserk : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Berserk";
    public override string BnameKR => "Berserk";
    public override string Description => $"야수화 상태\n공격시 자해 (감정 폭발)";

    public Berserk(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

    public override void AfterAttack(DamContext DC)
    {
        // 자신이 입힌 피해가 저장된 DC. 무슨 효과 가져오기
                // 자신에게 피해 입히기
        owner.TakeDamage(Stack);
    }

}

public class Overload : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Overload";
    public override string BnameKR => "Overload";
    public override string Description => $"과부하 상태\n기절, 행동 취소 (감정 폭발)";

    public Overload(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

}

public class Ecstasy : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Ecstasy";
    public override string BnameKR => "Ecstasy";
    public override string Description => $"황홀경 상태\n코스트 드로우 +2 (감정 폭발)";

    public Ecstasy(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        owner.RemoveBuff(this);
    }

}

