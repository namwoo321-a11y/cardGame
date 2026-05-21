using UnityEngine;

public class BuffCS_E_0
{
    // E_0 캐릭터 전용 버프들
}



public class Evolution : Buff
{
    public override string Bname => "Evolution";
    public override string Description => "[Speaker] 자동생성 버프";
   

    
    public Evolution(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    
}

// 가시: 상대가 나를 공격할 때, 그 공격에 반격으로 피해를 입히는 버프
// 1회 발동 후 사라짐.
public class Thorns : Buff
{
    public override string Bname => "Thorns";
    public override string BnameKR => "가시";
    public override string Description => "받은 피해 반사 (감소 전 피해)";

    public Thorns(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

    public override void AfterDamaged(DamContext dc)
    {
        // dc.attacker에게 반사 피해 적용
        if (dc.Attacker != null)
        {
            DamContext reflectDc = new DamContext(stack, DmgT.HP, dc.Attacker);
            dc.Attacker.ApplyDamage(reflectDc);
        }
        owner.RemoveBuff(this); // 1회 발동 후 제거
    }
    

         
     
         
        
    }


// 마더보드를 강화함. 영구적인 효과로, 나중에 제대로 구현.
public class MotherboardEnhance : Buff
{
    public override string Bname => "MotherboardEnhance";
    public override string Description => "[옵저버] 자동생성 버프";
   
    
    public MotherboardEnhance(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Retreat : Buff
{
    public override string Bname => "Retreat";
    public override string Description => "[옵저버] 자동생성 버프";
   

    
    public Retreat(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Vulnerable : Buff
{
    public override string Bname => "Vulnerable";
    public override string Description => "[메카라이저] 자동생성 버프";
   

    
    public Vulnerable(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Weak : Buff
{
    public override string Bname => "Weak";
    public override string Description => "[메카라이저] 자동생성 버프";
   

    
    public Weak(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Paralysis : Buff
{
    public override string Bname => "Paralysis";
    public override string Description => "[사막 독사] 자동생성 버프";
   

    
    public Paralysis(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class Seed : Buff
{
    public override string Bname => "Seed";
    public override string Description => "[개화의 시간-의] 자동생성 버프";
   

    
    public Seed(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}



public class Flash : Buff
{
    public override string Bname => "Flash";
    public override string Description => "[Moniter] 자동생성 버프";
   

    
    public Flash(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}