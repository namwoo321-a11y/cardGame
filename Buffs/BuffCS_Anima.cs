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
        // 무작위 화약 3종 중 하나를 부여하고 즉시 자신을 제거
        int rand = Rand(1, 3);
        if (rand == 1)
        {
            owner.AddBuff(new GPR(owner, caster, Stack));
        } else if (rand == 2)
        {
            owner.AddBuff(new GPY(owner, caster, Stack));
        } else
        {
            owner.AddBuff(new GPP(owner, caster, Stack));
        }
        owner.RemoveBuff(this);
    }

}

public class Explosion : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "Explosion";
    public override string BnameKR => "폭발";
    public override string Description => $"대상 <b>화약 폭발</b> 효과 발동";

    public Explosion(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        // 대상이 가지고 있는 통합 화약(GP) 버프를 찾습니다.
                GP gpBuff = owner.GetBuff("GP") as GP;
        if (gpBuff != null)
        {
            gpBuff.Explosion();
        }
        owner.RemoveBuff(this);
    }

}

public class GPY : Buff
{
    public override BuffType BuffType => BuffType.Bad;
    public override string Bname => "GPY";
    public override string BnameKR => "노랑 [화약]";
    public override string Description => $"화약 - 황색";

    public GPY(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, Stack, "Y"));
        owner.RemoveBuff(this);
    }

}

public class GPP : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPP";
    public override string BnameKR => "보라 [화약]";
    public override string Description => $"화약 - 자색";

    public GPP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, Stack, "P"));
        owner.RemoveBuff(this);
    }

}

public class GPR : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPR";
    public override string BnameKR => "적색 [화약]";
    public override string Description => $"화약 - 적색";

    public GPR(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        owner.AddBuff(new GP(owner, caster, Stack, "R"));
        owner.RemoveBuff(this);
    }

}

public class GPSelf : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "GPSelf";
    public override string BnameKR => "욕망 화약";
    public override string Description => $"욕망 화약욕망 화약욕망 화약욕망 화약";

    public GPSelf(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class LimitBreak : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "LimitBreak";
    public override string BnameKR => "한계 해제";
    public override string Description => $"의지 최대치 1," +
        $"\n공격, 피격, 폭발 피해 +25%," +
        $"\n턴 종료 | 수치 1, 수치가 3가 되면 기절, 버프 제거";

    public override int StackH => 4;

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
        Stack += 1;
        if (Stack == 4)
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
    public override string Description => $"화약 {Stack} : 적 {R} | 자 {P} | 황 {Y}" +
        $"\n예상 피해 {(R * 7) + (P * 6) + (Y * 5)}";

    public override int StackL => 0;

    public int R => elements.Count(e => e == "R");
    public int P => elements.Count(e => e == "P");
    public int Y => elements.Count(e => e == "Y");

    public GP(F_Cha target, F_Cha user, int s, string initialElement = "") : base(target, user, s) 
                            {
                                // 부여된 스택(s)만큼 리스트에 초기 속성을 채워줍니다.
                                if (!string.IsNullOrEmpty(initialElement))
                                {
                                    for (int i = 0; i < s; i++)
                                    {
                                        elements.Add(initialElement.ToUpper());
                                    }
                                }
                            }
    
    public GP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 1. 화약이 부여된 순서를 저장하는 리스트
                                private List<string> elements = new List<string>();
                            
                                // 2. 외부에서 순서를 확인할 수 있는 string 배열 프로퍼티 (예: {"Y", "R", "R"})
                                public string[] K => elements.ToArray();
                            
                            
                            // 추가

    public override void OnActivate()
    {
        if (Stack > 3)
        {
            CheckAutoExplosion();
        }
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        if (Stack > 3)
        {
            CheckAutoExplosion();
        }
    }

    private void CheckAutoExplosion()
    {
        if (Stack >= 4)
        {
            Explosion();
        }
    }

    public void Explosion()
    {
        if (elements.Count == 0) return;
        // 4. 부여된 순서대로 순차적 효과 발동
        foreach (string element in elements)
        {
            switch (element)
            {
                case "R":
                owner.TakeDamage(7);
                owner.AddBuff(new Burn(owner, caster, 1));
                break;
                case "P":
                owner.TakeDamage(6);
                caster.AddBuff(new Power_1T(caster, caster, 1));
                break;
                case "Y":
                owner.TakeDamage(5);
                owner.AddBuff(new DefPower(owner, caster, 1));
                break;
            }
        }
        // 5. 폭발 후 화약 제거 (초기화)
                elements.Clear();
        Stack = 0;
        owner.RemoveBuff(this);
    }

}

