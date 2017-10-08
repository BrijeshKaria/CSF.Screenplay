﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CSF.Screenplay.Abilities;
using CSF.Screenplay.Actors;
using CSF.Screenplay.Reporting.Models;

namespace CSF.Screenplay.Reporting.Builders
{
  /// <summary>
  /// Builder type which creates instances of <see cref="Report"/>.
  /// </summary>
  public class ReportBuilder
  {
    readonly ConcurrentDictionary<Guid, ScenarioBuilder> scenarios;

    /// <summary>
    /// Begins reporting upon a new scenario.
    /// </summary>
    /// <param name="friendlyName">The friendly scenario name.</param>
    /// <param name="featureName">The feature name.</param>
    /// <param name="idName">The uniquely identifying name for the test.</param>
    /// <param name="featureId">The uniquely identifying name for the feature.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void BeginNewScenario(string idName,
                                 string friendlyName,
                                 string featureName,
                                 string featureId,
                                 Guid scenarioId)
    {
      var success = scenarios.TryAdd(scenarioId, new ScenarioBuilder(idName, friendlyName, featureName, featureId));
      if(!success)
        throw new InvalidOperationException("The current scenario already exists in the current builder instance");
    }

    /// <summary>
    /// Reports the end of a scenario.
    /// </summary>
    /// <param name="isSuccess">If set to <c>false</c> then the scenario is marked as a failure.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void EndScenario(bool isSuccess,
                            Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.Finalise(isSuccess);
    }

    /// <summary>
    /// Begins reporting of a new performable.
    /// </summary>
    /// <param name="actor">Actor.</param>
    /// <param name="performable">Performable.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void BeginPerformance(INamed actor, Performables.IPerformable performable,
                                 Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.BeginPerformance(actor, performable);
    }

    /// <summary>
    /// Begins reporting of a performance of a given type.
    /// </summary>
    /// <param name="performanceType">Performance type.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void BeginPerformanceType(PerformanceType performanceType,
                                     Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.BeginPerformanceType(performanceType);
    }

    /// <summary>
    /// Records that the current performable has received a result.
    /// </summary>
    /// <param name="performable">Performable.</param>
    /// <param name="result">Result.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void RecordResult(Performables.IPerformable performable, object result,
                             Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.RecordResult(performable, result);
    }

    /// <summary>
    /// Records that the current performable has failed with an exception.
    /// </summary>
    /// <param name="performable">Performable.</param>
    /// <param name="exception">Exception.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void RecordFailure(Performables.IPerformable performable, Exception exception,
                              Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.RecordFailure(performable, exception);
    }

    /// <summary>
    /// Records that the current performable has completed successfully.
    /// </summary>
    /// <param name="performable">Performable.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void RecordSuccess(Performables.IPerformable performable,
                              Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.RecordSuccess(performable);
    }

    /// <summary>
    /// Ends the performance of the current type.
    /// </summary>
    public void EndPerformanceType(Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.EndPerformanceType();
    }

    /// <summary>
    /// Reports that the given actor has gained the given ability.
    /// </summary>
    /// <param name="actor">Actor.</param>
    /// <param name="ability">Ability.</param>
    /// <param name="scenarioId">The screenplay scenario identity.</param>
    public void GainAbility(INamed actor, IAbility ability,
                            Guid scenarioId)
    {
      var scenario = GetScenario(scenarioId);
      scenario.GainAbility(actor, ability);
    }

    /// <summary>
    /// Builds and returns the report from the current instance.
    /// </summary>
    /// <returns>The report.</returns>
    public Report GetReport()
    {
      return new Report(scenarios.Values.Select(x => x.GetScenario()).ToArray());
    }

    ScenarioBuilder GetScenario(Guid identity)
    {
      ScenarioBuilder scenario;
      if(!scenarios.TryGetValue(identity, out scenario))
        throw new InvalidOperationException("There is no matching scenario in the current builder.");

      return scenario;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReportBuilder"/> class.
    /// </summary>
    public ReportBuilder()
    {
      scenarios = new ConcurrentDictionary<Guid,ScenarioBuilder>();
    }
  }
}
