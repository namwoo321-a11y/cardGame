using UnityEngine;

public class BuffCS_UG
{
    // UG 캐릭터 전용 버프들
}



public class NextBlock : Buff
{
    public override string Bname => "NextBlock";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public NextBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class NextHeal : Buff
{
    public override string Bname => "NextHeal";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public NextHeal(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class TempPower : Buff
{
    public override string Bname => "TempPower";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public TempPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class MaxHP : Buff
{
    public override string Bname => "MaxHP";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public MaxHP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class PermBlock : Buff
{
    public override string Bname => "PermBlock";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public PermBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class NextPower : Buff
{
    public override string Bname => "NextPower";
    public override string Description => "[UG] 자동생성 버프";
   

    
    public NextPower(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class KeepBlock : Buff
{
    public override string Bname => "KeepBlock";
    public override string Description => "[UG] 자동생성 버프";
   
    public KeepBlock(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}
