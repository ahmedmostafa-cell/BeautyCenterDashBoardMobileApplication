using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.QuestionsAndAnswersData;
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC
{
    public class QuestionsAndAnswersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionsAndAnswersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        // GET: QuestionsAndAnswers
        public async Task<IActionResult> Index()
        {
            var applicationContext = await _unitOfWork.QuestionsAndAnswers.FindAllAsync(
                    criteria:s=>s.IsDeleted==false,
                    include:s=>s.Include(q => q.QuestionsAndAnswersSection));
            return View( applicationContext);
        }

        // GET: QuestionsAndAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _unitOfWork.QuestionsAndAnswers == null)
            {
                return NotFound();
            }

            var questionsAndAnswers = await _unitOfWork.QuestionsAndAnswers
                .FindAsync(m => m.Id == id && m.IsDeleted == false, include: s => s.Include(q => q.QuestionsAndAnswersSection));
               
            if (questionsAndAnswers == null)
            {
                return NotFound();
            }

            return View(questionsAndAnswers);
        }

        // GET: QuestionsAndAnswers/Create
        public IActionResult Create()
        {
            ViewData["QuestionsAndAnswersSectionId"] = new SelectList(_unitOfWork.QuestionsAndAnswersSections
                .FindAll(s=>s.IsDeleted==false), "Id", "Section");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionsAndAnswers questionsAndAnswers)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.QuestionsAndAnswers.Add(questionsAndAnswers);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionsAndAnswersSectionId"] = new SelectList(_unitOfWork.QuestionsAndAnswersSections
                .FindAll(s=>s.IsDeleted==false), "Id", "Section");
            return View(questionsAndAnswers);
        }

        // GET: QuestionsAndAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _unitOfWork.QuestionsAndAnswers == null)
            {
                return NotFound();
            }

            var questionsAndAnswers = await _unitOfWork.QuestionsAndAnswers
                .FindAsync(m => m.Id == id && m.IsDeleted == false, include: s => s.Include(q => q.QuestionsAndAnswersSection));
            if (questionsAndAnswers == null)
            {
                return NotFound();
            }
            ViewData["QuestionsAndAnswersSectionId"] = new SelectList(_unitOfWork.QuestionsAndAnswersSections
                .FindAll(s=>s.IsDeleted==false), "Id", "Section");
            return View(questionsAndAnswers);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuestionsAndAnswers questionsAndAnswers)
        {
            if (id != questionsAndAnswers.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.QuestionsAndAnswers.Update(questionsAndAnswers);
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionsAndAnswersExists(questionsAndAnswers.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionsAndAnswersSectionId"] = new SelectList(_unitOfWork.QuestionsAndAnswersSections
                .FindAll(s=>s.IsDeleted==false), "Id", "Section");
            return View(questionsAndAnswers);
        }

        // delete: QuestionsAndAnswers/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (_unitOfWork.QuestionsAndAnswers == null)
            {
                return Problem("Entity set 'ApplicationContext.QuestionsAndAnswers'  is null.");
            }
            var questionsAndAnswers = await _unitOfWork.QuestionsAndAnswers
                .FindByQuery(
                    criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
            if (questionsAndAnswers != null)
            {
                questionsAndAnswers.IsDeleted = true;
                questionsAndAnswers.DeletedAt = DateTime.Now;
                _unitOfWork.QuestionsAndAnswers.Update(questionsAndAnswers);
            }

            await _unitOfWork.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        private bool QuestionsAndAnswersExists(int id)
        {
          return _unitOfWork.QuestionsAndAnswers.IsExist(e => e.Id == id);
        }
    }
}
