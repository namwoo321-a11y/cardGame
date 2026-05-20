using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// BuffCase
/// 이 파일은 웹 메이커에서 자동 생성되었습니다.
/// </summary>

public class 이영G : Buff
{
    public override BuffType BuffType => BuffType.Good;
    public override string Bname => "이영G";
    public override string BnameKR => "새 버프";
    public override string Description => $"";

    public 이영G(F_Cha target, F_Cha user, int s) : base(target, user, s) 
    {
    }

    public override void OnActivate()
    {
        owner.Gain("Power", stack);
    }

    public override void OnUpdate(int val)
    {
        owner.Gain("Power", stack);
    }

    public override void OnDeactivate()
    {
        owner.Gain("Power", stack);
    }

    public override void OnTurnStart()
    {
        if (owner.CValue("SD") > 0)
        {
            owner.Gain("Power", stack);
        }
    }

}
