using System.Collections.Generic;
using UnityEngine;

public class Shaft : MonoBehaviour {

    public Transform MiningLocation => miningLocation;
    public Transform DepositLocation => depositLocation;
    public List<ShaftMiner> Miners => _miners;
    public Deposit CurrentDeposit { get; set; }


    [Header("Prefabs")]
    [SerializeField] private ShaftMiner minerPrefab;
    [SerializeField] private Deposit depositPrefab;

    [Header("Locations")]
    [SerializeField] private Transform miningLocation;
    [SerializeField] private Transform depositLocation;
    [SerializeField] private Transform depositInstantiationLocation;

    private GameObject _minersContainer;
    private List<ShaftMiner> _miners;


    private void Start() {
        _minersContainer = new GameObject("Miners");
        _miners = new List<ShaftMiner>();
        CreateMiner();
        CreateDeposit();
    }

    public void CreateMiner() {
        ShaftMiner newMiner = Instantiate(minerPrefab, depositLocation.position, Quaternion.identity);

        if (_miners.Count > 0)
            EqualizeNewMiner(newMiner);

        _miners.Add(newMiner);

        newMiner.transform.SetParent(_minersContainer.transform);
        newMiner.CurrentShaft = this;

        newMiner.Move(miningLocation.position);
    }

    private void CreateDeposit() {
        CurrentDeposit = Instantiate(depositPrefab, depositInstantiationLocation.position, Quaternion.identity);
        CurrentDeposit.transform.SetParent(depositInstantiationLocation);
    }

    private void EqualizeNewMiner(ShaftMiner newMiner) {
        newMiner.CollectCapacity = _miners[0].CollectCapacity;
        newMiner.CollectPerSecond = _miners[0].CollectPerSecond;
        newMiner.MoveSpeed = _miners[0].MoveSpeed;
    }
}
