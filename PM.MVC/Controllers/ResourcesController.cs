using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using PM.MVC.Models.EF;
using PM.MVC.Models.Services.Interfaces;

namespace PM.MVC.Controllers
{
    [Authorize]
    public class ResourcesController : Controller
    {
        private readonly IQualificationRepository<Resource> _repository;

            public ResourcesController(IQualificationRepository<Resource> repository)
            {
                _repository = repository;
            }

        // GET: Resources
        public async Task<ActionResult> Index()
        {
            return View(await _repository.GetAllAsync());
        }

        // GET: Resources/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Resources/Create
        [HttpPost]
        public async Task<ActionResult> Create([Bind("Id,Name")] Resource resource)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(resource);
                return RedirectToAction("Index");
            }

            return View(resource);
        }

        // GET: Resources/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            Resource resource = await _repository.GetOneAsync(id);

            return View(resource);
        }

        // POST: Resources/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit([Bind("Id,Name,Qualifications")] Resource resource)
        {
            if (ModelState.IsValid)
            {
                await _repository.UpdateAsync(resource);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        // POST: Resources/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost("Resources/{id:int:min(1)}/qualifications/{qualificationId:int:min(1)}")]
        public async Task<ActionResult> DeleteQualification(int id, int qualificationId)
        {
            await _repository.DeleteQualificationAsync(id, qualificationId);
            return RedirectToAction("Edit", new { id });
        }

        [HttpPost("Resources/{id:int:min(1)}/qualifications")]
        public async Task<ActionResult> AddQualification(int id, Qualification createRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _repository.AddQualificationAsync(id, createRequest);

            return RedirectToAction("Edit", new { id });
        }

        [HttpGet("Resources/{id:int:min(1)}/qualifications")]
        public ActionResult AddQualification()
        {
            return View();
        }
    }
}
