using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    //攻撃対象
    [SerializeField] private GameObject targetObj = null;
    //火花エフェクト
    [SerializeField] private GameObject sparkEffect = null;
    //チャージエフェクト
    [SerializeField] private GameObject chargeEffect = null;
    //武器
    [SerializeField] private GameObject weponObj = null;
    //UIマネージャー
    [SerializeField] private UIManager uIManager = null;
    //体力
    [SerializeField] private float hitPoint = 150;
    //ガードポイント
    [SerializeField] private float guradPoint = 100;
    //攻撃力
    [SerializeField] private float attack = 0f;
    //攻撃距離
    [SerializeField] private float attackRange = 0f;
    //攻撃範囲
    [SerializeField] private float attackAngle = 90.0f;

    //各SE
    [SerializeField] private AudioClip seGuard = null;
    [SerializeField] private AudioClip seAttack = null;
    [SerializeField] private AudioClip seKick = null;
    [SerializeField] private AudioClip seSlash = null;
    [SerializeField] private AudioClip seDown = null;
    

    //最大体力
    private float maxHpPoint;
    //最大ガード値
    private float maxGuradPoint;
    //キャラクターアニメーション
    private Animator animator;
    //強攻撃のチャージ時間によるボーナス
    private float chargeBonus;
    //強攻撃のチャージ中かどうか
    private bool isCharge;
    private GameObject cEffect;
    private RaycastHit raycasthit;
    private Vector3 initialPos;
    private Vector3 initialRot;
    private bool interval = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    virtual protected void Start()
    {
        initialPos = this.transform.position;
        initialRot = this.transform.eulerAngles;
        //ガード値・体力の最大を設定
        maxHpPoint = hitPoint;
        maxGuradPoint = guradPoint;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetBool("isGuard"))
        {
            if (guradPoint <= maxGuradPoint)
            {
                guradPoint += 1;
                uIManager.GuradBarUpdate(guradPoint, maxGuradPoint);
            }

        }
    }
    public void Initialize()
    {
        this.transform.position = initialPos;
        this.transform.localEulerAngles = initialRot;
        animator.SetBool("isGuard", false);
        animator.SetBool("isStun", false);
        animator.ResetTrigger("SAttack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("Damaged");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Kick");
        interval = false;

        hitPoint = maxHpPoint;
        guradPoint = maxGuradPoint;
        uIManager.GuradBarUpdate(guradPoint, maxGuradPoint);
        uIManager.HpBarUpdate(hitPoint, maxHpPoint);

    }
    //攻撃が当たったかどうかの確認
    private bool HitCheck()
    {
        RaycastHit[] hits;
        hits = Physics.SphereCastAll(transform.position, attackRange, transform.forward);
        foreach (RaycastHit hit in hits)
        {
            float angle = Vector3.Angle(hit.transform.position - transform.position, transform.forward);
            if (hit.transform.gameObject == targetObj)
            {
                float distance = Vector3.Distance(transform.position, targetObj.transform.position);
                if (angle <= attackAngle / 2 && distance <= attackRange)
                {
                    raycasthit = hit;
                    return true;

                }
            }
        }
        return false;
    }
    public void ReceiveDamage(float damage)
    {
        //Debug.Log(damage);
        if (cEffect) { Destroy(cEffect); ; }
        //スタン状態のときに攻撃を受けたらスタンを解除する
        if (animator.GetBool("isStun")) { animator.SetBool("isStun", false); }

        //ダメージ分体力を減らす
        //ガードしていた場合はダメージを軽減する
        if (animator.GetBool("isGuard"))
        {
            
            damage /= 3;
            hitPoint -= damage;
            //ガード値の減少
            guradPoint -= damage * 5;
            //ガード値が０以下になったらスタン状態になる
            if (guradPoint < 0)
            {
                guradPoint = 0;
                animator.SetBool("isGuard", false);
                animator.SetBool("isStund", true);
            }
            //エフェクトの発生
            Vector3 vector = new Vector3(0, 0.3f, 0);
            Instantiate(sparkEffect, weponObj.transform.position + vector, weponObj.transform.rotation);
            //ガードゲージの更新
            uIManager.GuradBarUpdate(guradPoint, maxGuradPoint);
            audioSource.PlayOneShot(seGuard);
        }
        else
        {
            //Debug.Log("Player damaged");
            animator.SetTrigger("Damaged");
            //アニメーションのパラメーターを初期化
            //animator.ResetTrigger("Damaged");
            animator.ResetTrigger("Attack");
            animator.SetBool("SAttack", false);
            animator.ResetTrigger("Kick");
            hitPoint -= damage;
        }
        //体力が０以下になったら死亡する
        if (hitPoint <= 0)
        {
            interval = true;
            //Debug.Log(interval);
            hitPoint = 0; ;
            animator.SetTrigger("Death");
            targetObj.SendMessage("RoundWin");
            audioSource.PlayOneShot(seDown);
            //animator.SetBool("isDead", true);
        }

        //体力ゲージの更新
        uIManager.HpBarUpdate(hitPoint, maxHpPoint);
        //ノックバック
        this.transform.Translate(0, 0, -0.3f);
    }
    public void ReceiveKick(float damage)
    {
        audioSource.PlayOneShot(seKick);
        //Debug.Log("AAA");
        //ガードしていた場合スタン状態になる
        if (animator.GetBool("isGuard"))
        {
            animator.SetBool("isGuard", false);
            animator.SetBool("isStun", true);
            animator.ResetTrigger("Damaged");
        }
        else
        {
            //ダメージ分体力を減らす
            animator.SetTrigger("Damaged");
            hitPoint -= damage;
            ////体力が０以下になったら死亡する
            if (hitPoint <= 0)
            {
                interval = true;
                hitPoint = 0; ;
                animator.SetTrigger("Death");
                targetObj.SendMessage("RoundWin");
                //animator.SetBool("isDead", true);
            }
        }
    }
    public void RoundWin()
    {
        uIManager.RoundWin();
        interval = true;
    }
    public float GetAttackRenge()
    {
        return attackRange;
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
   
    //弱攻撃
    void Attack(float mag)
    {
        audioSource.PlayOneShot(seAttack);
        if (HitCheck() && !interval)
        {
            raycasthit.transform.gameObject.SendMessage("ReceiveDamage", attack * mag);
            //Debug.Log("Player Attack");
        }
    }
    //強攻撃
    void SAttack(float mag)
    {
        Destroy(cEffect);
        isCharge = false;
        audioSource.PlayOneShot(seSlash);
        if (HitCheck() && !interval)
        {
            //Debug.Log(chargeBonus);
            raycasthit.transform.gameObject.SendMessage("ReceiveDamage", attack * mag + chargeBonus);
            //Debug.Log("Player SAttack");
        }
        chargeBonus = 0;
    }
    //キック
    void Kick(float mag)
    {
        if (HitCheck() && !interval)
        {
            raycasthit.transform.gameObject.SendMessage("ReceiveKick", attack * mag);
        }
    }
    void ComboEnd()
    {
        animator.ResetTrigger("Attack");
    }
    void Charge()
    {
        isCharge = true;
        animator.SetFloat("SAttackSpeed", 0);
        //エフェクトの発生
        Vector3 vector = new Vector3(0, 0.8f, 0);
        cEffect = Instantiate(chargeEffect, this.transform.position + vector, this.transform.rotation);
        StartCoroutine(DelayProcess(2f));
        StartCoroutine(DelayProcess(4f));
    }
    void RecoveryStun()
    {
        animator.SetBool("isStun", false);
        //Debug.Log("check");
    }
    void Death()
    {
        
        targetObj.SendMessage("Initialize");
        //targetObj.SendMessage("");
        Initialize();
        //uIManager.RoundWin();
    }
    //攻撃を受けた時の処理

    IEnumerator DelayProcess(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (isCharge)
        {
            //Debug.Log("charge");
            chargeBonus += 10;
        }
        //chargeEffect.ChangeColor(col);
    }
}
