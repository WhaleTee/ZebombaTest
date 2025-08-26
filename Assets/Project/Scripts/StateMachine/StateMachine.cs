using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine {
  private StateNode currentState;
  private readonly Dictionary<Type, StateNode> stateNodes = new Dictionary<Type, StateNode>();
  private readonly HashSet<ITransition> anyTransitions = new HashSet<ITransition>();

  public void Update() {
    var transition = GetTransition();
    if (transition != null) ChangeState(transition.To);

    currentState?.State?.Update();
  }

  public void FixedUpdate() => currentState?.State?.FixedUpdate();

  public void SetState(IState state) => ChangeState(GetOrAddNode(state).State);

  public void AddTransition(IState from, IState to, IPredicate condition) => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);

  public void AddAnyTransition(IState to, IPredicate condition) => anyTransitions.Add(new Transition(to, condition));

  private void ChangeState(IState state) {
    if (state == currentState?.State) return;

    var previousState = currentState?.State;
    var stateNode = stateNodes[state.GetType()];
    var nextState = stateNode.State;

    previousState?.OnExit();
    nextState?.OnEnter();

    currentState = stateNode;
  }

  private ITransition GetTransition() {
    foreach (var transition in anyTransitions.Where(transition => transition.Condition.Evaluate())) {
      return transition;
    }

    return currentState?.Transitions?.FirstOrDefault(transition => transition.Condition.Evaluate());
  }

  private StateNode GetOrAddNode(IState state) {
    var presentNode = stateNodes.TryGetValue(state.GetType(), out var node);

    if (!presentNode) {
      node = new StateNode(state);
      stateNodes.Add(state.GetType(), node);
    }

    return node;
  }

  private class StateNode {
    public IState State { get; }
    public HashSet<ITransition> Transitions { get; }

    public StateNode(IState state) {
      State = state;
      Transitions = new HashSet<ITransition>();
    }

    public void AddTransition(IState to, IPredicate condition) => Transitions.Add(new Transition(to, condition));
  }
}