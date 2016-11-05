public class FiniteStateMachine
{

    public FSM_State currentState;
    public FSM_State previousState;
    public FSM_State nextState;

    public void SetNextState(FSM_State newState)
    {
        nextState = newState;
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    public void FixedUpdate()
    {
        if (currentState != null)
        {
            currentState.FixedUpdate();
        }
    }

    public void ChangeState(FSM_State newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
            previousState = currentState;
        }
        currentState = newState;
        currentState.OnEnter();
    }

    public void SwitchToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }

    public void SwitchToNextState()
    {
        if (nextState != null)
        {
            ChangeState(nextState);
        }
    }
}