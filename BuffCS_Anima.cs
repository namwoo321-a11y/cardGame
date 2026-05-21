using UnityEngine;

public class BuffCS_Anima
{
    // Anima 캐릭터 전용 버프들
}



public class GPRand : Buff
{
    public override string Bname => "GP_Rand";
    public override string Description => "[Anima] 자동생성 버프";
    public GPRand(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    
}

public class Explosion : Buff
{
    public override string Bname => "Explosion";
    public override string Description => "[Anima] 자동생성 버프";
    
    public Explosion(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class GPY : Buff
{
    public override string Bname => "GP_Y";
    public override string Description => "[Anima] 자동생성 버프";
    
    public GPY(F_Cha target, F_Cha user, int s) : base(target, user, s) { }

}

public class GPP : Buff
{
    public override string Bname => "GP_P";
    public override string Description => "[Anima] 자동생성 버프";
   
    
    public GPP(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class GPR : Buff
{
    public override string Bname => "GP_R";
    public override string Description => "[Anima] 자동생성 버프";
    
    public GPR(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}


public class GPSelf : Buff
{
    public override string Bname => "GP_Self";
    public override string Description => "[Anima] 자동생성 버프";


    public GPSelf(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
}


public class LimitBreak : Buff
{
    public override string Bname => "LimitBreak";
    public override string Description => "[Anima] 자동생성 버프";
   
    
    public LimitBreak(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}