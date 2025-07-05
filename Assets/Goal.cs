using UnityEngine;

using TMPro; // TextMeshProを使用するために必要

public class GoalController : MonoBehaviour

{

    // InspectorからゲームクリアUIを割り当てるための変数

    public GameObject gameClearUI;

    void Start()

    {

        //念のため、ゲーム開始時にもUIを非表示にしておく

        if (gameClearUI != null)

        {

            gameClearUI.SetActive(false);

        }

    }

    // 他のColliderがこのオブジェクトのトリガーに侵入したときに呼ばれるメソッド

    private void OnTriggerEnter(Collider other)

    {

        // 接触したオブジェクトのタグが "Player" かどうかをチェック

        if (other.CompareTag("Player"))

        {

            Debug.Log("ゴール！"); // 動作確認用のログ

            // ゲームクリアUIがあれば表示する

            if (gameClearUI != null)

            {

                gameClearUI.SetActive(true);

            }

            // (任意) ゲームの時間を止めて、プレイヤーの動きなどを停止する

            Time.timeScale = 0f;

        }

    }

}
