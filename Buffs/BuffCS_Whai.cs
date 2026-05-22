using Google.Protobuf.WellKnownTypes;
using UnityEngine;

public class BuffCS_Whai
{
    // Whai 캐릭터 전용 버프들
}



public class DarkMagic : Buff
{
    public override string Bname => "DarkMagic";
    public override string BnameKR => "마력";
    public override string Description => $"수치 4당 주는 피해 1 증가 ({Mathf.Clamp(stack/4, 0, 5)} / 최대 5)\n" +
        $"수치 5당 받는 피해 1 증가 ({Mathf.Clamp(stack / 5, 0, 4)} / 최대 4)";
   
    public DarkMagic(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    // 주는 피해 증가: stack당 1, 최대 4
    public override DamContext BeforeAttack(DamContext DC)
    {
        int damageBonus = Mathf.Clamp(stack/4, 0, 5); // stack 4당 1 증가, 최대 5
        DC.PlusDamage += damageBonus;
        return DC;
    }
    public override DamContext BeforeDamaged(DamContext DC)
    {
        int damageIncrease = (stack / 5); // stack 5당 받는 피해 1 증가
        damageIncrease = Mathf.Clamp(damageIncrease, 0, 4); // 최대 4까지 증가
        DC.PlusDamage += damageIncrease;
        return DC;
    }


     
 
     
    
}


public class Villain : Buff
{
    public override string Bname => "Villain";
    public override string BnameKR => "악당";
    public override string Description => "";
   

    
    public Villain(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Evade : Buff
{
    public override string Bname => "Evade";
    public override string Description => "[Whai] 자동생성 버프";
   

    
    public Evade(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Blind : Buff
{
    public override string Bname => "Blind";
    public override string Description => "[Whai] 자동생성 버프";
   

    
    public Blind(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class CostDown : Buff
{
    public override string Bname => "CostDown";
    public override string Description => "[Whai] 자동생성 버프";
   

    
    public CostDown(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Darkness : Buff
{
    public override string Bname => "Darkness";
    public override string Description => "[Whai] 자동생성 버프";
   

    
    public Darkness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}



public class Lightness : Buff
{
    public override string Bname => "Lightness";
    public override string BnameKR => "빛";
    public override string Description => "[Whai] 자동생성 버프";
   

    
    public Lightness(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}
