using UnityEngine;

public class LogisticMachine : PlacedObject {

    [SerializeField] int maxStorage;
    [SerializeField] int currentStorage;




    LogisticDir GetNextDir(LogisticDir dir) {
        switch(dir) {
            default:
            case LogisticDir.Straight: return LogisticDir.Right;
            case LogisticDir.Right: return LogisticDir.Left;
            case LogisticDir.Left: return LogisticDir.Straight;
        }
    }
}