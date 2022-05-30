using SabberStoneBasicAI.PartialObservation;
using SabberStoneCore.Model.Entities;
using SabberStoneCore.Model.Zones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SabberStoneBasicAI.Score
{
	class IgScore : Score
	{
		private static double BATTLECRY = 0.1f;
		private static double CHARGE = 0.1f;
		private static double DEATHRATTLE = 0.1f;
		private static double DIVINE_SHIELD = 0.2f;
		private static double INSPIRE = 0.05f;
		private static double LIFE_STEAL = 0.1f;
		private static double OVERKILL = 0.1f;
		private static double TAUNT = 0.4f;
		private static double WINDFURY = 0.3f;
		private static double STEALTH = 0.05f;
		private static double HERO_HEALTH = 0.7f;
		private static double MINIONS = 0.5f;


		private static double getScorePerPlayer(Controller player)
		{
			Minion[] minions = player.BoardZone.GetAll();
			double minionsScore = getMinionsScore(minions);
			double score = getHeroScore(player, minionsScore);
			return score;
		}

		private static double getMinionsScore(Minion[] minions)
		{
			double score = 0;
			foreach (Minion minion in minions)
			{
				score += minion.AttackDamage + minion.Health;
				if (minion.HasBattleCry)
					score += BATTLECRY;
				if (minion.HasCharge)
					score += CHARGE;
				if (minion.HasDeathrattle)
					score += DEATHRATTLE;
				if (minion.HasDivineShield)
					score += DIVINE_SHIELD;
				if (minion.HasInspire)
					score += INSPIRE;
				if (minion.HasLifeSteal)
					score += LIFE_STEAL;
				if (minion.HasOverkill)
					score += OVERKILL;
				if (minion.HasTaunt)
					score += TAUNT;
				if (minion.HasWindfury)
					score += WINDFURY;
				if (minion.HasStealth)
					score += STEALTH;
			}

			return score;

		}

		private static double getHeroScore(Controller player, double minionsScore)
		{
			double score = player.Hero.Health * HERO_HEALTH + player.Hero.Armor * HERO_HEALTH + minionsScore * MINIONS;

			return score;

		}
		public static double getScore(POGame poGame)
		{
			double playerScore = getScorePerPlayer(poGame.CurrentPlayer);
			double opponentScore = getScorePerPlayer(poGame.CurrentOpponent);

			double finalScore = playerScore - opponentScore;
			double finalScore1 = finalScore / Math.Max(playerScore, opponentScore);
			double finalScore2 = finalScore1 / 2;
			double finalScore3 = finalScore2 + 0.5;

			return finalScore3;
		}
		public static int getScoreRamp(POGame poGame)
		{
			int OpHeroHp = poGame.CurrentOpponent.Hero.Health;
			int HeroHp = poGame.CurrentPlayer.Hero.Health;
			BoardZone opBoardZone = poGame.CurrentOpponent.BoardZone;
			BoardZone boardZone = poGame.CurrentPlayer.BoardZone;
			int MinionTotHealthTaunt = boardZone.Sum(p => p.Health);
			int OpMinionTotHealthTaunt = opBoardZone.Sum(p => p.Health);
			int MinionTotAtk = boardZone.Sum(p => p.AttackDamage);
			if (OpHeroHp < 1)
				return int.MaxValue;

			if (HeroHp < 1)
				return int.MinValue;

			int result = 0;

			if (opBoardZone.Count == 0 && boardZone.Count > 0)
				result += 1000;

			result += (boardZone.Count - opBoardZone.Count) * 50;

			result += (MinionTotHealthTaunt - OpMinionTotHealthTaunt) * 25;

			result += MinionTotAtk;

			result += (HeroHp - OpHeroHp) * 10;

			return result;
		}
	}
}
