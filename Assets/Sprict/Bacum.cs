using UnityEngine;

public class Bacum : MonoBehaviour
{
    // このゲームオブジェクトが他のトリガーコライダーに接触したときに呼び出されるメソッド
    private void OnTriggerEnter(Collider other)
    {
        // 接触した相手のタグが "Cutter" の場合
        if (other.CompareTag("Cutter"))
        {
            Debug.Log("Cutterに接触。PlayerとCutterPの役割を交換します。");
            SwapRolesAndCamera("Player", "CutterP");
        }
        // 接触した相手のタグが "Sword" の場合
        else if (other.CompareTag("Sword"))
        {
            Debug.Log("Swordに接触。PlayerとSwordPの役割を交換します。");
            SwapRolesAndCamera("Player", "SwordP");
        }
    }

    /// <summary>
    /// 指定された2つのタグを持つオブジェクトの役割（位置、向き、タグ）を交換し、
    /// MainCameraを新しいプレイヤーに追従させます。
    /// </summary>
    /// <param name="playerTag">現在のプレイヤーのタグ（常に "Player"）</param>
    /// <param name="otherTag">交換相手のタグ（"CutterP" または "SwordP"）</param>
    private void SwapRolesAndCamera(string playerTag, string otherTag)
    {
        // タグを使ってシーンからオブジェクトを検索
        GameObject currentPlayerObj = GameObject.FindWithTag(playerTag);
        GameObject otherObj = GameObject.FindWithTag(otherTag);

        // 'MainCamera' タグを使ってメインカメラを検索
        Camera mainCamera = Camera.main;
        GameObject mainCameraObj = null;

        if (mainCamera != null)
        {
            mainCameraObj = mainCamera.gameObject;
        }

        // 必要なオブジェクトが全て見つかったか確認
        if (currentPlayerObj == null || otherObj == null || mainCameraObj == null)
        {
            if (currentPlayerObj == null) Debug.LogError($"エラー: タグ '{playerTag}' を持つオブジェクトが見つかりません。");
            if (otherObj == null) Debug.LogError($"エラー: タグ '{otherTag}' を持つオブジェクトが見つかりません。");
            if (mainCameraObj == null) Debug.LogError("エラー: 'MainCamera' タグを持つカメラが見つかりません。");
            return;
        }

        // --- 1. 位置(position)と向き(rotation)の交換 ---
        Vector3 tempPosition = currentPlayerObj.transform.position;
        Quaternion tempRotation = currentPlayerObj.transform.rotation;

        currentPlayerObj.transform.position = otherObj.transform.position;
        currentPlayerObj.transform.rotation = otherObj.transform.rotation;

        otherObj.transform.position = tempPosition;
        otherObj.transform.rotation = tempRotation;

        // --- 2. タグの交換 ---
        // これにより、どちらが現在のプレイヤーであるかが明確になる
        currentPlayerObj.tag = otherTag;
        otherObj.tag = playerTag; // otherObjが新しいプレイヤーになる

        // --- 3. MainCameraの親を、新しいプレイヤーに付け替え ---
        // 新しく "Player" タグが付いたオブジェクト (元のotherObj) にカメラを追従させる
        mainCameraObj.transform.SetParent(otherObj.transform);

        Debug.Log($"役割交換完了。新しいプレイヤーは '{otherObj.name}' です。");

        // (推奨) カメラの相対位置と向きをリセットして、視点を安定させます
        // 好みの視点になるように、以下の数値を調整してください。
        mainCameraObj.transform.localPosition = new Vector3(0, 5f, -10f); // 例: キャラクターの少し後ろ上
        mainCameraObj.transform.localRotation = Quaternion.Euler(20f, 0, 0); // 例: 少し下を向く
    }
}