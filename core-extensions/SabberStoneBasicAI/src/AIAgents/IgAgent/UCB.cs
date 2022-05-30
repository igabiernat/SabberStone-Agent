using SabberStoneBasicAI.AIAgents.Igagent;
using SabberStoneCore.Tasks.PlayerTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneBasicAI.AIAgents.Igagent
{
	class UCB
	{
		public static Node SelectNodeUCB(Node root)
		{
			float C = 2;
			Dictionary<Node, double> ucbScoresNodes = new Dictionary<Node, double>();
			foreach (Node child in root.childrenNodes)
			{
				double ucbScore = 0;
				if (child.nrOfVisits > 0)
				{
					ucbScore = child.score / child.nrOfVisits + C * Math.Sqrt(Math.Log(root.nrOfVisits)/ child.nrOfVisits);
				}
				else
				{
					ucbScore = Double.MaxValue;
				}
				ucbScoresNodes.Add(child, ucbScore);
			}
			Node selectedNode = ucbScoresNodes.Aggregate((x, y) => x.Value >= y.Value ? x : y).Key;
			return selectedNode;
		}

	}
}
