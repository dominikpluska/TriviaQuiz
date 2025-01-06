﻿using QuizAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizAPI.Dto
{
    public class GameSessionDto
    {
        public string GameSessionId { get; set; }
        public int UserId { get; set; }
        public string SessionTime { get; set; }
    }
}
