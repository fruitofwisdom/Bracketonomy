# Bracketonomy
An application to automatically generate NCAA brackets from team data.

Team data is stored in CSV format with a column each representing:
* ID - a unique ID
* Name - the team's name
* Seed - the team's tournament seed
* BPI Off - the team's BPI offensive strength
* BPI Def - the team's BPI defensive strength
* Record W - the number of wins in their overall record
* Record L - the number of losses in their overall record
* Conference W - the team's number of conference wins
* Conference L - the team's number of conference losses
* Vs Top 25 W - wins against top-25 ranked teams
* Vs Top 25 L = losses against top-25 ranked teams
* Last 10 W - wins in the last 10 games
* Last 10 L - losses in the last 10 games
* SOS Rank - the team's Strength of Schedule ranking
* SOR Rank - the team's Strength of Record ranking
* PPG - the team's average points-per-game
* Opposing PPG - their opposing team's average points-per-game
* Field Goal Percentage
* Three-Point Percentage
* Free Throw Percentage

These stats are then fed in to various equations that can be tweaked with weights to predict an NCAA tournament bracket.
