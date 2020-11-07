using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PM.MVC.Models.EF;
using PM.MVC.Models.Services.Interfaces;

namespace PM.MVC.Controllers
{
    [Authorize]
    public class QualificationsController : Controller
    {
        private readonly IRepository<Qualification> _repository;

        public QualificationsController(IRepository<Qualification> repository)
        {
            _repository = repository;
        }

        // GET: Qualifications
        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }

        // GET: Qualifications/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Qualifications/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind("Level,Skill")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(qualification);
                return RedirectToAction("Index");
            }

            return View(qualification);
        }

        // GET: Qualifications/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Qualification qualification = await _repository.GetOneAsync(id);

            return View(qualification);
        }

        // POST: Qualifications/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit([Bind("Id,Level,Skill")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(qualification);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Edit");
        }

        // POST: Qualifications/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
