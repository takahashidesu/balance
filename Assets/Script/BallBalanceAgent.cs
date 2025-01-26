using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class BallBalanceAgent : Agent
{
    [SerializeField] private Transform ball; // ボールのTransform
    [SerializeField] private Rigidbody ballRigidbody; // ボールのRigidbody
    [SerializeField] private Transform platform; // 棒のTransform
    [SerializeField] private float forceMultiplier = 10f; // 棒の動かす力

    private Vector3 initialBallPosition;
    private Vector3 initialPlatformPosition;

    public override void Initialize()
    {
        // 初期位置を保存
        initialBallPosition = ball.position;
        initialPlatformPosition = platform.position;
    }

    public override void OnEpisodeBegin()
    {
        // エピソード開始時にボールと棒を初期位置に戻す
        ballRigidbody.velocity = Vector3.zero;
        ball.position = initialBallPosition;
        platform.position = initialPlatformPosition;
        platform.rotation = Quaternion.identity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ボールの位置と速度を観測
        sensor.AddObservation(ball.localPosition); // ボールの位置
        sensor.AddObservation(ballRigidbody.velocity); // ボールの速度

        // 棒の傾きを観測
        sensor.AddObservation(platform.rotation.eulerAngles); // 棒の回転角
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 棒の傾きを制御するアクションを取得
        float moveX = actions.ContinuousActions[0]; // X軸方向
        float moveZ = actions.ContinuousActions[1]; // Z軸方向

        // 棒に力を加えて動かす
        platform.Rotate(new Vector3(moveX, 0, moveZ) * forceMultiplier * Time.deltaTime);

        // 報酬計算
        if (Mathf.Abs(ball.localPosition.x) > 5 || Mathf.Abs(ball.localPosition.z) > 5)
        {
            // ボールが棒から落ちたらペナルティを与えてエピソード終了
            SetReward(-1f);
            EndEpisode();
        }
        else
        {
            // ボールが中央付近にあるほど報酬を与える
            float distanceToCenter = Vector3.Distance(ball.localPosition, Vector3.zero);
            SetReward(1f - distanceToCenter / 5f); // 中心に近いほど高得点
        }
    }
}