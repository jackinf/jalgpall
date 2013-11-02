﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Uptime_Jalgpall.Models;

namespace Uptime_Jalgpall.Controllers
{
    public class PairController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Pair/
        public ActionResult Index()
        {
            var results = from c in db.Pairs
                        join d in db.Teams on c.Team1.ID equals d.ID
                        join e in db.Teams on c.Team2.ID equals e.ID
                        select new
                        {
                            ID = c.ID,
                            Team1 = d,
                            Team2 = e,
                            Team1Scored = c.Team1Scored,
                            Team2Scored = c.Team2Scored
                        };

            List<Pair> pairs = new List<Pair>();
            foreach (var result in results)
            {
                Pair pair = new Pair();
                pair.ID = result.ID;
                pair.Team1 = result.Team1;
                pair.Team2 = result.Team2;
                pair.Team1Scored = result.Team1Scored;
                pair.Team2Scored = result.Team2Scored;
                pairs.Add(pair);
            }

            return View(pairs);
        }

        // GET: /Pair/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pair pair = db.Pairs.Find(id);
            if (pair == null)
            {
                return HttpNotFound();
            }
            return View(pair);
        }

        // GET: /Pair/Create
        public ActionResult Create()
        {
            var teams = db
                .Teams
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                });
            var tournaments = db
                .Tournaments
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                });
            ViewBag.teams = new SelectList(teams, "Value", "Text");
            ViewBag.tournaments = new SelectList(tournaments, "Value", "Text");

            return View();
        }

        // POST: /Pair/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ID,Team1,Team2,Winner,Tournament")] Pair pair)
        {
            if (ModelState.IsValid)
            {
                db.Pairs.Add(pair);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pair);
        }

        // GET: /Pair/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pair pair = db.Pairs.Find(id);
            if (pair == null)
            {
                return HttpNotFound();
            }
            return View(pair);
        }

        // POST: /Pair/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Team1Scored,Team2Scored")] Pair pair)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pair).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pair);
        }

        // GET: /Pair/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pair pair = db.Pairs.Find(id);
            if (pair == null)
            {
                return HttpNotFound();
            }
            return View(pair);
        }

        // POST: /Pair/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pair pair = db.Pairs.Find(id);
            db.Pairs.Remove(pair);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Step1()
        {
            return View(db.Tournaments.ToList());
        }

        /*
         * GET 
         * id - tournament id
         */ 
        public ActionResult Step2(int id)
        {
            Pair pair = FindTournament(id);
            if (pair.Tournament == null)
                return HttpNotFound();
            ViewBag.pair1_team1 = GetSelectListOfTeams();
            ViewBag.pair1_team2 = ViewBag.pair1_team1;

            return View(pair);
        }

        // POST: Pair/Step2
        //

        [HttpPost]
        public ActionResult Step2(FormCollection fc, int id)
        {
            int team1_id;
            int team2_id;

            for (int i = 1; i <= fc.AllKeys.Count(); i+=2)
            {
                try
                {
                    team1_id = int.Parse(fc.Get(i));
                    team2_id = int.Parse(fc.Get(i+1));
                    AddNewPair(team1_id, team2_id, id);
                }
                catch
                {
                    break;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Pair");
        }

        private void AddNewPair(int team1_id, int team2_id, int tournament_id)
        {
            // Cannot play against yourself
            if (team1_id == team2_id)
            {
                return;
            }

            if (DoesPairExists(team1_id, team2_id, tournament_id)) 
            {
                return;
            }

            Pair pair = FindTournament(tournament_id);
            Team team1 = db.Teams.Find(team1_id);
            Team team2 = db.Teams.Find(team2_id);
            if (pair.Tournament == null || team1 == null || team2 == null)
                return;
            pair.Team1 = team1;
            pair.Team2 = team2;
            db.Pairs.Add(pair);
        }

        private bool DoesPairExists(int team1_id, int team2_id, int tournament_id)
        {
            var result = db.Pairs.Select(c => c).Where(c => c.Team1.ID == team1_id && c.Team2.ID == team2_id && c.Tournament.ID == tournament_id).DefaultIfEmpty();
            return result == null ? false : true;
        }

        public PartialViewResult AjaxAddPair(int pairs)
        {
            string keyForTeam1 = string.Format("pair{0}_team1", pairs.ToString());
            string keyForTeam2 = string.Format("pair{0}_team2", pairs.ToString());
            ViewData[keyForTeam1] = GetSelectListOfTeams();
            ViewData[keyForTeam2] = GetSelectListOfTeams();

            ViewModelHelper helper = new ViewModelHelper();
            helper.HelperIntList = new List<int>();
            helper.HelperIntList.Add(pairs);
            helper.HelperStringList = new List<string>();
            helper.HelperStringList.Add(keyForTeam1);
            helper.HelperStringList.Add(keyForTeam2);
            return PartialView("PartialAddPair", helper);
        }

        /*
         * Searches for tournament.
         */ 
        private Pair FindTournament(int id)
        {
            // id must be correct
            if (id <= 0)
            {
                return new Pair();
            }

            // Search for tournament with this id
            var tournament = db.Tournaments.Find(id);
            if (tournament == null)
            {
                return new Pair();
            }

            // Store tournament in Pair model
            Pair pair = new Pair { Tournament = tournament};
            return pair;
        }

        private SelectList GetSelectListOfTeams()
        {
            var teams = db
                .Teams
                .ToList()
                .Select(c => new SelectListItem
                {
                    Value = c.ID.ToString(),
                    Text = c.Name
                });
            return new SelectList(teams, "Value", "Text");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}