﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.ViewModels
{
    public class TeamViewData
    {
        public int Id { get; private set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ot { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsAgainst { get; set; }
        public int Points { get; set; }
        public int DivisionRank { get; set; }
        public int ConferenceRank { get; set; }
        public int LeagueRank { get; set; }
        public int GamesPlayed { get; set; }
        public string Division { get; set; }
        public string Conference { get; set; }
        public List<GameViewData> Games { get; set; }
        public List<GoalieViewData> Goalies { get; set; }
        public List<SkaterViewData> Skaters { get; set; }

        public TeamViewData(int id, string name)
        {
            Id = id;
            Name = name;
            Logo = GetLogoPath(id);
        }

        public void RefreshData()
        {
            Logo = GetLogoPath(Id);
        }

        private string GetLogoPath(int id)
        {
            switch (id)
            {
                case 1: return "https://upload.wikimedia.org/wikipedia/en/9/9f/New_Jersey_Devils_logo.svg";
                case 2: return "https://upload.wikimedia.org/wikipedia/en/4/42/Logo_New_York_Islanders.svg";
                case 3: return "https://upload.wikimedia.org/wikipedia/commons/a/ae/New_York_Rangers.svg";
                case 4: return "https://upload.wikimedia.org/wikipedia/en/d/dc/Philadelphia_Flyers.svg";
                case 5: return "https://upload.wikimedia.org/wikipedia/en/c/c0/Pittsburgh_Penguins_logo_%282016%29.svg";
                case 6: return "https://upload.wikimedia.org/wikipedia/en/1/12/Boston_Bruins.svg";
                case 7: return "https://upload.wikimedia.org/wikipedia/en/9/9e/Buffalo_Sabres_Logo.svg";
                case 8: return "https://upload.wikimedia.org/wikipedia/commons/6/69/Montreal_Canadiens.svg";
                case 9: return "https://upload.wikimedia.org/wikipedia/en/d/db/Ottawa_Senators.svg";
                case 10: return "https://upload.wikimedia.org/wikipedia/en/b/b6/Toronto_Maple_Leafs_2016_logo.svg";
                case 12: return "https://upload.wikimedia.org/wikipedia/en/3/32/Carolina_Hurricanes.svg";
                case 13: return "https://upload.wikimedia.org/wikipedia/en/4/43/Florida_Panthers_2016_logo.svg";
                case 14: return "https://upload.wikimedia.org/wikipedia/en/2/2f/Tampa_Bay_Lightning_Logo_2011.svg";
                case 15: return "https://upload.wikimedia.org/wikipedia/en/2/2d/Washington_Capitals.svg";
                case 16: return "https://upload.wikimedia.org/wikipedia/en/2/29/Chicago_Blackhawks_logo.svg";
                case 17: return "https://upload.wikimedia.org/wikipedia/en/e/e0/Detroit_Red_Wings_logo.svg";
                case 18: return "https://upload.wikimedia.org/wikipedia/en/9/9c/Nashville_Predators_Logo_%282011%29.svg";
                case 19: return "https://upload.wikimedia.org/wikipedia/en/e/ed/St._Louis_Blues_logo.svg";
                case 20: return "https://upload.wikimedia.org/wikipedia/en/6/60/Calgary_Flames_Logo.svg";
                case 21: return "https://upload.wikimedia.org/wikipedia/en/4/45/Colorado_Avalanche_logo.svg";
                case 22: return "https://upload.wikimedia.org/wikipedia/en/4/4d/Logo_Edmonton_Oilers.svg";
                case 23: return "https://upload.wikimedia.org/wikipedia/en/3/3a/Vancouver_Canucks_logo.svg";
                case 24: return "https://upload.wikimedia.org/wikipedia/en/7/72/Anaheim_Ducks.svg";
                case 25: return "https://upload.wikimedia.org/wikipedia/en/c/ce/Dallas_Stars_logo_%282013%29.svg";
                case 26: return "https://upload.wikimedia.org/wikipedia/en/6/63/Los_Angeles_Kings_logo.svg";
                case 28: return "https://upload.wikimedia.org/wikipedia/en/3/37/SanJoseSharksLogo.svg";
                case 29: return "https://upload.wikimedia.org/wikipedia/en/5/5d/Columbus_Blue_Jackets_logo.svg";
                case 30: return "https://upload.wikimedia.org/wikipedia/en/1/1b/Minnesota_Wild.svg";
                case 52: return "https://upload.wikimedia.org/wikipedia/en/9/93/Winnipeg_Jets_Logo_2011.svg";
                case 53: return "https://upload.wikimedia.org/wikipedia/en/2/27/Arizona_Coyotes.svg";
                case 54: return "https://upload.wikimedia.org/wikipedia/en/a/ac/Vegas_Golden_Knights_logo.svg";
                default: return "";
            }
        }

    }
}