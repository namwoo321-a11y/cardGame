using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase Auto Generated - BuffCS_El.cs
/// </summary>

public class Chill : Buff
{
    public override BuffType BuffType => owner.name is "El" ? BuffType.Good : BuffType.Bad;
    public override string Bname => "Chill";
    public override string BnameKR => "한기";
    public override string Description => $"피해량 -20%, 공격, 턴 종료 |1/2 감소" +
        $"\n받는 한기 피해  수치 x 5% ({stack*5}%)";

    public Chill(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        stack /= 2;
        StackCheck();
    }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        if (DC.DT == DmgT.Frost)
        {
            DC.PercentDamage += stack * 0.05f;
        }
        return DC;
    }

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage -= 0.2f;
        return DC;
    }

    public override void AfterAttack(DamContext DC)
    {
        stack /= 2;
        StackCheck();
    }

}

public class IceShard : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "IceShard";
    public override string BnameKR => "얼음 조각";
    public override string Description => $"[El] 자동생성 버프";

    public IceShard(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class IceArmor : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "IceArmor";
    public override string BnameKR => "얼음장";
    public override string Description => $"피격 전 | {stack}만큼 피해 감소" +
        $"\n피격 후 | 한기 수치/5 <b>({stack/5})</b> 부여하고 수치 1/2 감소";

    public IceArmor(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeDamaged(DamContext DC)
    {
        DC.PlusDamage -= stack;
        return DC;
    }

    public override void AfterDamaged(DamContext DC)
    {
        DC.Attacker.AddBuff(new chill(DC.Attacker, caster, stack/5));
    }

}

public class NextChillAura : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "NextChillAura";
    public override string BnameKR => "차가운 바람";
    public override string Description => $"한기를 가진 적 적중했을 때," +
        $"\n다음 턴 한기 {stack} 부여 (캐릭터 당 1회)";

    // 캐릭터별로 누적 횟수를 저장 (Key: 캐릭터, Value: 발동 횟수)
        private Dictionary<F_Cha, int> _appliedCounts = new Dictionary<F_Cha, int>();
        private const int MAX_COUNT = 2; // 제한 횟수

    public NextChillAura(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        _appliedCounts.Clear(); 
    }

    public override void AfterAttack(DamContext DC)
    {
        F_Cha target = DC.Target; 
        if (target == null) return;
        
                // 1. 현재까지의 발동 횟수 확인
                if (!_appliedCounts.ContainsKey(target))
                {
                    _appliedCounts[target] = 0;
                }
        
                // 2. 제한 횟수 도달 여부 체크
                if (_appliedCounts[target] >= MAX_COUNT) 
                {return;}
        
                // 3. 효과 발동 로직 (여기에 한기 부여 로직 작성)
        DC.Target.AddBuff(new chill(DC.Target, caster, stack));
                // 4. 횟수 증가
                _appliedCounts[target]++;
    }

}

public class HeatAnomaly : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "HeatAnomaly";
    public override string BnameKR => "열이상";
    public override string Description => $"자신에게 한기가 있을 때," +
        $"\n턴 종료시 모든 적 {stack} 피해";

    public HeatAnomaly(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        // 캐릭터에게 한기 버프가 있을 때,
        // 모든 적에 stack만큼 피해
    }

}

public class NorthWind : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "NorthWind";
    public override string BnameKR => "북녘";
    public override string Description => $"턴 종료 | 한기를 가진 적에 한기 {stack} 부여";

    public NorthWind(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnEnd()
    {
        // 턴 종료 | 한기를 가진 적에 한기 {stack} 부여
    }

}

public class ChillDamageUp : Buff
{
    public override BuffType BuffType => BuffType.Power;
    public override string Bname => "ChillDamageUp";
    public override string BnameKR => "차갑게";
    public override string Description => $"한기를 가진 캐릭터에 가하는 피해 2 상승 (공격, ";

    public ChillDamageUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override DamContext BeforeAttack(DamContext DC)
    {
        if (DC.Target.CValue("Chill") > 0)
        {
            DC.PlusDamage += stack;
        }
        return DC;
    }

}

public class NextWill : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "NextWill";
    public override string BnameKR => "다음 턴 의지 증가";
    public override string Description => $"다음 턴 시작 | 의지 {stack} 증가, 버프 제거";

    public NextWill(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnTurnStart()
    {
        owner.Gain("Will", stack);
        owner.RemoveBuff(this);
    }

}
