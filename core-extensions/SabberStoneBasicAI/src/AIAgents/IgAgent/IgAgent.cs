using System;
using System.Collections.Generic;
using SabberStoneCore.Tasks.PlayerTasks;
using SabberStoneBasicAI.PartialObservation;
using SabberStoneCore.Enums;
using System.Linq;
using SabberStoneCore.Model.Entities;
using System.Diagnostics;
using SabberStoneBasicAI.Score;

// TODO choose your own namespace by setting up <submission_tag>
// each added file needs to use this namespace or a subnamespace of it
namespace SabberStoneBasicAI.AIAgents.Igagent
{
	class IgAgent : AbstractAgent
	{
		private Random Rnd;
		public override void InitializeAgent()
		{
			Rnd = new Random();
		}

		public override void FinalizeAgent()
		{
		}

		public override void FinalizeGame()
		{
		}

		public override void InitializeGame()
		{
		}

		public Func<List<IPlayable>, List<int>> RandomMulliganRule()
		{
			return p => p.Where(t => Rnd.Next(1, 3) > 1).Select(t => t.Id).ToList();
		}

		public override PlayerTask GetMove(POGame poGame)
		{

			if (poGame.CurrentPlayer.Options().Count == 1)
			{
				return poGame.CurrentPlayer.Options()[0];
			}

			POGame initialState = poGame.getCopy();
			Node rootNode = new Node(null, null, new List<Node>());

			InitializeRoot(rootNode, initialState);

			Node afterSelect;
			Node afterExpand;

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			//while (rootNode.nrOfVisits < 10) //Debug
			while (stopwatch.ElapsedMilliseconds <= 100) //Main loop of MCTS
			{
				poGame = initialState;
				afterSelect = Select(rootNode, ref poGame);
				afterExpand = Expand(afterSelect, ref poGame);
				for (int i = 0; i < 10; i++)
				{
					double reward = RandomSimulate(afterExpand, poGame);
					Backpropagate(afterExpand, reward);
					rootNode.nrOfVisits++;
				}
				
			}
			stopwatch.Stop();

			PlayerTask bestMove = null;
			double bestScore = 0;

			//Get node with largest score
			foreach (Node child in rootNode.childrenNodes)
			{
				double score = child.score + child.nrOfVisits;
				if (score > bestScore)
				{
					bestScore = score;
					bestMove = child.playerTask;
				}
			}
			return bestMove;
		}

		//Add possible actions as children to root
		private void InitializeRoot(Node root, POGame poGame)
		{
			foreach (PlayerTask task in poGame.CurrentPlayer.Options())
			{
				root.childrenNodes.Add(new Node(task, root, new List<Node>()));
			}
		}


		public Node Select(Node rootNode, ref POGame poGame)
		{
			Node selectedNode = UCB.SelectNodeUCB(rootNode); //Upper Confidence Bound Algorithm for choosing node 
			//Node selectedNode = rootNode.childrenNodes[Rnd.Next(0, rootNode.childrenNodes.Count)];

			if (selectedNode.playerTask.PlayerTaskType != PlayerTaskType.END_TURN)
				poGame = poGame.Simulate(new List<PlayerTask> { selectedNode.playerTask })[selectedNode.playerTask]; //Simulate game with selected move

			return selectedNode;
		}

		public Node Expand(Node expandingNode, ref POGame poGame)
		{
			Node randomSimulationNode;
			POGame poGameCopy = poGame.getCopy();
			List<PlayerTask> actions = poGame.CurrentPlayer.Options(); //Get all possible moves from current state
																			 
			if (expandingNode.nrOfVisits == 0 || expandingNode.playerTask.PlayerTaskType == PlayerTaskType.END_TURN) 
				randomSimulationNode = expandingNode;
			else
			{
				foreach (PlayerTask action in actions)
				{
					expandingNode.childrenNodes.Add(new Node(action, expandingNode, new List<Node>())); //Adding possible moves - children nodes - to a chosen node
				}
				randomSimulationNode = expandingNode.childrenNodes[Rnd.Next(0, expandingNode.childrenNodes.Count)]; //Get random action
				
				if (expandingNode.playerTask.PlayerTaskType != PlayerTaskType.END_TURN)
					poGame = poGame.Simulate(new List<PlayerTask> { randomSimulationNode.playerTask })[randomSimulationNode.playerTask];//Simulate game with chosen move

			}

			return randomSimulationNode;
		}
		public double RandomSimulate(Node randomSimulationNode, POGame poGame)
		{
			float result = -1f;

			PlayerTask task;

			if (poGame == null)
				return 0.5f;

			if (randomSimulationNode.playerTask.PlayerTaskType == PlayerTaskType.END_TURN)
				return IgScore.getScore(poGame);

			while (poGame.getGame().State != SabberStoneCore.Enums.State.COMPLETE)
			{
				List<PlayerTask> options = poGame.CurrentPlayer.Options();
				//int optionIndex = options.Count()-1;
				int optionIndex = Rnd.Next(0,options.Count());
				task = options[optionIndex];
				if (task.PlayerTaskType != PlayerTaskType.END_TURN)
					poGame = poGame.Simulate(new List<PlayerTask> { task })[task];

				if (poGame == null)
					return 0.5f;

				if (task.PlayerTaskType == PlayerTaskType.END_TURN)
					return IgScore.getScore(poGame);
			}
			if (poGame.CurrentPlayer.PlayState == SabberStoneCore.Enums.PlayState.CONCEDED || poGame.CurrentPlayer.PlayState == SabberStoneCore.Enums.PlayState.LOST)
			{
				result = 0f;
			}
			if (poGame.CurrentPlayer.PlayState == SabberStoneCore.Enums.PlayState.WON)
			{
				result = 1f;
			}

			return result;
		}

		public void Backpropagate(Node expandedNode, double result)
		{
			expandedNode.nrOfVisits++;
			expandedNode.score += result;
			if (expandedNode.parentNode != null && expandedNode.playerTask != null)
				Backpropagate(expandedNode.parentNode, result);
		}

	}
}
