using UnityEngine;

public class BuffCS_E_1
{
    // E_1 캐릭터 전용 버프들
}



public class Spore : Buff
{
    public override string Bname => "Spore";
    public override string Description => "[꽃개] 자동생성 버프";
   
    public Spore(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    
}


public class Rooted : Buff
{
    public override string Bname => "Rooted";
    public override string Description => "[덩굴] 자동생성 버프";
   

    
    public Rooted(F_Cha target, F_Cha user, int s) : base(target, user, s) { }
    


     
 
     
    
}
