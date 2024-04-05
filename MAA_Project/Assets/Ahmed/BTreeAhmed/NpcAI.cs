using System.Collections;
using System.Collections.Generic;
using Ahmed.BTreeAhmed.Nodes;
using UnityEngine;

public class NpcAI : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private TalkToPlayerCoroutineManager _manager;
    [SerializeField] PathFinding path;
    [SerializeField] Grid grid;
    [SerializeField] float detectRange;
    [SerializeField] List<Transform> points = new List<Transform>();
    [SerializeField] List<Vector3> destinations = new List<Vector3>();
    [SerializeField] float moveSpeed;
    public int pointNum = 0;
    private BTNode topNode;
    public bool alreadyIntroduced;

    [SerializeField] Transform player;
    private void Start()
    {
        
        ConstructTree();
    }
    private void Update()
    {
        
        topNode.Evaluate();
    }
    private void ConstructTree()
    {
        //EyesClosed eyesClosed = new EyesClosed(eyes);
        //CheckIfDone checkIfDone = new CheckIfDone();
        Idle idle = new Idle();
        RangeNode range = new RangeNode(detectRange,transform,player.transform,alreadyIntroduced);
        TalkToPlayer talk = new TalkToPlayer(_manager, this,player);
        MoveNpcToPoint moveNpc = new MoveNpcToPoint(points,destinations, pointNum, this.transform, player ,this);
        AlreadyIntroduced checkIntroduced = new AlreadyIntroduced(range);
        CheckFirstDialog checkFirstDialog = new CheckFirstDialog(_manager);
        CheckSecondDialog checkSecondDialog = new CheckSecondDialog(_manager);
        CheckThirdDialog checkThirdDialog = new CheckThirdDialog(_manager, this);
        CheckFourthDialog checkFourthDialog = new CheckFourthDialog(_manager,this);
        CheckFirstPhrase checkFirstPhrase = new CheckFirstPhrase();
        CheckSecondPhrase checkSecondPhrase = new CheckSecondPhrase();
        CheckFirstDestinationPoint checkFirstPoint = new CheckFirstDestinationPoint(this);
        CheckSecondDestinationPoint checkSecondPoint = new CheckSecondDestinationPoint(this);
        
        // Assemble tree
        
        BTInvertor notInRange = new BTInvertor(range);
        BTSequence secondMove = new BTSequence(new List<BTNode>() { checkSecondPoint, moveNpc });
        BTSequence waiteForPlayerToCatchSecondPhrase =
            new BTSequence(new List<BTNode>{checkSecondPhrase, checkFourthDialog, talk});
        BTSequence waiteForPlyarToCatchFirstPhrase =
            new BTSequence(new List<BTNode>() {notInRange, checkFirstPhrase, checkThirdDialog, talk}); //remove not in Range later
        BTSequence secondDialog = new BTSequence(new List<BTNode> { range,checkSecondDialog, talk });
        BTSelector makePlayerCapturePhrase = new BTSelector(new List<BTNode>() {secondDialog,waiteForPlyarToCatchFirstPhrase });
        BTSequence moveToFirstPoint = new BTSequence(new List<BTNode>() { checkFirstPoint, moveNpc });
        BTSequence firstDialog = new BTSequence(new List<BTNode> { checkFirstDialog, talk });
        BTSelector firstMove = new BTSelector(new List<BTNode>{firstDialog,moveToFirstPoint});
        BTSequence firstEncounter = new BTSequence(new List<BTNode> { checkIntroduced, notInRange });
        topNode = new BTSelector(new List<BTNode> {firstEncounter, firstMove ,makePlayerCapturePhrase,secondMove,waiteForPlayerToCatchSecondPhrase});
    }
    private void BuildPathToPointNpc(Transform point)
    {
        if (path != null && grid != null)
        {
            path.FindPath(this.gameObject.transform.position, point.position);
            destinations.Clear();
            for(int i = 0; i < grid.path.Count; i++)
            {
                destinations.Add(grid.path[i].worldPosition);
            }
            
        }
        else
        {
            Debug.LogError("PathFinding object is not assigned.");
        }
    }
    public void MoveNpcAlongPath()
    {
        var dist = Vector3.Distance(this.transform.position, points[pointNum].position);
        BuildPathToPointNpc(points[pointNum]);
        if (destinations.Count > 0)
        {
            Vector3 currentTarget = (destinations[0] - transform.position).normalized;
            rb.velocity = currentTarget * moveSpeed;
            Quaternion lookAtDest = Quaternion.LookRotation(currentTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtDest, 5f * Time.deltaTime);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }
}

