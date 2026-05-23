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
        int rand = UnityEngine.Random.Range(0, 3);
        if (rand == 0) owner.AddBuff(new GPR(owner, caster, stack));
        else if (rand == 1) owner.AddBuff(new GPY(owner, caster, stack));
        else owner.AddBuff(new GPP(owner, caster, stack));

        owner.RemoveBuff(this);
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
        // 대상이 가지고 있는 통합 화약(GP) 버프를 찾습니다.
        // (주의: F_Cha에 GetBuff가 없다면 owner.Buffs.Find(b => b.Bname == "GP") as GP 형태로 수정하세요)
        GP gpBuff = owner.GetBuff("GP") as GP; 

        if (gpBuff != null && gpBuff.stack > 0)
        {
            // GP Description에 명시된 예상 피해 공식 적용 (R*6 + P*5 + Y*4)
            int totalDamage = (gpBuff.R * 6) + (gpBuff.P * 5) + (gpBuff.Y * 4);
            if (totalDamage > 0)
            {
                owner.TakeDamage(totalDamage);
            }

            // 쌓인 화약 색상별로 폭발 추가 효과 적용
            if (gpBuff.R > 0) owner.AddBuff(new Burn(owner, caster, gpBuff.R));
            if (gpBuff.Y > 0) owner.AddBuff(new DefPower(owner, caster, gpBuff.Y));
            if (gpBuff.P > 0) caster.AddBuff(new Power_1T(caster, caster, gpBuff.P));

            // 폭발했으므로 대상의 GP 수치 초기화 및 버프 제거
            gpBuff.R = 0;
            gpBuff.P = 0;
            gpBuff.Y = 0;
            owner.Consume("GP", gpBuff.stack); 
        }

        // 기폭제 역할이 끝났으므로 폭발 버프 자신을 제거
        owner.RemoveBuff(this);
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
    public override string Description => $"화약 {stack} : 적 {R} | 자 {P} | 황 {Y}" +
        $"\n예상 피해 {R*6 + P*5 + Y*4}";

    public int R = 0;
    public int P = 0;
    public int Y = 0;

    public GP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void OnActivate()
    {
        CheckAutoExplosion();
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        CheckAutoExplosion();
    }

    // OnActivate와 OnUpdate의 중복 코드를 방지하기 위해 묶어줍니다.
    private void CheckAutoExplosion()
    {
        // 스택이 4 이상이면 폭발 발생.
        if (stack > 3)
        {
            stack -= 3;
            
            // 폭발 로직 발동 후, R/P/Y 수치도 3만큼 차감해주어야 값이 누적되는 버그가 안 생깁니다.
            if (R > 0)
            {
                owner.TakeDamage(3 * 7);
                owner.AddBuff(new Burn(owner, caster, 3));
                R = Mathf.Max(0, R - 3);
            }
            else if (Y > 0)
            {
                owner.TakeDamage(3 * 5);
                owner.AddBuff(new DefPower(owner, caster, 3));
                Y = Mathf.Max(0, Y - 3);
            }
            else if (P > 0)
            {
                owner.TakeDamage(3 * 6);
                caster.AddBuff(new Power_1T(caster, caster, 3));
                P = Mathf.Max(0, P - 3);
            }
        }
    }
}



