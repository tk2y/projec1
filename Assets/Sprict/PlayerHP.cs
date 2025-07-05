using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player, CutterP, SwordPの体力を共有し、特定のタグを持つオブジェクトからのダメージを管理します。
/// このスクリプトは以下の2つの役割を持ちます。
/// 1. 空のGameObjectにアタッチし、体力とダメージ設定を管理する「管理オブジェクト」
/// 2. ダメージを受けるキャラクター(Player, CutterP, SwordP)にアタッチし、当たり判定を行う「判定コンポーネント」
/// </summary>
public class SharedHealthManager : MonoBehaviour
{
    #region シングルトン (管理オブジェクトへのアクセス用)
    // このクラスの唯一のインスタンスを保持します。
    public static SharedHealthManager Instance { get; private set; }
    #endregion

    #region Inspector設定項目 (管理オブジェクト側で設定します)
    [Header("体力設定")]
    [Tooltip("共有する体力の最大値")]
    public float maxHealth = 100f;

    [Header("ダメージ設定")]
    [Tooltip("Cutterタグを持つオブジェクトから受けるダメージ")]
    public float cutterDamage = 10f;
    [Tooltip("Swordタグを持つオブジェクトから受けるダメージ")]
    public float swordDamage = 15f;
    [Tooltip("CutterWeaponタグを持つオブジェクトから受けるダメージ")]
    public float cutterWeaponDamage = 20f;
    [Tooltip("SwordWeaponタグを持つオブジェクトから受けるダメージ")]
    public float swordWeaponDamage = 25f;
    #endregion

    #region 共有データ
    // 現在の共有体力を保持する変数
    private float _currentHealth;

    // 外部から現在の体力を安全に読み取るためのプロパティ
    public float CurrentHealth => _currentHealth;
    #endregion

    /// <summary>
    /// スクリプトのインスタンスがロードされたときに呼び出されます。
    /// </summary>
    void Awake()
    {
        // --- シングルトンパターンの実装 ---
        // まだ管理インスタンス(Instance)が設定されていない場合
        if (Instance == null)
        {
            // このインスタンスを管理インスタンスとして設定
            Instance = this;
            // 体力を初期化
            _currentHealth = maxHealth;
            Debug.Log("体力管理マネージャーが初期化されました。");

            // (任意)シーンをまたいでもこの管理オブジェクトを破壊しないようにする場合
            // DontDestroyOnLoad(gameObject);
        }
        // 既に管理インスタンスが存在する場合は、このインスタンスは当たり判定用とみなす
    }

    /// <summary>
    /// ダメージを適用するメソッド（管理インスタンスから呼び出されます）
    /// </summary>
    /// <param name="damageAmount">受けるダメージ量</param>
    public void ApplyDamage(float damageAmount)
    {
        // 既に体力が0以下なら何もしない
        if (_currentHealth <= 0) return;

        _currentHealth -= damageAmount;
        Debug.Log($"ダメージ！ {damageAmount} のダメージを受けました。残り体力: {_currentHealth}");

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// 体力が0になった時の処理
    /// </summary>
    private void Die()
    {
        Debug.LogWarning("体力が0になりました。");
        // ここにゲームオーバー処理や、キャラクターが破壊されるなどの処理を記述できます。
    }

    /// <summary>
    /// トリガーによる衝突判定（当たり判定用コンポーネントとして機能）
    /// </summary>
    /// <param name="other">接触した相手のCollider</param>
    private void OnTriggerEnter(Collider other)
    {
        // このインスタンスが管理用インスタンス自身の場合は、衝突判定を行わない
        if (Instance == this)
        {
            return;
        }

        // --- このスクリプトがアタッチされたオブジェクトがダメージを受けるべきか判定 ---
        List<string> damageableTags = new List<string> { "Player", "CutterP", "SwordP" };
        if (!damageableTags.Contains(gameObject.tag))
        {
            // Player, CutterP, SwordP 以外なら処理を中断
            return;
        }

        // --- 接触した相手のタグに応じてダメージを決定 ---
        float damageToTake = 0f;
        switch (other.tag)
        {
            case "Cutter":
                damageToTake = Instance.cutterDamage;
                break;
            case "Sword":
                damageToTake = Instance.swordDamage;
                break;
            case "CutterWeapon":
                damageToTake = Instance.cutterWeaponDamage;
                break;
            case "SwordWeapon":
                damageToTake = Instance.swordWeaponDamage;
                break;
            default:
                // ダメージを与えるタグ以外に接触した場合は何もしない
                return;
        }

        // ダメージ量が0より大きい場合、管理インスタンスにダメージ処理を依頼
        if (damageToTake > 0)
        {
            Debug.Log($"{gameObject.name}({gameObject.tag}) が {other.name}({other.tag}) と接触しました。");
            Instance.ApplyDamage(damageToTake);
        }
    }
}