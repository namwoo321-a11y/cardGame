public override void OnActivate()
    {
        // 스택이 4 이상이면 폭발 발생. 같은 색일테지만 제대로 체크
        if (stack > 3)
        {
            stack -= 3;
            if (R > 0)
            {
                owner.TakeDamage(3 * 7);
                owner.AddBuff(new Burn(owner, caster, 3));
            }
            else if (Y > 0)
                    {
                        owner.TakeDamage(3 * 5);
                        owner.AddBuff(new DefPower(owner, caster, 3));
            } else if (P > 0)
        {
            owner.TakeDamage(3 * 6);
            caster.AddBuff(new Power_1T(caster, caster, 3));
        }
    }

    public override void OnUpdate(int val)
    {
        base.OnUpdate(val);
        // 스택이 4 이상이면 폭발 발생. 같은 색일테지만 제대로 체크
        if (stack > 3)
        {
            stack -= 3;
            if (R>0)
            {
                owner.TakeDamage(3 * 7);
                owner.AddBuff(new Burn(owner, caster, 3));
                } else if (Y > 0)
            {
                owner.TakeDamage(3 * 5);
                owner.AddBuff(new DefPower(owner, caster, 3));
                } else if (P > 0)
            {
                owner.TakeDamage(3 * 6);
                caster.AddBuff(new Power_1T(caster, caster, 3));
            }
    }
