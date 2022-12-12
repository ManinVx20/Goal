using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace StormDreams
{
    public class MatchManager : NetworkBehaviour
    {
        public static MatchManager Instance;

        [SerializeField]
        private Transform[] _playerStartPoints;
        [SerializeField]
        private Transform _ballStartPoint;
        [SerializeField]
        private GameObject _ballPrefab;
        [SerializeField]
        private float _prematchTimeLength = 5.0f;
        [SerializeField]
        private float _matchTimeLength = 60.0f;

        [SyncObject]
        private readonly SyncTimer _prematchTimer = new();
        [SyncObject]
        private readonly SyncTimer _matchTimer = new();

        [SyncVar(OnChange = nameof(ScorePlayerOne_OnChange))]
        private int _scorePlayerOne = 0;
        [SyncVar(OnChange = nameof(ScorePlayerTwo_OnChange))]
        private int _scorePlayerTwo = 0;

        private bool _prematchTimerFinished;
        private bool _matchTimerFinished;

        private GameObject _currentBall;

        private void Awake()
        {
            Instance = this;

            _prematchTimer.OnChange += PrematchTimer_OnChange;
            _matchTimer.OnChange += MatchTimer_OnChange;
        }

        private void OnDestroy()
        {
            _prematchTimer.OnChange -= PrematchTimer_OnChange;
            _matchTimer.OnChange -= MatchTimer_OnChange;
        }

        private void Update()
        {
            if (!_prematchTimerFinished)
            {
                _prematchTimer.Update(Time.deltaTime);
                ViewManager.Instance.Get<PrematchView>().SetCountdownTimeText(_prematchTimer.Remaining);
            }

            if (!_matchTimerFinished)
            {
                _matchTimer.Update(Time.deltaTime);
                ViewManager.Instance.Get<MatchView>().SetMatchTimeText(_matchTimer.Remaining);
            }
        }

        [Server]
        public void BeginMatch()
        {
            _prematchTimer.StartTimer(_prematchTimeLength);

            for (int i = 0; i < GameManager.Instance.ConnectedPlayers.Count; i++)
            {
                GameManager.Instance.ConnectedPlayers[i].ControlledDummy.SetDummyStartPositionAndRotation(_playerStartPoints[i].position, _playerStartPoints[i].rotation);
            }

            SetDummiesInput(false);
        }

        [Server]
        private void StartMatch()
        {
            SpawnBall();

            SetDummiesInput(true);
        }

        [Server]
        private void EndMatch()
        {
            StopAllCoroutines();

            DespawnBall();

            SetDummiesInput(false);
        }

        public void HandleGoal(int player)
        {
            if (!IsServer)
            {
                return;
            }

            if (_currentBall == null || player != 1 && player != 2)
            {
                return;
            }

            if (player == 1)
            {
                _scorePlayerTwo += 1;
            }
            else if (player == 2)
            {
                _scorePlayerOne += 1;
            }

            StartCoroutine(ResetMatch(2.0f));
        }

        private void SpawnBall()
        {
            if (_currentBall != null)
            {
                return;
            }

            _currentBall = Instantiate(_ballPrefab, _ballStartPoint.position, Quaternion.identity);
            Spawn(_currentBall);
        }

        private void DespawnBall()
        {
            if (_currentBall == null)
            {
                return;
            }

            Despawn(_currentBall, DespawnType.Destroy);
        }

        private IEnumerator ResetMatch(float delayTime)
        {
            SetDummiesInput(false);

            DespawnBall();

            yield return new WaitForSeconds(delayTime);

            for (int i = 0; i < GameManager.Instance.ConnectedPlayers.Count; i++)
            {
                GameManager.Instance.ConnectedPlayers[i].ControlledDummy.ResetDummyStartPositionAndRotation();
            }

            SpawnBall();

            SetDummiesInput(true);
        }

        private void SetDummiesInput(bool enable)
        {
            for (int i = 0; i < GameManager.Instance.ConnectedPlayers.Count; i++)
            {
                GameManager.Instance.ConnectedPlayers[i].ControlledDummy.SetDummyControl(enable);
            }
        }

        private void PrematchTimer_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (op == SyncTimerOperation.Finished)
            {
                if (asServer)
                {
                    Debug.Log("The prematch timer has completed.");
                    _prematchTimerFinished = true;
                    _matchTimer.StartTimer(_matchTimeLength);

                    ViewManager.Instance.Show<MatchView>();

                    StartMatch();
                }
                else
                {
                    ViewManager.Instance.Show<MatchView>();
                }
            }
        }

        private void MatchTimer_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
        {
            if (op == SyncTimerOperation.Finished)
            {
                if (asServer)
                {
                    Debug.Log("The match timer has completed.");
                    _matchTimerFinished = true;

                    ViewManager.Instance.Get<PostmatchView>().SetFinalScoresText(_scorePlayerOne, _scorePlayerTwo);
                    ViewManager.Instance.Show<PostmatchView>();

                    EndMatch();
                }
                else
                {
                    ViewManager.Instance.Get<PostmatchView>().SetFinalScoresText(_scorePlayerOne, _scorePlayerTwo);
                    ViewManager.Instance.Show<PostmatchView>();
                }
            }
        }

        private void ScorePlayerOne_OnChange(int prev, int next, bool asServer)
        {
            if (!asServer)
            {
                ViewManager.Instance.Get<MatchView>().SetScoresText(next, _scorePlayerTwo);
            }
        }

        private void ScorePlayerTwo_OnChange(int prev, int next, bool asServer)
        {
            if (!asServer)
            {
                ViewManager.Instance.Get<MatchView>().SetScoresText(_scorePlayerOne, next);
            }
        }
    }
}
