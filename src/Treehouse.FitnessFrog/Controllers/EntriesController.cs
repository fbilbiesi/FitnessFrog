using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            Entry entry = new Entry()
            {
                Date = DateTime.Today.Date,

            };
            SetupSelectListItems();
            return View(entry);
        }

        [ActionName("Add"), HttpPost]
        public ActionResult AddPost(Entry entry)
        {
            ValidateDuration(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");

            }

            SetupSelectListItems();
            return View();
        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = _entriesRepository.GetEntry((int)id);

            if (entry == null)
            {
                return HttpNotFound();
            }

            SetupSelectListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
            ValidateDuration(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);

                return RedirectToAction("Index");

            }

            SetupSelectListItems();
            return View(entry);
            
        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }

        private void ValidateDuration(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Value of duration must be greater than 0");
            }
        }

        private void SetupSelectListItems()
        {
            ViewBag.ActivitiesSelectList = new SelectList(Data.Data.Activities, "Id", "Name");
        }
    }
}