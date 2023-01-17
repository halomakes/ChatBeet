﻿using ChatBeet.Data;
using ChatBeet.Data.Entities;
using ChatBeet.Models;
using ChatBeet.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBeet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Duration = 300)]
    public class SuspicionController : Controller
    {
        private readonly UserPreferencesService userPreferencesService;
        private readonly SuspicionService suspicionService;

        public SuspicionController(UserPreferencesService userPreferencesService, SuspicionService suspicionService)
        {
            this.userPreferencesService = userPreferencesService;
            this.suspicionService = suspicionService;
        }

        /// <summary>
        /// Get current suspicion levels
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuspicionRank>>> GetSuspicionLevels()
        {
            var mostSuspicious = (await suspicionService.GetSuspicionsAsync())
                .AsQueryable()
                .GroupBy(s => s.Suspect.ToLower())
                .Select(g => new
                {
                    Nick = g.Key,
                    Active = g.Where(grp => grp.TimeReported >= suspicionService.ActiveWindowStart).Count(),
                    Total = g.Count()
                })
                .OrderByDescending(t => t.Active)
                .ToList();

            var colorPrefs = await userPreferencesService.Get(mostSuspicious.Select(s => s.Nick), UserPreference.GearColor);

            var result = mostSuspicious.Select(s =>
            {
                var pref = colorPrefs.FirstOrDefault(p => p.Nick.Equals(s.Nick, StringComparison.OrdinalIgnoreCase));
                return new SuspicionRank
                {
                    Nick = pref?.Nick ?? s.Nick,
                    Level = s.Active,
                    LifetimeLevel = s.Total,
                    Color = pref?.Value
                };
            }).ToList();

            return Ok(result);
        }
    }
}
