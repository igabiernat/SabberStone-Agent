using System;
using System.Collections.Generic;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneBasicAI.PartialObservation;
using SabberStoneCore.Enums;
using System.Linq;
using SabberStoneCore.Model.Entities;

namespace SabberStoneBasicAI.AIAgents.Igagent
{
	class Node
	{
		public PlayerTask playerTask { get; set; }
		public Node parentNode { get; set; }
		public List<Node> childrenNodes { get; set; }
		public int nrOfVisits { get; set; }
		public double score { get; set; }
		public Node(PlayerTask playerTask, Node parentNode, List<Node> childrenNodes)
		{
			this.playerTask = playerTask;
			this.parentNode = parentNode;
			this.childrenNodes = childrenNodes;
			nrOfVisits = 0;
			score = 0;
		}
	}

}
