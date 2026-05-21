using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_Cha : MonoBehaviour
{
    [Header("UI Set")]
    // 내 프리팹에 같이 붙어있는 UI 관리자 스크립트를 연결해 둡니다.
    public F_ChaUI myUI;
    public Canvas childCanvas;
    public CanvasGroup myCG; // 캐릭터 선택되지 않았으면 아군일 때 0.5f, 적일 때 0.2f 정도로 투명하게 만들어서 선택된 캐릭터가 더 눈에 띄도록 하는 용도입니다.

    [Header("SD / Animation")]
    public SpriteRenderer ChaSD;
    public bool isActing = false; // UsingCard 대신 일반적인 상태명 사용

    // UI 생성을 위한 프리팹과 부모
    [Header("Buff")]
    public GameObject buffUIPrefab;
    public Transform buffUIParent;


    [Header("Basic Info")]
    public string chaName = "AAA";
    public Vector3 originPos; // FPos 대신 명확한 이름 사용

    //[Header("Stats")]
    [HideInInspector] public int LV, EXP;
    [HideInInspector] public int MaxHP, HP, MaxMP, MP, Block, SD;    // 방어 (턴이 지나면 사라짐) 보호 (유지되는 방어막 등)
    [HideInInspector] public int Will, MaxWill, PlusWill; // 의지 시스템
    [HideInInspector] public int Draw, MaxDraw, PlusDraw; // 드로우 시스템

    [HideInInspector] public bool Discarding = false; // UsingCard 대신 일반적인 상태명 사용

    //[Header("Power Stats")]
    [HideInInspector] public int Power, DefPower, HealPower, SDPower;


    // --- 1. 버프 시스템 (이전에 만든 구조 적용) ---
    [HideInInspector] public List<Buff> activeBuffs = new List<Buff>();
    // 현재 캐릭터에게 걸려있는 버프들의 리스트. 버프 클래스는 나중에 만들 예정
    [HideInInspector] public Dictionary<Buff, BuffUI> buffUIDict = new Dictionary<Buff, BuffUI>();
    // 버프와 UI를 연결하는 딕셔너리. 버프가 걸릴 때 UI도 생성하고, 버프가 사라질 때 UI도 제거하는 식으로 관리합니다.
        //-----------------------함수 시작 -----------------------


    private void Start()
    {
        Discarding = false;
        originPos = transform.position;
    }


    /// <summary>캐릭터가 해당 이름의 변수를 가지고 있는지 확인</summary>
    public bool HasCValue(string valueName)
    {
        // Reflection을 사용하여 F_Cha의 public int 필드를 체크
        var field = typeof(F_Cha).GetField(valueName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int)) { return true; }

        // 버프에서 찾기
        for (int i = 0; i < activeBuffs.Count; i++) { if (activeBuffs[i].Bname == valueName) { return true; } }

        return false;
    }

    /// <summary>
    /// 해당 이름의 변수 값을 반환합니다. (없으면 0)
    /// </summary>
    public int CValue(string valueName)
    {
        // Reflection을 사용하여 F_Cha의 public int 필드 값을 가져오기
        var field = typeof(F_Cha).GetField(valueName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int)) { return (int)field.GetValue(this); }

        // 버프에서 찾기
        return BuffCheck(valueName);
    }


    /// <summary>
    /// 해당 필드에 값을 더합니다. (Reflection 기반)
    /// </summary>
    public void Gain(string fieldName, int amount)
    {
        var field = typeof(F_Cha).GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int))
        {
            int currentValue = (int)field.GetValue(this);
            field.SetValue(this, currentValue + amount);
            myUI.RefreshAllUI();
            return;
        }

        Debug.LogWarning($"필드 '{fieldName}'를 찾을 수 없습니다.");
    }

    /// <summary> 값을 소모합니다. 얻는 효과와 겹치지 않음.</summary>
    public void Consume(string fieldName, int amount)
    {
        var field = typeof(F_Cha).GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (field != null && field.FieldType == typeof(int))
        {
            int currentValue = (int)field.GetValue(this);
            field.SetValue(this, Mathf.Max(0, currentValue - amount));
            myUI.RefreshAllUI();
            return;
        }

        Buff newBuff = Buff.CreateByName(fieldName);
        for (int i = 0; i < activeBuffs.Count; i++)
        {

            if (activeBuffs[i].Bname == newBuff.Bname)
            {
                activeBuffs[i].stack = Mathf.Max(0, activeBuffs[i].stack - amount); // newBuff 제거
                if (buffUIDict.TryGetValue(activeBuffs[i], out BuffUI ui)) 
                {
                    ui.UpdateUI(activeBuffs[i].stack);
                    // 동일한 ChaUI의 버프도 변경.
                    myUI.buffUIDict.TryGetValue(activeBuffs[i], out BuffUI uii); uii.UpdateUI();
                } // 버프 UI 갱신
                return;
            }
        }
        Debug.LogWarning($"필드 '{fieldName}'를 찾을 수 없습니다.");
    }

    public void Consume(Buff newBuff, int amount)
    {
        // activeBuffs[i]가 가진 함수명이 newBuff의 함수명과 일치하는지 확인
        for (int i = 0; i < buffUIDict.Count; i++)
        {
            if (activeBuffs[i].Bname == newBuff.Bname)
            {
                activeBuffs[i].stack = Mathf.Max(0, activeBuffs[i].stack - amount);
                if (buffUIDict.TryGetValue(activeBuffs[i], out BuffUI ui))
                {
                    ui.UpdateUI(activeBuffs[i].stack);
                    myUI.buffUIDict.TryGetValue(activeBuffs[i], out BuffUI uii); uii.UpdateUI();
                }
                return;
            }
        }
        Debug.LogWarning($"버프 '{newBuff.Bname}'를 찾을 수 없습니다.");
    }
    // (Buff newBuff)

    // ----------------- 2. 통합된 처리 (중복 코드 제거) ------------

    //--------------------2-0 드로우, 처리 --------------------

    public virtual void DrawCard(int amount)
    {
        if (this is Ally AA) { AA.DrawCard(amount); }
    }
    public virtual void DiscardCard(int amount, CardPos where = CardPos.Hand)
    {
        if (this is Ally AA) { AA.DiscardCard(amount, where); }
    }
    public virtual void RemoveCard(int amount, CardPos where = CardPos.Hand)
    {
        if (this is Ally AA) { AA.RemoveCard(amount, where); }
    }

    /// <summary> 예상 피해량, 공격전 피격전 효과 발동 | 피해 타입 타겟 공격 </summary>
    public int GetExpectedDamage(int baseDamage, DmgT damageType, F_Cha target = null, bool isTrue = true)
    {
        // 예상 피해량 계산. 공격 피해 계산이므로 공격자 ON
        DamContext DC = new DamContext(baseDamage, damageType, this, target, isTrue);

        // 공격자 버프 및 피격자 버프 확인
        for (int i = activeBuffs.Count - 1; i >= 0; i--) { activeBuffs[i].BeforeAttack(DC); }

        if (target != null)
        { for (int i = target.activeBuffs.Count - 1; i >= 0; i--) { target.activeBuffs[i].BeforeDamaged(DC); } }

        // 최종 피해 Return
        return DC.GetFinalDamage(); // 예상 피해량 확인 후 공격 과정 
    }

    //--------------------2-1. 통합 피해 처리 함수 --------------------

    /// <summary>
    /// 계산된 피해 적용 + 반격 효과 + return DamageResult
    /// </summary>
    public DamageResult ApplyDamage(DamContext dc)
    {

        DamageResult result = new DamageResult();
        result.damageType = dc.DT;
        result.attacker = dc.Attacker;
        result.target = this; // 타겟 지정의 필요가 있습니까? 진짜 모름 | 그 전에 필요한가? 보러감 ㅅㄱ
        result.originalDamage = dc.Damage;
        result.timestamp = Time.frameCount;

        // 피해량 계산
        int processedDamage = dc.GetFinalDamage(); 
        if (processedDamage <= 0) return result; // 피해가 0 이하면 result 내보냄

        result.finalDamage = processedDamage; // 최종 피해값으로 저장.
        int remainingDamage = processedDamage;

        // 2. 방어(Block) 및 보호(SD) 차감 (MP 피해, Pierce는 제외)
        if (dc.DT != DmgT.MP && dc.DT != DmgT.Pierce)
        {
            // Block 차감
            if (Block > 0)
            {
                result.blockDealt = Mathf.Min(Block, remainingDamage);
                Block -= result.blockDealt;
                remainingDamage -= result.blockDealt;
            }

            // SD(Shield) 차감
            if (remainingDamage > 0 && SD > 0)
            {
                result.sdDealt = Mathf.Min(SD, remainingDamage);
                SD -= result.sdDealt;
                remainingDamage -= result.sdDealt;
            }
        }

        // 3. 실제 체력/정신력 차감
        if (remainingDamage > 0)
        {
            if (dc.DT == DmgT.MP || dc.DT == DmgT.TrueMP || dc.DT == DmgT.Both || dc.DT == DmgT.TrueBoth)
            {
                int mpDamage = Mathf.Min(MP, remainingDamage);
                MP -= mpDamage;
                result.mpDealt = mpDamage;

                if (dc.DT == DmgT.MP || dc.DT == DmgT.TrueMP)
                {
                    remainingDamage = 0;
                }
                else
                {
                    remainingDamage -= mpDamage;
                }
            }

            if (remainingDamage > 0)
            {
                int hpDamage = Mathf.Min(HP, remainingDamage);
                result.hpDealt = hpDamage;
                result.overkill = remainingDamage - hpDamage;
                HP = Mathf.Max(0, HP - hpDamage);
            }
        }

        // 4. UI 갱신 및 이펙트
        if (myUI != null) myUI.RefreshAllUI();

        // 피해 텍스트 표시
        int[] Damages = { result.hpDealt, result.blockDealt, result.sdDealt }; // 체력 방어 보호
        FightUIManager.Instance.MakeBadText(this, Damages);

        dc.DamResult = result;
        // 5. AfterDamaged 훅: 피격 후 버프 처리 (반격, 추가 효과 등)
        OnAfterDamagedEvent(dc);

        return result;
    }

    /// <summary>
    /// 피격, 공격 효과 X 순수 피해 | (공격) 피해, 타입 / (추가피해) 피해, 타입, 공격자 
    /// </summary>
    public void TakeDamage(int dam, DmgT DT = DmgT.HP, bool istrue = true)
    {
        ApplyDamage(new DamContext(dam, DT, this, istrue));
    }


    //--------------------2-2. 회복 처리 --------------------

    /// <summary>회복 함수</summary>
    public void Heal(int amount, DmgT healType = DmgT.HP, F_Cha user = null)
    {
        if (healType == DmgT.MP || healType == DmgT.TrueMP)
        {
            MP = Mathf.Min(MaxMP, MP + amount);
            FightUIManager.Instance.MakeGoodText(this, amount, "MP");
        }
        else if (healType == DmgT.TrueBoth || healType == DmgT.Both)
        {
            MP = Mathf.Min(MaxMP, MP + amount);
            HP = Mathf.Min(MaxHP, HP + amount);
            FightUIManager.Instance.MakeGoodText(this, amount, "MP");
            FightUIManager.Instance.MakeGoodText(this, amount, "HP");
        }
        else
        {
            HP = Mathf.Min(MaxHP, HP + amount);
            FightUIManager.Instance.MakeGoodText(this, amount, "HP");
        }

        if (myUI != null) myUI.RefreshAllUI();
    }

    //--------------------3 버프 작업 --------------------

    /// <summary>버프 부여</summary>
    public void AddBuff(Buff newBuff)
    {
        Buff existingBuff = activeBuffs.Find(b => b.Bname == newBuff.Bname);

        if (existingBuff != null)
        {
            // 기존 버프에 스택 추가 — OnUpdate 호출
            existingBuff.OnUpdate(newBuff.stack);

            if (buffUIDict.TryGetValue(existingBuff, out BuffUI ui))
            {
                ui.UpdateUI(existingBuff.stack);

                // myUI의 버프 UI도 existingBuff를 키값으로 바로 찾아서 업데이트 (Find 불필요)
                if (myUI != null && myUI.buffUIDict.TryGetValue(existingBuff, out BuffUI uii))
                {
                    uii.UpdateUI(existingBuff.stack); // UI에 스택 값 전달
                }
                else
                {
                    print($"[오류] F_ChaUI 딕셔너리에 버프가 없습니다: {existingBuff.Bname}");
                }
            }
        }
        else
        {
            newBuff.OnActivate();
            activeBuffs.Add(newBuff);

            // 없다면 생성.
            if (buffUIPrefab != null && buffUIParent != null)
            {
                MakenewUI(newBuff, newBuff.stack);
            }
        }
        UpdateBuffPosition();
    }

    public void MakenewUI(Buff newBuff, int amount)
    {
        if (buffUIPrefab != null && buffUIParent != null)
        {
            BuffUI spawnedUI = Instantiate(buffUIPrefab, buffUIParent).GetComponent<BuffUI>();
            spawnedUI.Setup(newBuff); buffUIDict.Add(newBuff, spawnedUI); // 새 버프와 UI를 딕셔너리에 연결, UI 업데이트
            myUI.MakenewUI(spawnedUI, newBuff); // myUI에도 생성.
        }
    }

    /// <summary>버프 제거</summary>
    public void RemoveBuff(Buff targetBuff, bool arrangeUI = true)
    {
        if (activeBuffs.Contains(targetBuff))
        {
            targetBuff.OnDeactivate();
            activeBuffs.Remove(targetBuff);

            // 1. 내 로컬 UI 파괴
            if (buffUIDict.TryGetValue(targetBuff, out BuffUI ui))
            {
                Destroy(ui.gameObject);
                buffUIDict.Remove(targetBuff);
            }

            // 2. myUI(글로벌/동기화 UI)에도 동일하게 삭제 요청! (고스트 버프 해결)
            if (myUI != null)
            {
                Buff existingBuffInMyUI = myUI.activeBuffs.Find(b => b.Bname == targetBuff.Bname);
                if (existingBuffInMyUI != null)
                {
                    myUI.RemoveBuff(existingBuffInMyUI, arrangeUI); // myUI의 버프도 지워줌
                }
            }
        }

        // 턴 종료 시점처럼 대량으로 지워질 때가 아닐 때만 즉시 정렬
        if (arrangeUI)
        {
            UpdateBuffPosition();
        }
    }

    /// <summary> 버프 여부 스택 확인 함수 (버프 리스트에서 해당 이름의 버프를 찾아서 스택 수를 반환, 없으면 0) </summary>
    private int BuffCheck(string valueName)
    {
        for (int i = 0; i < activeBuffs.Count; i++)
        {
            if (activeBuffs[i].Bname == valueName) { return activeBuffs[i].stack; }
        }
        return 0;
    }
    public void UpdateBuffPosition()
    {
        myUI.UpdateBuffPosition(); // 캐릭터 선택시 업데이트 되는 버프 포지션
        // 버프 UI의 위치를 업데이트하는 로직 (예: 캐릭터가 이동할 때)
        int index = activeBuffs.Count;
        int spacing = 50; // 버프 UI 간의 간격 (예시값)
        foreach (var kvp in buffUIDict)
        {
            BuffUI ui = kvp.Value;
            if (ui != null)
            {
                if (ui.gameObject != null)
                {
                    // 버프 UI의 위치 계산 (예: 캐릭터의 위치를 기준으로 일정 간격으로 배치)
                    // 1개 x값: 0, 2개 x값: -spacing/2, spacing/2, 3개 x값: -spacing, 0, spacing ... 이런 식으로 중앙 정렬하면서 간격 조정
                    Vector3 targetPos = new Vector3((index - 1) * spacing - (activeBuffs.Count - 1) * spacing / 2, 0, 0);
                    // 예시 계산식: index에 따라 간격을 조정하면서 중앙 정렬
                    index--;
                }
            }
            // ui.transform.position = ...; // 원하는 위치 계산 후 적용
        }
    }

    // --------------------- 3. 턴 및 버프 사이클 ---------------------

    public virtual void OnTurnStart()
    { 
        bool needUIRearrange = false;

        // Snapshot 생성(new List) 없이 뒤에서부터 순회하면 리스트 삭제 시 인덱스가 꼬이지 않아 안전합니다.
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            string bName = buff.Bname;

            buff.OnTurnStart(); // 내부에서 RemoveBuff가 호출될 수 있음

            // 버프가 삭제되지 않고 살아남았다면 UI 갱신
            if (activeBuffs.Contains(buff))
            {
                if (buffUIDict.TryGetValue(buff, out BuffUI ui))
                {
                    ui.UpdateUI(buff.stack);
                }

                // myUI 동기화 (Null 체크 필수!)
                if (myUI != null)
                {
                    Buff existingBuff1 = myUI.activeBuffs.Find(b => b.Bname == bName);
                    // existingBuff1이 null이 아닐 때만 TryGetValue 실행
                    if (existingBuff1 != null && myUI.buffUIDict.TryGetValue(existingBuff1, out BuffUI uii))
                    {
                        uii.UpdateUI();
                    }
                }
            }
            else
            {
                // 버프가 하나라도 삭제되었다면 UI 재배치 플래그 ON
                needUIRearrange = true;
            }
        }

        // 턴 종료 처리가 모두 끝난 후, 버프가 지워진 적이 있다면 딱 한 번만 UI 정렬
        if (needUIRearrange)
        {
            UpdateBuffPosition();
        }
    }

    public virtual void OnTurnEnd()
    {
        bool needUIRearrange = false;

        // Snapshot 생성(new List) 없이 뒤에서부터 순회하면 리스트 삭제 시 인덱스가 꼬이지 않아 안전합니다.
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            string bName = buff.Bname;

            buff.OnTurnEnd(); // 내부에서 RemoveBuff가 호출될 수 있음

            // 버프가 삭제되지 않고 살아남았다면 UI 갱신
            if (activeBuffs.Contains(buff))
            {
                if (buffUIDict.TryGetValue(buff, out BuffUI ui))
                {
                    ui.UpdateUI(buff.stack);
                }

                // myUI 동기화 (Null 체크 필수!)
                if (myUI != null)
                {
                    Buff existingBuff1 = myUI.activeBuffs.Find(b => b.Bname == bName);
                    // existingBuff1이 null이 아닐 때만 TryGetValue 실행
                    if (existingBuff1 != null && myUI.buffUIDict.TryGetValue(existingBuff1, out BuffUI uii))
                    {
                        uii.UpdateUI();
                    }
                }
            }
            else
            {
                // 버프가 하나라도 삭제되었다면 UI 재배치 플래그 ON
                needUIRearrange = true;
            }
        }

        // 턴 종료 처리가 모두 끝난 후, 버프가 지워진 적이 있다면 딱 한 번만 UI 정렬
        if (needUIRearrange)
        {
            UpdateBuffPosition();
        }
    }


    private int OnBeforeDamagedEvent(DamContext dc)
    {
        // 피격자의 버프들이 피해를 수정
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].BeforeDamaged(dc);
        }
        return dc.GetFinalDamage();
    }

    private void OnAfterDamagedEvent(DamContext dc)
    {
        // 피격 후 버프 훅 실행
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            activeBuffs[i].AfterDamaged(dc);
        }
    }

    // --------------- 4. 애니메이션 및 이동 ( DOTween ) ---------------
    // 기존의 코드를 유지하되, 하드코딩된 숫자를 변수로 빼서 유연하게 만들었습니다.

    public IEnumerator MoveToTarget(F_Cha target, float offset = 2f)
    {
        isActing = true;
        if (target == null) { yield break; }
        // 타겟이 내 오른쪽에 있는지 왼쪽에 있는지 판별
        float direction = transform.position.x < target.transform.position.x ? -1f : 1f;
        Vector3 targetPos = target.transform.position + new Vector3(offset * direction, 0, 0);

        // 방향 뒤집기
        ChaSD.flipX = transform.position.x < targetPos.x;

        yield return transform.DOMove(targetPos, 0.3f).WaitForCompletion();
    }

    public IEnumerator ReturnToOrigin()
    {
        ChaSD.flipX = originPos.x >= transform.position.x; // 원래 위치가 현재 위치보다 오른쪽에 있으면 뒤집고, 왼쪽에 있으면 뒤집지 않음 (기본 방향이 오른쪽이라고 가정)
        yield return transform.DOMove(originPos, 0.3f).WaitForCompletion();
        ChaSD.flipX = true; // 기본 방향으로 복구, 나중에 캐릭터마다 기본 방향이 다르면 변수로 빼서 관리하는 것이 좋습니다.
        isActing = false;
    }
    //  
    public IEnumerator DeadEffect()
    {
        isActing = false;
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            ChaSD.color = new Color(ChaSD.color.r, ChaSD.color.g, ChaSD.color.b, Mathf.Lerp(1f, 0f, t / 1f));
            yield return null;
        }
        yield return null;
    
    }

    public int ChaAni(CardEffect effect, int EC)
    {
        // 1. 특수 스킬 및 행동.

        //  string CN = card.Data.cardName; // else if (CN == "난도" && effect is DamageEffect)
        //  EC == 0                        { user.ChaAni("IsAttack"); }

        // 2.  기본 행동
        if (effect is DamageEf || effect is AddDamageEf) { ChaAni("IsAttack"); }
        else if (effect is ShieldEf || effect is BlockEf) { ChaAni("IsShlied"); }
        else { return 0; }
        return 1;
    }

    public void ChaAni(string t)
    {
        myUI.SChaAni.SetTrigger(t);
    }
}

/// <summary>
/// 피해 결과를 저장하는 구조체
/// 실제 입은/입힌 피해를 기록하여 로깅, 통계, 리플레이 등에 활용 가능
/// </summary>
public struct DamageResult
{
    /// <summary>원본 피해량</summary>
    public int originalDamage;

    /// <summary>최종 피해량 (버프/디버프 적용 후)</summary>
    public int finalDamage;

    /// <summary>Block으로 흡수된 피해</summary>
    public int blockDealt;

    /// <summary>SD(Shield/보호막)로 흡수된 피해</summary>
    public int sdDealt;

    /// <summary>실제 체력에 입은 피해</summary>
    public int hpDealt;

    /// <summary>실제 정신력에 입은 피해</summary>
    public int mpDealt;

    /// <summary>오버킬 (체력이 0 이하로 내려간 양)</summary>
    public int overkill;

    /// <summary>피해 타입</summary>
    public DmgT damageType;

    /// <summary>공격자</summary>
    public F_Cha attacker;

    /// <summary>피격자</summary>
    public F_Cha target;

    /// <summary>타임스탬프 (턴 수, 실시간 등)</summary>
    public int timestamp;

    public override string ToString()
    {
        return $"[{damageType}] 원본: {originalDamage} -> 최종: {finalDamage} | Block: {blockDealt} | SD: {sdDealt} | HP: {hpDealt} | MP: {mpDealt}";
    }
}

/// <summary>
/// 피해 계산 및 전달용 컨텍스트
/// BeforeAttack, BeforeDamaged 등의 Buff 훅에서 수정 가능
/// </summary>
public class DamContext
{
    // 필수 --

    /// <summary>피해량 (기본)</summary>
    public int Damage { get; set; } /// <summary>피해 타입</summary>
    public DmgT DT { get; set; } /// <summary>공격 주체, 피해를 입힌 자</summary>
    public F_Cha Attacker { get; set; } /// <summary>피격 주체, 피해를 받은 자</summary>
    public F_Cha Target { get; set; }
    
    ///<summary>미리보기 여부</summary>
    public bool IsPreview { get; set; }    /// <summary>고정 추가 피해</summary>
    public int PlusDamage { get; set; } = 0;     /// <summary>배율 (1.0 = 100%)</summary>
    public float PercentDamage { get; set; } = 1f;
    /// <summary>최대/최소 피해 제한 (-1 = 제한 없음)</summary>
    public int FixDamage { get; set; } = -1;
    /// <summary>치명타 여부</summary>
    public bool isCritical { get; set; } = false;

    public DamageResult DamResult { get; set; }
    /// <summary>[피해] 피해, 타입, 공격자, 피격자, Preview 여부 </summary>
    public DamContext(int damage, DmgT DT, F_Cha attacker, F_Cha target, bool IsPre = false)
    {
        this.Damage = damage;
        this.DT = DT;
        this.Attacker = attacker;
        this.Target = target;
        this.IsPreview = IsPre;
    }
    /// <summary>[추가피해] 피해, 피해타입, 피격자, Preview 여부</summary>
    public DamContext(int damage, DmgT DT, F_Cha target, bool IsPre = false)
    { 
        this.Damage = damage;
        this.DT = DT;
        this.Attacker = null;
        this.Target = target;
        this.IsPreview = IsPre;
    }

    /// <summary>최종 피해량을 계산 (버프/디버프 적용 후)</summary>
    public int GetFinalDamage()
    {
        if (FixDamage != -1) { return FixDamage; } // 고정 피해량 정리

        if (PercentDamage <= 0) { PercentDamage = 0f; } // 곱해지는 값이 0 이하면 안 됨
        int calculated = Mathf.RoundToInt((Damage + PlusDamage) * PercentDamage);
        return Mathf.Max(0, calculated);
    }
    ///
}