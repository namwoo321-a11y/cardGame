using Unity.VisualScripting;
using UnityEngine;

public class BuffCS_El
{
    
}


public class Chill : Buff
{
    public override string Bname => "Chill";
    public override string BnameKR => "한기";
    public override string Description => $"피해량 -20%, 공격, 턴 종료 |1/2 감소" +
        $"\n받는 한기 피해 + 수치 x 5% ({stack*5}%)";
    public override BuffType BuffType => owner.name is "El" ? BuffType.Good : BuffType.Bad;

    public override DamContext BeforeAttack(DamContext DC)
    {
        DC.PercentDamage *= (1 - 0.2f); return DC;
        // 피해량 -20%
    }
    public override DamContext BeforeDamaged(DamContext DC)
    {
        if (DC.DT == DmgT.Frost) { DC.PercentDamage += stack * 0.02f; }
        return DC;
        // 피해량 -20%
    }

    public override void AfterAttack(DamContext DamageShift)
    {
        stack /= 2; StackCheck(); // 
    }
    public override void OnTurnEnd()
    {
        stack /= 2; StackCheck();// 
    }

public Chill(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}


public class IceShard : Buff
{
    public override string Bname => "IceShard";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Good;
    
    public IceShard(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    

}


public class IceArmor : Buff
{
    public override string Bname => "IceArmor";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Good;

    public IceArmor(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}



public class NextChillAura : Buff
{
    public override string Bname => "NextChillAura";
    public override string Description => "";
    public override BuffType BuffType => BuffType.Power;

    public NextChillAura(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class HeatAnomaly : Buff
{
    public override string Bname => "HeatAnomaly";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Power;

    public HeatAnomaly(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class NorthWind : Buff
{
    public override string Bname => "NorthWind";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Power;

    public NorthWind(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class ChillDamageUp : Buff
{
    public override string Bname => "ChillDamageUp";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Power;

    public ChillDamageUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}

public class NextWill : Buff
{
    public override string Bname => "NextWill";
    public override string Description => "[El] 자동생성 버프";
    public override BuffType BuffType => BuffType.Good;

    public NextWill(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    

}