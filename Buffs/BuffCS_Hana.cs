using UnityEngine;

public class BuffCS_Hana
{
    // Hana 캐릭터 전용 버프들
}



public class PowerUp : Buff
{
    public override string Bname => "PowerUp";
    public override string Description => "[Hana] 자동생성 버프";
   

    
    public PowerUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class CostUp : Buff
{
    public override string Bname => "CostUp";
    public override string Description => "[Hana] 자동생성 버프";
   

    
    public CostUp(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}
