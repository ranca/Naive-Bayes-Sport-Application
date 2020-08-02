﻿using Dapper;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.IO;
using DbServices.Models;
using System.Linq;

namespace DbServices.Services
{
    public class TeamService : DatabaseService
    {
        public TeamService() : base() { }
        public Team GetTeam(int teamId)
        {
            var sql = @"SELECT * FROM fan.Teams a
                        WHERE a.TeamId = @teamId";
            using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
            {
                var res = connection.Query<Team>(sql, new { teamId }).ToList().First();
                return res;
            }
        }
        public int InsertTeam(string a, string b, string c)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
                {
                    var affectedRows = connection.Execute("Insert into fan.Teams (TeamId, TeamName, Country) " +
                                "values (@Id, @Name, @Nation)", new { Id = a, Name = b, Nation = c });
                    return affectedRows;
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }

        public int UpdateLeague(int teamId, string teamName, object league, int season)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
                {
                    var teams = connection.Query<Team>("Select TeamId, TeamName from fan.Teams").AsList();

                    if (teamId == 0)
                    {
                        teamId = teams.Find(el => el.TeamName == teamName).TeamId;
                    }

                    var affectedRows = connection.Execute("Insert into fan.TeamsSeason (TeamId, LeagueId, SeasonId) " +
            "values (@TeamId, @LeagueId, @SeasonId)", new { TeamId = teamId, ((League)league).LeagueId, SeasonId = season });
                    return affectedRows;
                }
                
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }
        public int DeleteLeague(int teamId, string teamName, object league, int season)
        {
            try
            {

                using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
                {
                    var teams = connection.Query<Team>("Select TeamId, TeamName from fan.Teams").AsList();

                    if (teamId == 0)
                    {
                        teamId = teams.Find(el => el.TeamName == teamName).TeamId;
                    }

                    var affectedRows = connection.Execute("delete from fan.TeamsSeason where " +
            "TeamId = @TeamId and LeagueId = @LeagueId and SeasonId = @SeasonId", new { TeamId = teamId, ((League)league).LeagueId, SeasonId = season });
                    return affectedRows;
                }

            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }
        public List<Player> GetPlayers(object teamObj, string season)
        {
            var team = (teamObj as Team);
            if (team == null)
            {
                return new List<Player>();
            }
            var sql = @"SELECT a.PlayerId, b.Nr, a.FullName, a.Nation, a.Nation2, d.Flag, a.Position, b.Games, b.Goals, b.Assists, b.Goals + b.Assists Points, b.PlusMinus, b.PIM, b.GoalsAgainstAvg, b.SavesPercent, b.SeasonId, b.Nr, c.TeamId, c.TeamName 
                        FROM fan.Players a
                        INNER JOIN fan.PlayersTeams b on a.PlayerId = b.PlayerId
                        INNER JOIN fan.Teams c ON c.TeamId = b.TeamId
                        INNER JOIN fan.Nations d ON d.NationId = a.Nation
                        WHERE c.TeamId = @TeamId and b.SeasonId = @Season
                        ORDER BY b.Nr";
            using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
            {
                var res = connection.Query<Player, PlayerSeason, Team, Player>(
                    sql, (player, playerSeason, team) => { playerSeason.Player = player; playerSeason.Team = team; return player; }, new { TeamId = team.TeamId, Season = season }, splitOn: "PlayerId,SeasonId,TeamId").AsList();
                return res;
            }
        }

        public int UpdateTeamLogo(string path, object teamId, object teamName)
        {
            byte[] bytes = File.ReadAllBytes(path);
            using (SqlConnection connection = new SqlConnection(m_builder.ConnectionString))
            {
                    var affectedRows = connection.Execute("update fan.Teams set TeamLogo = @Logo " +
            "where TeamId = @TeamId or TeamName = @TeamName", new { Logo = bytes, TeamId = teamId, TeamName = teamName});
                    return affectedRows;
            }
        }
    }
}