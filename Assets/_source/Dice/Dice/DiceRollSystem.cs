using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiceRollSystem : SystemBase<BB_Dummy>
{
    public const int LAUNCHER_SLOTS = 9;

    BB_GameState gameState;
    BB_Debug debug;

    TablePhysicsRoot _tablePhysicsRoot;
    Transform[] _launcherMounts = new Transform[LAUNCHER_SLOTS];
    List<BaseDie> _activeDice = new List<BaseDie>();
    List<Rigidbody> _activeDiceRB = new List<Rigidbody>();
    Simulation activeSimulation;

    Scene _lastSimulationScene;

    protected override void Awake()
    {
        base.Awake();
        gameState = GetBlackboard(DiceBlackboards.GameState) as BB_GameState;
        debug = GetBlackboard(DiceBlackboards.Debug) as BB_Debug;

        debug.debugLaunchDiceRequest.AddListener(() =>
        {
            DebugSimulate();
        });

        _tablePhysicsRoot = FindFirstObjectByType<TablePhysicsRoot>();

        // Store impulse array transforms
        var impulseArray = _tablePhysicsRoot.impulseArrayRoot.transform;
        for (int i = 0; i < impulseArray.childCount; i++)
            _launcherMounts[i] = impulseArray.GetChild(i);

        // Prepare simulation scene
        _lastSimulationScene = SceneManager.CreateScene("simulationScene", new CreateSceneParameters() { localPhysicsMode = LocalPhysicsMode.Physics3D });

        // Copy the collision box into sim scene
        var tableCollisionPosition = _tablePhysicsRoot.tableCollision.transform.position;
        var tableCollisionClone = GameObject.Instantiate(_tablePhysicsRoot.tableCollision);
        tableCollisionClone.transform.parent = null;
        SceneManager.MoveGameObjectToScene(tableCollisionClone, _lastSimulationScene);
        tableCollisionClone.transform.position = tableCollisionPosition;
    }

    private void DebugSimulate()
    {
        List<BaseDie> debugDies = new List<BaseDie>();
        List<int> desiredOutcomes = new List<int>();
        for (int i = 0; i < debug.debugPopulatePlayerSelection.Value.Count; i++)
        {
            var launchSettings = debug.debugPopulatePlayerSelection.Value[i];
            debugDies.Add(launchSettings.die);
            desiredOutcomes.Add(launchSettings.desiredOutcome);
        }
        var layout = GenerateLauncherLayout(debugDies);
        var sim = Simulate(layout);
        OverrideValues(sim, desiredOutcomes);
        PlaySimulation(sim);
    }

    public class LauncherLayout
    {
        public Die[] slots = new Die[9];
    }

    public class Simulation
    {
        public float totalSimTime;
        public float currentTime;

        public float totalSimSteps;
        public int currentFixedStep;

        public float simTimeStep;

        public class DieData
        {
            public int? valueOverride;
            public Quaternion valueOverrideOffsetRotation;
            public int valueAtLastStep;
            public List<StepData> steps = new List<StepData>();
        }

        public struct StepData
        {
            public float time;
            public int step;
            public Vector3 position;
            public Quaternion rotation;
        }

        public Dictionary<BaseDie, DieData> diceData;

        public Simulation()
        {
            diceData = new Dictionary<BaseDie, DieData>();
        }

        public void Add(BaseDie die, StepData step)
        {
            if (!diceData.TryGetValue(die, out DieData data))
                diceData[die] = data = new DieData();
            data.steps.Add(step);
        }
    }

    private List<BaseDie> _dieBuffer = new List<BaseDie>();

    public void PlaySimulation(Simulation sim)
    {
        if (activeSimulation != null)
            activeSimulation = null;

        activeSimulation = sim;
        sim.currentTime = 0;
        sim.currentFixedStep = 0;

        string values = "Dice Values: ";
        foreach (var pair in sim.diceData)
            values += pair.Value.valueAtLastStep + " ,";
        Debug.Log(values);
    }

    protected override void FixedUpdate()
    {
        base.Update();
        // Assuming simulation was at the same fixed delta as the rest of the physics for now
        if (activeSimulation != null)
        {
            if (activeSimulation.currentFixedStep >= activeSimulation.totalSimSteps)
                activeSimulation.currentFixedStep = 0;

            foreach (var pair in activeSimulation.diceData)
            {
                var die = pair.Key;
                var data = pair.Value;
                var step = data.steps[activeSimulation.currentFixedStep];
                die.transform.position = step.position;

                if(data.valueOverride.HasValue && debug.useDesiredOutcome.Value)
                {
                    die.transform.rotation =  step.rotation * data.valueOverrideOffsetRotation;
                }
                else
                    die.transform.rotation = step.rotation;
            }

            activeSimulation.currentFixedStep++;
        }
    }

    public void OverrideValues(Simulation sim, List<int> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (values[i] == 0)
                continue; // invalid state

            int desiredValue = values[i];

            foreach (var pair in sim.diceData)
            {
                var basedie = pair.Key;
                var data = pair.Value;

                if (data.valueOverride.HasValue)
                    continue;

                if (data.valueAtLastStep == desiredValue)
                {
                    data.valueOverrideOffsetRotation = Quaternion.identity;
                    data.valueOverride = desiredValue; // mark as overriden
                    break;
                }

                if (basedie is Die die)
                {
                    if (desiredValue != data.valueAtLastStep && desiredValue <= (int)die.Sides)
                    {
                        data.valueOverrideOffsetRotation = ComputeRotationSideToSide(die, data.valueAtLastStep, desiredValue);
                        data.valueOverride = desiredValue;
                        break;
                    }
                }
            }
        }
    }

    public Quaternion ComputeRotationSideToSide(Die die, int from, int to)
    {
        die.transform.rotation = Quaternion.identity;
        var t = die.SideComponents[to - 1].transform.forward;
        var f = die.SideComponents[from - 1].transform.forward;
        return Quaternion.FromToRotation(t, f);
    }

    public Simulation Simulate(LauncherLayout layout)
    {
        for (int i = 0; i < _activeDice.Count; i++)
        {
            var die =_activeDice[i];
            if (die)
                Destroy(_activeDice[i].gameObject);
        }
        _activeDice.Clear();

        for (int i = 0; i < _launcherMounts.Length; i++)
        {
            var mount = _launcherMounts[i];
            var die = layout.slots[i];
            if (!die)
                continue;

            var physical = GameObject.Instantiate(die.gameObject).GetComponent<Die>();
            SceneManager.MoveGameObjectToScene(physical.gameObject, _lastSimulationScene);
            var rigidbody = physical.GetComponent<Rigidbody>();
            rigidbody.Move(mount.position, mount.rotation);
            _activeDice.Add(physical);
            _activeDiceRB.Add(rigidbody);
            GenerateLaunchImpulseOnDie(physical, mount);
        }

        Simulation sim = new Simulation();

        bool allAsleep()
        {
            bool asleep = true;

            for (int i = 0; i < _activeDice.Count; i++)
            {
                var rb = _activeDiceRB[i];
                asleep &= rb.IsSleeping();
            }
            return asleep;
        }

        float maxSimTime = 10f;
        float simTime = 0;
        int simSteps = 0;
        float fixedStep = Time.fixedDeltaTime;

        PhysicsScene simulationPhysicsScene = _lastSimulationScene.GetPhysicsScene();

        while (!allAsleep() && simTime < maxSimTime)
        {
            simulationPhysicsScene.Simulate(fixedStep);
            for (int i = 0; i < _activeDiceRB.Count; i++)
            {
                var rb = _activeDiceRB[i];
                var die = _activeDice[i];
                sim.Add(die, new Simulation.StepData()
                {
                    position = rb.position,
                    rotation = rb.rotation,
                    step = simSteps,
                    time = simTime
                });
            }
            simSteps++;
            simTime += fixedStep;
        }

        // capture the side the dice is one at the last step
        for (int i = 0; i < _activeDice.Count; i++)
        {
            var d = _activeDice[i] as Die;
            float maxDot = -2;
            DieSideComponent maxDotSide = null;
            for (int s = 0; s < d.SideComponents.Length; s++)
            {
                var c = d.SideComponents[s];
                float dot = Vector3.Dot(Vector3.up, c.transform.forward);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    maxDotSide = c;
                }
            }
            sim.diceData[d].valueAtLastStep = maxDotSide.SideValue;
        }

        // simulation complete, destroy all rigidbodies
        for (int i = 0; i < _activeDice.Count; i++)
        {
            for (int c = 0; c < _activeDice[i].allColliders.Count; c++)
            {
                Destroy(_activeDice[i].allColliders[c]);
            }
            Destroy(_activeDiceRB[i]);
        }
        _activeDiceRB.Clear();

        sim.totalSimTime = simTime;
        sim.totalSimSteps = simSteps;
        sim.simTimeStep = fixedStep;
        return sim;
    }

    public LauncherLayout GenerateLauncherLayout(List<BaseDie> list)
    {
        LauncherLayout layout = new LauncherLayout();

        _dieBuffer.Clear();
        _dieBuffer.AddRange(list);
        if (_dieBuffer.Count > LAUNCHER_SLOTS)
            Debug.LogError("DiceRollSystem: Too many dice provided, rework impulse array at this point to support more dice.");

        _dieBuffer.Shuffle();

        int diceLaunched = 0;
        while (_dieBuffer.Count > 0 && diceLaunched < LAUNCHER_SLOTS)
        {
            var die = _dieBuffer[_dieBuffer.Count - 1];
            _dieBuffer.RemoveAt(_dieBuffer.Count - 1);

            for (int t = 0; t < layout.slots.Length; t++)
            {
                if (layout.slots[t])
                    continue;
                if (Random.Range(0, 2) > 0)
                {
                    layout.slots[t] = die as Die;
                    diceLaunched++;
                    break;
                }
            }
        }

        return layout;
    }

    public void GenerateLaunchImpulseOnDie(Die die, Transform mount)
    {
        Rigidbody rb = die.GetComponent<Rigidbody>();
        var impulseSettings = die.impulseSettings;
        Vector3 finalForce = default;
        float forceDeviation = Random.Range(0, impulseSettings.forceRandomDeviation);
        if (impulseSettings.randomize)
        {
            Vector2 directionDeviation = Random.insideUnitCircle * impulseSettings.randomDeviation;
            finalForce = (Vector3.forward + new Vector3(directionDeviation.x, directionDeviation.y, 0)) * (impulseSettings.force + forceDeviation);
        }
        else
        {
            finalForce = Vector3.forward * impulseSettings.force * forceDeviation;
        }

        finalForce = mount.TransformDirection(finalForce);
        rb.AddForce(finalForce, impulseSettings.forceMode);

        float finalAngularForce = impulseSettings.angularForce + Random.Range(0, impulseSettings.angularForceRandomDeviation);
        rb.AddRelativeTorque(Random.insideUnitSphere.normalized * finalAngularForce, impulseSettings.forceMode);
    }
}
