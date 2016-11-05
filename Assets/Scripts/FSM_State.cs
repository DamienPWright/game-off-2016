abstract public class FSM_State
{

    protected FiniteStateMachine _fsm;

    public FSM_State(FiniteStateMachine fsm)
    {
        _fsm = fsm;
    }

    abstract public void OnEnter();

    abstract public void Update();

    abstract public void FixedUpdate();

    abstract public void OnExit();
}