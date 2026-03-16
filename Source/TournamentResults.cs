using System.Collections.Generic;
using System.ComponentModel;

namespace Bracketonomy
{
	// A Result is one 
	public struct Result
	{
		public int Seed;
		public string Team;
		public int Score;
		public bool Winner;
	}

	// The results of the tournament and how to simulate it.
	public class TournamentResults : INotifyPropertyChanged
	{
		// ResultControls will bind into this dictionary, one for each seed and winner, etc. See
		// all possible field names at the bottom.
		public Dictionary<string, Result> Results = new Dictionary<string, Result>();

		// This is invoked whenever the results are changed.
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public void Populate(TeamData data)
		{
			Results.Clear();

			// Seed the initial team names.
			for (int i = 0; i < 64; ++i)
			{
				string fieldName = FieldNames[i];
				Result result = new Result
				{
					Seed = data.Teams[i].Seed,
					Team = data.Teams[i].Name,
					Score = 0,
					Winner = false
				};
				Results.Add(fieldName, result);
			}

			// Tell the UI our results have changed.
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		// Based on a given field, look up what the winner's field should be. For example,
		// WestSeed1 would return WestWinner1, WestWinner1 would return WestSweetSixteen1, etc.
		private string LookUpWinner(string fieldName1)
		{
			string toReturn = "Unknown";
			for (int i = 0; i < FieldNames.Length; ++i)
			{
				if (FieldNames[i] == fieldName1)
				{
					int winnerIndex = 0;
					// All 64 teams play in the first round.
					if (i < 64)
					{
						winnerIndex = i / 2 + 64;
					}
					// Then 32 teams play in the second round.
					else if (i < 96)
					{
						winnerIndex = (i - 64) / 2 + 96;
					}
					// Then the Sweet Sixteen.
					else if (i < 112)
					{
						winnerIndex = (i - 96) / 2 + 112;
					}
					// Then the Elite Eight.
					else if (i < 120)
					{
						winnerIndex = (i - 112) / 2 + 120;
					}
					// Then the Final Four.
					else if (i < 124)
					{
						winnerIndex = (i - 120) / 2 + 124;
					}
					// Then the championship game.
					else if (i < 126)
					{
						winnerIndex = 126;
					}
					toReturn = FieldNames[winnerIndex];
					break;
				}
			}
			return toReturn;
		}

		// Run all the simulations up to the championship.
		public void SimulateTournament(TeamData data)
		{
			// For example, compare WestSeed1 and WestSeed16 and put the results in WestWinner1.
			for (int i = 0; i < 126; i += 2)
			{
				string team1FieldName = FieldNames[i];
				string team2FieldName = FieldNames[i + 1];
				string winnerFieldName = LookUpWinner(team1FieldName);
				Result team1Result = Results[team1FieldName];
				Result team2Result = Results[team2FieldName];
				Result winnerResult = SimulateGame(ref team1Result, ref team2Result, data);
				// NOTE: Scores are calculated and kept in the "previous" results.
				Results[team1FieldName] = team1Result;
				Results[team2FieldName] = team2Result;
				Results[winnerFieldName] = winnerResult;
			}

			// Tell the UI our results have changed.
			PropertyChanged.Invoke(this, new PropertyChangedEventArgs("LookUpResults"));
		}

		// Run a simulation of one game and return the winner.
		private Result SimulateGame(ref Result team1Result, ref Result team2Result, TeamData data)
		{
			Team team1 = data.GetTeam(team1Result.Team);
			Team team2 = data.GetTeam(team2Result.Team);

			// Run the math between two teams and receive out the scores of each team.
			(int team1Score, int team2Score) = TournamentMath.SimulateGame(team1, team2);

			// Create new, updated Results to prevent binding cache issues.
			team1Result = new Result
			{
				Seed = team1Result.Seed,
				Team = team1Result.Team,
				Score = team1Score,
				Winner = team1Score >= team2Score
			};
			team2Result = new Result
			{
				Seed = team2Result.Seed,
				Team = team2Result.Team,
				Score = team2Score,
				Winner = team1Score < team2Score
			};
			Result winnerResult = new Result
			{
				Seed = team1Score >= team2Score ? team1Result.Seed : team2Result.Seed,
				Team = team1Score >= team2Score ? team1Result.Team : team2Result.Team,
				Score = 0,		// Will be calculated next round.
				Winner = false
			};
			return winnerResult;
		}

		// Binds a field name to the results.
		public Result LookUpResults(string fieldName)
		{
			Result toReturn = new Result();
			if (Results.ContainsKey(fieldName))
			{
				toReturn = Results[fieldName];
			}
			return toReturn;
		}

		// The full list of possible field names.
		private static readonly string[] FieldNames = new string[]
		{
			"WestSeed1",
			"WestSeed16",
			"WestSeed8",
			"WestSeed9",
			"WestSeed5",
			"WestSeed12",
			"WestSeed4",
			"WestSeed13",
			"WestSeed6",
			"WestSeed11",
			"WestSeed3",
			"WestSeed14",
			"WestSeed7",
			"WestSeed10",
			"WestSeed2",
			"WestSeed15",
			"EastSeed1",
			"EastSeed16",
			"EastSeed8",
			"EastSeed9",
			"EastSeed5",
			"EastSeed12",
			"EastSeed4",
			"EastSeed13",
			"EastSeed6",
			"EastSeed11",
			"EastSeed3",
			"EastSeed14",
			"EastSeed7",
			"EastSeed10",
			"EastSeed2",
			"EastSeed15",
			"SouthSeed1",
			"SouthSeed16",
			"SouthSeed8",
			"SouthSeed9",
			"SouthSeed5",
			"SouthSeed12",
			"SouthSeed4",
			"SouthSeed13",
			"SouthSeed6",
			"SouthSeed11",
			"SouthSeed3",
			"SouthSeed14",
			"SouthSeed7",
			"SouthSeed10",
			"SouthSeed2",
			"SouthSeed15",
			"MidwestSeed1",
			"MidwestSeed16",
			"MidwestSeed8",
			"MidwestSeed9",
			"MidwestSeed5",
			"MidwestSeed12",
			"MidwestSeed4",
			"MidwestSeed13",
			"MidwestSeed6",
			"MidwestSeed11",
			"MidwestSeed3",
			"MidwestSeed14",
			"MidwestSeed7",
			"MidwestSeed10",
			"MidwestSeed2",
			"MidwestSeed15",
			"WestWinner1",
			"WestWinner2",
			"WestWinner3",
			"WestWinner4",
			"WestWinner5",
			"WestWinner6",
			"WestWinner7",
			"WestWinner8",
			"EastWinner1",
			"EastWinner2",
			"EastWinner3",
			"EastWinner4",
			"EastWinner5",
			"EastWinner6",
			"EastWinner7",
			"EastWinner8",
			"SouthWinner1",
			"SouthWinner2",
			"SouthWinner3",
			"SouthWinner4",
			"SouthWinner5",
			"SouthWinner6",
			"SouthWinner7",
			"SouthWinner8",
			"MidwestWinner1",
			"MidwestWinner2",
			"MidwestWinner3",
			"MidwestWinner4",
			"MidwestWinner5",
			"MidwestWinner6",
			"MidwestWinner7",
			"MidwestWinner8",
			"WestSweetSixteen1",
			"WestSweetSixteen2",
			"WestSweetSixteen3",
			"WestSweetSixteen4",
			"EastSweetSixteen1",
			"EastSweetSixteen2",
			"EastSweetSixteen3",
			"EastSweetSixteen4",
			"SouthSweetSixteen1",
			"SouthSweetSixteen2",
			"SouthSweetSixteen3",
			"SouthSweetSixteen4",
			"MidwestSweetSixteen1",
			"MidwestSweetSixteen2",
			"MidwestSweetSixteen3",
			"MidwestSweetSixteen4",
			"WestEliteEight1",
			"WestEliteEight2",
			"EastEliteEight1",
			"EastEliteEight2",
			"SouthEliteEight1",
			"SouthEliteEight2",
			"MidwestEliteEight1",
			"MidwestEliteEight2",
			"WestFinalFour",
			"EastFinalFour",
			"SouthFinalFour",
			"MidwestFinalFour",
			"WestEastChampion",
			"SouthMidwestChampion",
			"Champion",
		};
	}
}
