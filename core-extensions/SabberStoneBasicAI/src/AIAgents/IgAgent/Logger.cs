using SabberStoneBasicAI.AIAgents;
using SabberStoneBasicAI.PartialObservation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SabberStoneBasicAI.AIAgents.Igagent
{
	class Logger
	{
		static string path = "C:\\Users\\Iga\\Desktop\\IgAgent\\Results.csv";
		public static void WriteGameStatsToFile(AbstractAgent player1, AbstractAgent player2, GameStats gameStats)
		{
			string gameStatsStr = "Games played;" + gameStats.nr_games + ";\n"
				+ player1.GetType().Name + ";" + gameStats.PlayerA_Wins + ";" + gameStats.time_per_player[0] + ";\n"
				+ player2.GetType().Name + ";" + gameStats.PlayerB_Wins + ";" + gameStats.time_per_player[1] + ";";

			using (StreamWriter sw = File.AppendText(path))
			{
				sw.WriteLine(gameStatsStr);
			}
		}
	}
}
