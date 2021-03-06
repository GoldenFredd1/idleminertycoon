using System.Collections;
using UnityEngine;

public class ElevatorMiner : BaseMiner {
    [SerializeField] private Elevator elevator;

    private int _currentShaftIndex = -1;
    private Deposit _currentDeposit;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.N)) {
            MoveToNextLocation();
        }
    }

    public void MoveToNextLocation() {
        _currentShaftIndex++;

        Shaft currentShaft = ShaftManager.Instance.Shafts[_currentShaftIndex];
        Vector2 nextPosition = new Vector2(transform.position.x, currentShaft.DepositLocation.position.y);

        _currentDeposit = currentShaft.CurrentDeposit;

        Move(nextPosition);
    }

    protected override void CollectGold() {
        if (!_currentDeposit.CanCollectGold() && _currentShaftIndex == ShaftManager.Instance.Shafts.Count - 1) {
            _currentShaftIndex = -1;
            ChangeGoal();
            Vector3 elevatorDepositPosition = new Vector3(transform.position.x, elevator.DepositLocation.position.y, transform.position.z);
            Move(elevatorDepositPosition);
            return;
        }

        int amountToCollect = _currentDeposit.CollectGold(this);
        float collectTime = amountToCollect / CollectPerSecond;

        OnLoading?.Invoke(this, collectTime);

        StartCoroutine(IECollect(amountToCollect, collectTime));
    }

    protected override void DepositGold() {
        if (CurrentGold <= 0) {
            _currentShaftIndex = -1;
            ChangeGoal();
            MoveToNextLocation();

            return;
        }

        float depositTime = CurrentGold / CollectPerSecond;

        OnLoading?.Invoke(this, depositTime);

        StartCoroutine(IEDeposit(CurrentGold, depositTime));
    }

    protected override IEnumerator IECollect(int collectGold, float collectTime) {
        yield return new WaitForSeconds(collectTime);

        if (CurrentGold > 0 && CurrentGold < CollectCapacity) {
            CurrentGold += collectGold;
        } else {
            CurrentGold = collectGold;
        }

        _currentDeposit.RemoveGold(collectGold);

        yield return new WaitForSeconds(0.5f);

        if (CurrentGold == CollectCapacity || _currentShaftIndex == ShaftManager.Instance.Shafts.Count - 1) {
            _currentShaftIndex = -1;
            ChangeGoal();

            Vector3 elevatorDepositPosition = new Vector3(transform.position.x, elevator.DepositLocation.position.y, transform.position.z);
            Move(elevatorDepositPosition);
        } else {
            MoveToNextLocation();
        }
    }

    protected override IEnumerator IEDeposit(int collectedGold, float depositTime) {
        yield return new WaitForSeconds(depositTime);

        elevator.ElevatorDeposit.DepositGold(CurrentGold);
        CurrentGold = 0;
        _currentShaftIndex = -1;

        ChangeGoal();
        MoveToNextLocation();
    }
}
