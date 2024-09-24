// using UnityEngine;
// using UnityEngine.SceneManagement;
// using System.Collections;
//
// public class SceneController : MonoBehaviour
// {
//     public float timeToWait = 10.0f; // 设置时间值
//
//     private void Start()
//     {
//         StartCoroutine(WaitAndStopScene(timeToWait));
//     }
//
//     private IEnumerator WaitAndStopScene(float waitTime)
//     {
//         yield return new WaitForSeconds(waitTime);
//
//         // 停止场景
//         Time.timeScale = 0; // 暂停时间
//         
//         // 可以选择在这里加载一个结束场景
//         // SceneManager.LoadScene("GameOverScene");
//     }
// }
