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
    public class SkillsController : Controller
    {
        private readonly IRepository<Skill> _repository;

        public SkillsController(IRepository<Skill> repository)
        {
            _repository = repository;
        }

        // GET: Skills
        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }

        // GET: Skills/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Skills/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind("Id,Name")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(skill);
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        // GET: Skills/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Skill skill = await _repository.GetOneAsync(id);
            return View(skill);
        }

        // POST: Skills/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit([Bind("Id,Name")] Skill skill)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(skill);
                return RedirectToAction("Index");
            }

            return View(skill);
        }

        // POST: Skills/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

    }
}
