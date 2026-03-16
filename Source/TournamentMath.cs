using System;

namespace Bracketonomy
{
	// TODO: Allow for the saving and loading of weights?
	public static class Weights
	{
		// When calculating a score, how much should each be weighted?
		public static float PpgWeight = 0.6f;
		public static float CalculatedWeight = 0.4f;

		// For the BPI factor, how much should each be weighted?
		public static float SeedWeight = 0.1f;
		public static float BpiOffWeight = 0.4f;
		public static float BpiDefWeight = 0.6f;

		// For the record factor, how much should each be weighted?
		public static float RecordWeight = 0.7f;
		public static float ConferenceWeight = 0.0f;
		public static float VsTop25Weight = 0.1f;
		public static float Last10Weight = 0.2f;
		public static float SosRankWeight = 0.0f;
		public static float SorRankWeight = 0.0f;

		// How much should the BPI and record factor into a team's performance?
		public static float BpiFactorWeight = 0.7f;
		public static float RecordFactorWeight = 0.3f;
		public static float OverallFactorWeight = 0.1f;

		// How many attempts of each type per game should be considered?
		// NOTE: NCAA standard has been 50% of all points from field goals, 30% three-pointers,
		// and 20% free throws. These values give roughly that approximation.
		public static int FieldGoalAttempts = 35;
		public static int ThreePointAttempts = 22;
		public static int FreeThrowAttempts = 20;

		// These values are calculated after all team data is loaded.
		public static float MinBpiOff = 0.0f;
		public static float MaxBpiOff = 0.0f;
		public static float MinBpiDef = 0.0f;
		public static float MaxBpiDef = 0.0f;
	}

	public class TournamentMath
	{
		public static (int team1Score, int team2Score) SimulateGame(Team team1, Team team2)
		{
			// Calculate various "factors" to predict a team's performance. A team's BPI factor is
			// a combination of its conference seed and its component BPI offensive and defensive
			// score. A team's record factor is a combination of its total win/loss record, its
			// conference record, its record against top-25 teams, its record from its last ten
			// games, its Strength of Schedule rank, and its Strength of Record rank. These are
			// combined into a +/- effect on their performance when calculating a predicted game
			// score below.
			float team1BpiFactor = CalculateBpiFactor(team1);
			float team2BpiFactor = CalculateBpiFactor(team2);
			float team1RecordFactor = CalculateRecordFactor(team1);
			float team2RecordFactor = CalculateRecordFactor(team2);
			float team1BlendedFactor =
				team1BpiFactor * Weights.BpiFactorWeight +
				team1RecordFactor * Weights.RecordFactorWeight;
			float team2BlendedFactor =
				team2BpiFactor * Weights.BpiFactorWeight +
				team2RecordFactor * Weights.RecordFactorWeight;

			// Calculate roughly how different in ability the teams are based on these factors to
			// influence the difference in score when they play. This will range around 1.0f +/-
			// the overall factor weight.
			float team1WeightedFactor =
				1.0f + (team1BlendedFactor - team2BlendedFactor) / 100.0f * Weights.OverallFactorWeight;
			float team2WeightedFactor =
				1.0f + (team2BlendedFactor - team1BlendedFactor) / 100.0f * Weights.OverallFactorWeight;

			// Calculate the final score of the game. This is a combination of historic points-
			// per-game combined with opposing points-per-game and a calculated score based on
			// shots attempted and performance-weighted historic percentages. The team with the
			// highest score is, of course, the winner for the simulation.
			(int team1Score, int team2Score) = CalculateScore(team1, team2, team1WeightedFactor, team2WeightedFactor);

			// Ties aren't allowed, so prefer the team with the higher weighted factor.
			if (team1Score == team2Score)
			{
				if (team1WeightedFactor > team2WeightedFactor)
				{
					team1Score++;
				}
				else
				{
					team2Score++;
				}
			}

			return (team1Score, team2Score);
		}

		// Scale a value to between [0.0f ... 100.0f] based on a relative min and max.
		private static float GetLinearResult(float value, float min, float max)
		{
			float step = 100.0f / (max - min);
			float clampedValue = Math.Clamp(value, min, max);
			float result = step * (clampedValue - min);
			return result;
		}

		private static float CalculateBpiFactor(Team team)
		{
			// Compare seed, BPI offensive, and BPI defensive scores based on relative ranges.
			float seedFactor = GetLinearResult(17 - team.Seed, 1.0f, 16.0f);
			float bpiOffFactor = GetLinearResult(team.BpiOff, Weights.MinBpiOff, Weights.MaxBpiOff);
			float bpiDefFactor = GetLinearResult(team.BpiDef, Weights.MinBpiDef, Weights.MaxBpiDef);
			float bpiFactor =
				seedFactor * Weights.SeedWeight +
				bpiOffFactor * Weights.BpiOffWeight +
				bpiDefFactor * Weights.BpiDefWeight;
			return bpiFactor;
		}

		private static float CalculateRecordFactor(Team team)
		{
			// A record of total wins minus losses of 28 or higher is 100%, 8 or lower is 0%.
			float recordWLFactor = GetLinearResult(team.RecordW - team.RecordL, 8.0f, 28.0f);
			// A record of conference wins minus losses of 17 or higher is 100%, 0 or lower is 0%
			float conferenceWLFactor = GetLinearResult(team.ConferenceW - team.ConferenceL, 0.0f, 17.0f);
			// A record against top-25 teams of 3 or higher is 100%, -3 or lower is 0%
			float vsTop25WLFactor = GetLinearResult(team.VsTop25W - team.VsTop25L, -3.0f, 3.0f);
			// A record of the last 10 games of 10 is 100%, 0 is 0%.
			float last10WLFactor = GetLinearResult(team.Last10W - team.Last10L, 0.0f, 10.0f);
			// An SOS rank of 1 is 100%, 250 or higher is 0%.
			float sosRankFactor = GetLinearResult(251 - team.SosRank, 1.0f, 250.0f);
			// An SOR rank of 1 is 100%, 150 or higher is 0%.
			float sorRankFactor = GetLinearResult(151 - team.SorRank, 1.0f, 150.0f);
			float recordFactor =
				recordWLFactor * Weights.RecordWeight +
				conferenceWLFactor * Weights.ConferenceWeight +
				vsTop25WLFactor * Weights.VsTop25Weight +
				last10WLFactor * Weights.Last10Weight +
				sosRankFactor * Weights.SosRankWeight +
				sorRankFactor * Weights.SorRankWeight;
			return recordFactor;
		}

		private static (int, int) CalculateScore(Team team1, Team team2, float team1WeightedFactor, float team2WeightedFactor)
		{
			// We'll use two methods to predict the score of a game. First, a simple average
			// between one team's points-per-game and the other team's opposing points-per-game.
			float team1PpgScore = (team1.Ppg + team2.OppPgg) / 2.0f;
			float team2PpgScore = (team2.Ppg + team1.OppPgg) / 2.0f;

			// Second, we'll calculate each team's score based on predicted successful field goal,
			// three-point, and free throw attempts.
			float team1FieldGoalPoints = Weights.FieldGoalAttempts * (team1.FieldGoalPer / 100.0f) * 2.0f;
			float team1ThreePointPoints = Weights.ThreePointAttempts * (team1.ThreePointPer / 100.0f) * 3.0f;
			float team1FreeThrowPoints = Weights.FreeThrowAttempts * (team1.FreeThrowPer / 100.0f);
			float team1CalculatedPoints = team1FieldGoalPoints + team1ThreePointPoints + team1FreeThrowPoints;
			float team2FieldGoalPoints = Weights.FieldGoalAttempts * (team2.FieldGoalPer / 100.0f) * 2.0f;
			float team2ThreePointPoints = Weights.ThreePointAttempts * (team2.ThreePointPer / 100.0f) * 3.0f;
			float team2FreeThrowPoints = Weights.FreeThrowAttempts * (team2.FreeThrowPer / 100.0f);
			float team2CalculatedPoints = team2FieldGoalPoints + team2ThreePointPoints + team2FreeThrowPoints;

			// Then we'll average the two based on their respective weights.
			float team1BlendedScore =
				team1PpgScore * Weights.PpgWeight +
				team1CalculatedPoints * Weights.CalculatedWeight;
			float team2BlendedScore =
				team2PpgScore * Weights.PpgWeight +
				team2CalculatedPoints * Weights.CalculatedWeight;

			// Lastly, we'll scale the scores based on the relative strength of the teams.
			int team1Score = (int)Math.Round(team1BlendedScore * team1WeightedFactor);
			int team2Score = (int)Math.Round(team2BlendedScore * team2WeightedFactor);

			return (team1Score, team2Score);
		}
	}
}
