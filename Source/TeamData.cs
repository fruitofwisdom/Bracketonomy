using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Bracketonomy
{
	public class Team
	{
		public int Id;					// A unique ID.
		public string Name;				// The team's name.

		public int Seed;				// The team's tournament seed.
		public float BpiOff;			// The team's BPI offensive strength.
		public float BpiDef;			// The team's BPI defensive strength.

		public float RecordW;			// The number of wins in their overall record.
		public float RecordL;			// The number of losses in their overall record.
		public int ConferenceW;			// The team's number of conference wins.
		public int ConferenceL;			// The team's number of conference losses.
		public int VsTop25W;			// Wins against top-25 ranked teams.
		public int VsTop25L;			// Losses against top-25 ranked teams.
		public int Last10W;				// Wins in the last 10 games.
		public int Last10L;				// Losses in the last 10 games.
		public int SosRank;				// The team's Strength of Schedule ranking.
		public int SorRank;				// The team's Strength of Record ranking.

		public float Ppg;				// The team's average points-per-game.
		public float OppPgg;			// Their opposing team's average points-per-game.
		public float FieldGoalPer;		// field goal percentage
		public float ThreePointPer;		// three-point percentage
		public float FreeThrowPer;		// free throw percentage

		public Team(string[] fields)
		{
			Id = fields[0] == "" ? 0 : int.Parse(fields[0]);
			Name = fields[1].Trim();
			Seed = fields[2] == "" ? 0 : int.Parse(fields[2]);
			// BpiRank used to be field 3. Now ignored.
			BpiOff = fields[4] == "" ? 0.0f : float.Parse(fields[4]);
			BpiDef = fields[5] == "" ? 0.0f : float.Parse(fields[5]);
			RecordW = fields[6] == "" ? 0 : float.Parse(fields[6]);
			RecordL = fields[7] == "" ? 0 : float.Parse(fields[7]);
			ConferenceW = fields[8] == "" ? 0 : int.Parse(fields[8]);
			ConferenceL = fields[9] == "" ? 0 : int.Parse(fields[9]);
			VsTop25W = fields[10] == "" ? 0 : int.Parse(fields[10]);
			VsTop25L = fields[11] == "" ? 0 : int.Parse(fields[11]);
			Last10W = fields[12] == "" ? 0 : int.Parse(fields[12]);
			Last10L = fields[13] == "" ? 0 : int.Parse(fields[13]);
			SosRank = fields[14] == "" ? 0 : int.Parse(fields[14]);
			SorRank = fields[15] == "" ? 0 : int.Parse(fields[15]);
			Ppg = fields[16] == "" ? 0.0f : float.Parse(fields[16]);
			OppPgg = fields[17] == "" ? 0.0f : float.Parse(fields[17]);
			FieldGoalPer = fields[18] == "" ? 0.0f : float.Parse(fields[18]);
			ThreePointPer = fields[19] == "" ? 0.0f : float.Parse(fields[19]);
			FreeThrowPer = fields[20] == "" ? 0.0f : float.Parse(fields[20]);
		}
	}

	public class TeamData
	{
		public List<Team> Teams = new List<Team>();

		public Team GetTeam(string teamName)
		{
			Team toReturn = null;

			foreach (Team team in Teams)
			{
				if (team.Name == teamName)
				{
					toReturn = team;
					break;
				}
			}

			return toReturn;
		}

		public async Task<bool> ReadFile(StorageFile file)
		{
			Teams.Clear();

			IList<string> lines = await FileIO.ReadLinesAsync(file);
			foreach (string line in lines)
			{
				string[] fields = line.Split(',');
				if (fields.Length > 0 && char.IsDigit(fields[0][0]))
				{
					Teams.Add(new Team(fields));
				}
			}

			// Calculate some weights based off loaded data.
			foreach (Team team in Teams)
			{
				if (team.BpiOff < Weights.MinBpiOff)
				{
					Weights.MinBpiOff = team.BpiOff;
				}
				if (team.BpiOff > Weights.MaxBpiOff)
				{
					Weights.MaxBpiOff = team.BpiOff;
				}
				if (team.BpiDef < Weights.MinBpiDef)
				{
					Weights.MinBpiDef = team.BpiDef;
				}
				if (team.BpiDef > Weights.MaxBpiDef)
				{
					Weights.MaxBpiDef = team.BpiDef;
				}
			}

			return true;
		}
	}
}
