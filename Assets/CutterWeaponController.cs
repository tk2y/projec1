using UnityEngine;
using System.Collections;
using System.Collections.Generic; // HashSetを使うために必要
using System; // Actionを使うために必要

public class CutterWeaponController : MonoBehaviour
{
    [Tooltip("この武器がダメージを与える対象のタグ")]
    [SerializeField] private string[] hitTargetTags = { "Player", "Cuuerp", "Swordp" };

    // 1回の攻撃でヒットしたオブジェクトを記録し、多段ヒットを防ぐためのリスト
    private HashSet<Collider> hitObjects;

    void Awake()
    {
        // 最初は非表示にしておく
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 武器を投げる処理を開始する
    /// </summary>
    /// <param name="targetPosition">目的地のワールド座標</param>
    /// <param name="owner">持ち主(Cutter)のTransform</param>
    /// <param name="duration">目的地に到達するまでの時間</param>
    /// <param name="onAttackFinish">攻撃（往復）が完了したときに呼ばれる処理</param>
    public void Throw(Vector3 targetPosition, Transform owner, float duration, Action onAttackFinish)
    {
        // 攻撃開始時にヒットリストを初期化
        hitObjects = new HashSet<Collider>();

        // 武器をCutterの位置に表示する
        transform.position = owner.position;
        gameObject.SetActive(true);

        // 移動シーケンスのコルーチンを開始
        StartCoroutine(MoveSequence(targetPosition, owner, duration, onAttackFinish));
    }

    /// <summary>
    /// 武器の往復移動を制御するコルーチン
    /// </summary>
    private IEnumerator MoveSequence(Vector3 targetPosition, Transform owner, float duration, Action onAttackFinish)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // --- 1. 目的地へ向かう（行き） ---
        while (elapsedTime < duration)
        {
            // Lerpを使って滑らかに移動
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 1フレーム待つ
        }
        transform.position = targetPosition; // 誤差をなくすため、ぴったり合わせる

        // --- 2. Cutterの元へ戻る（帰り） ---
        elapsedTime = 0f;
        Vector3 returnStartPosition = transform.position;
        while (elapsedTime < duration)
        {
            // Cutterが動く可能性を考慮し、常に最新のowner.positionを参照する
            transform.position = Vector3.Lerp(returnStartPosition, owner.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // 1フレーム待つ
        }

        // --- 3. 攻撃終了処理 ---
        gameObject.SetActive(false); // 武器を非表示にする
        onAttackFinish?.Invoke();    // CutterControllerに攻撃が完了したことを通知する
    }

    /// <summary>
    /// 当たり判定 (Trigger)
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 既にこの攻撃でヒット済みのオブジェクトは無視する
        if (hitObjects.Contains(other))
        {
            return;
        }

        // 攻撃対象のタグかどうかをチェック
        bool isTarget = false;
        foreach (string tag in hitTargetTags)
        {
            if (other.CompareTag(tag))
            {
                isTarget = true;
                break;
            }
        }

        if (isTarget)
        {
            Debug.Log(other.name + " に攻撃がヒット！");
            hitObjects.Add(other); // ヒットリストに追加して、同じ敵への多段ヒットを防ぐ

            // ここにダメージを与える処理などを記述します
            // 例: other.GetComponent<PlayerHealth>()?.TakeDamage(10);
        }
    }
}