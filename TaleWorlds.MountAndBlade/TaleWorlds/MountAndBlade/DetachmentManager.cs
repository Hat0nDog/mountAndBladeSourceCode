// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DetachmentManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class DetachmentManager
  {
    private List<IDetachment> _detachments;
    private Dictionary<IDetachment, DetachmentData> data = new Dictionary<IDetachment, DetachmentData>();
    private readonly Team team;

    internal MBReadOnlyList<IDetachment> Detachments { get; private set; }

    private IEnumerable<Formation> formations => this.team.FormationsIncludingSpecial;

    public DetachmentManager(Team team)
    {
      this._detachments = new List<IDetachment>();
      this.Detachments = this._detachments.GetReadOnlyList<IDetachment>();
      this.team = team;
      team.OnFormationsChanged += new Action<Team, Formation>(this.Team_OnFormationsChanged);
    }

    private void Team_OnFormationsChanged(Team arg1, Formation arg2)
    {
      float time = MBCommon.TimeType.Mission.GetTime();
      foreach (IDetachment detachment in arg2.Detachments)
      {
        if (this.data.ContainsKey(detachment))
        {
          DetachmentData detachmentData = this.data[detachment];
          detachmentData.agentScores.Clear();
          detachmentData.firstTime = time;
        }
        else
        {
          if (!this._detachments.Contains(detachment))
            this.MakeDetachment(detachment);
          else
            this.data[detachment] = new DetachmentData();
          DetachmentData detachmentData = this.data[detachment];
          detachmentData.agentScores.Clear();
          detachmentData.firstTime = time;
        }
      }
    }

    public void Clear()
    {
      this.team.OnFormationsChanged -= new Action<Team, Formation>(this.Team_OnFormationsChanged);
      this.team.OnFormationsChanged += new Action<Team, Formation>(this.Team_OnFormationsChanged);
      foreach (IDetachment detachment in this.Detachments.ToList<IDetachment>())
        this.DestroyDetachment(detachment);
    }

    internal void MakeDetachment(IDetachment detachment)
    {
      this._detachments.Add(detachment);
      this.data[detachment] = new DetachmentData();
    }

    internal void DestroyDetachment(IDetachment detachment)
    {
      foreach (Formation formation in this.data[detachment].joinedFormations.ToList<Formation>())
        formation.LeaveDetachment(detachment);
      this._detachments.Remove(detachment);
      this.data.Remove(detachment);
    }

    internal void OnFormationJoinDetachment(Formation formation, IDetachment detachment)
    {
      DetachmentData detachmentData = this.data[detachment];
      detachmentData.joinedFormations.Add(formation);
      detachmentData.firstTime = MBCommon.TimeType.Mission.GetTime();
    }

    internal void OnFormationLeaveDetachment(Formation formation, IDetachment detachment)
    {
      DetachmentData detachmentData = this.data[detachment];
      detachmentData.joinedFormations.Remove(formation);
      detachmentData.agentScores.RemoveAll((Predicate<AgentValuePair<float[]>>) (ags => ags.Agent.Formation == formation));
      detachmentData.firstTime = MBCommon.TimeType.Mission.GetTime();
    }

    internal void TickDetachments()
    {
      // ISSUE: unable to decompile the method.
    }

    internal void TickAgent(Agent agent)
    {
      if (!agent.IsDetachedFromFormation)
      {
        float time = MBCommon.TimeType.Mission.GetTime();
        bool flag = (double) time - (double) agent.LastDetachmentTickAgentTime > 1.5;
        foreach (IDetachment detachment in agent.Formation.Detachments)
        {
          if ((double) detachment.GetDetachmentWeight(agent.Formation.Team.Side) > -3.40282346638529E+38)
          {
            DetachmentData detachmentData = this.data[detachment];
            AgentValuePair<float[]> agentValuePair = (AgentValuePair<float[]>) null;
            for (int index = 0; index < detachmentData.agentScores.Count; ++index)
            {
              if (detachmentData.agentScores[index].Agent == agent)
              {
                agentValuePair = detachmentData.agentScores[index];
                break;
              }
            }
            if (agentValuePair == null)
            {
              if (detachmentData.agentScores.Count == 0)
                detachmentData.firstTime = time;
              float[] templateCostsOfAgent = detachment.GetTemplateCostsOfAgent(agent, (float[]) null);
              detachmentData.agentScores.Add(new AgentValuePair<float[]>(agent, templateCostsOfAgent));
              agent.LastDetachmentTickAgentTime = time;
            }
            else if (flag)
            {
              agentValuePair.Value = detachment.GetTemplateCostsOfAgent(agent, agentValuePair.Value);
              agent.LastDetachmentTickAgentTime = time;
            }
          }
          else
            this.data[detachment].firstTime = time;
        }
      }
      else
      {
        if (agent.Detachment == null || agent.Detachment.IsAgentEligible(agent))
          return;
        agent.Detachment.RemoveAgent(agent);
        agent.Formation?.AttachUnit(agent);
      }
    }

    internal void OnAgentRemoved(Agent agent)
    {
      foreach (IDetachment detachment in agent.Formation.Detachments)
        this.data[detachment].agentScores.RemoveAll((Predicate<AgentValuePair<float[]>>) (ags => ags.Agent == agent));
    }

    [Conditional("DEBUG")]
    private void AssertDetachments()
    {
      foreach (IDetachment detachment1 in this._detachments)
      {
        IDetachment detachment = detachment1;
        foreach (Agent agent1 in detachment.Agents)
        {
          Agent agent = agent1;
          this._detachments.Count<IDetachment>((Func<IDetachment, bool>) (d => d.Agents.Contains<Agent>(agent)));
        }
        IEnumerable<Agent> source = this.data[detachment].agentScores.Select<AgentValuePair<float[]>, Agent>((Func<AgentValuePair<float[]>, Agent>) (asc => asc.Agent));
        source.Count<Agent>((Func<Agent, bool>) (a => a.Team != this.team));
        source.FirstOrDefault<Agent>((Func<Agent, bool>) (a => a.Team != this.team));
        source.FirstOrDefault<Agent>((Func<Agent, bool>) (a => a.Formation != null && !a.Formation.Detachments.Contains(detachment)));
        source.Count<Agent>((Func<Agent, bool>) (a => a.Formation != null && !a.Formation.Detachments.Contains(detachment)));
      }
    }

    [Conditional("DEBUG")]
    internal void AssertDetachment(Team team, IDetachment detachment)
    {
      this.data[detachment].joinedFormations.Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnits > 0));
      team.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f.Detachments.Contains(detachment)));
    }
  }
}
