﻿using System;
using System.Collections.Generic;
using System.Linq;
using CSF.Screenplay.Abilities;
using CSF.Screenplay.Actions;

namespace CSF.Screenplay.Actors
{
  public class Performer : IPerformer
  {
    #region fields

    readonly IEnumerable<IAbility> abilities;

    #endregion

    #region public API

    public void Perform<TParams>(IAction<TParams> action, TParams parameters)
    {
      if(ReferenceEquals(action, null))
        throw new ArgumentNullException(nameof(action));

      action.Execute(parameters);
    }

    public object Perform<TParams>(IActionWithResult<TParams> action, TParams parameters)
    {
      if(ReferenceEquals(action, null))
        throw new ArgumentNullException(nameof(action));

      return action.Execute(parameters);
    }

    public TResult Perform<TParams,TResult>(IAction<TParams,TResult> action, TParams parameters)
    {
      if(ReferenceEquals(action, null))
        throw new ArgumentNullException(nameof(action));

      return action.Execute(parameters);
    }

    #endregion

    #region ICanPerformActions implementation

    TAction IPerformer.GetAction<TAction>()
    {
      return GetAction<TAction>();
    }

    bool IPerformer.SupportsActionType<TAction>()
    {
      return SupportsActionType<TAction>();
    }

    protected virtual IAbility GetAbility<TAction>()
    {
      return abilities.FirstOrDefault(x => x.CanProvideAction<TAction>());
    }

    protected virtual bool SupportsActionType<TAction>()
    {
      return GetAbility<TAction>() != null;
    }

    protected virtual TAction GetAction<TAction>() where TAction : class
    {
      var ability = GetAbility<TAction>();
      if(ability == null)
        return null;

      return ability.GetAction<TAction>();
    }

    protected virtual IEnumerable<Type> GetAllActionTypes()
    {
      return abilities
        .SelectMany(x => x.GetActionTypes())
        .Distinct()
        .ToArray();
    }

    #endregion

    #region constructor

    internal Performer(IEnumerable<IAbility> abilities)
    {
      if(abilities == null)
        throw new ArgumentNullException(nameof(abilities));

      this.abilities = abilities;
    }

    #endregion
  }
}
