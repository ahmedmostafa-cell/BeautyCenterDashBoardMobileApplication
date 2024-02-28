using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JamalKhanah.Core.Entity.QuestionsAndAnswersData;
using JamalKhanah.RepositoryLayer.Interfaces;

namespace JamalKhanah.Controllers.MVC
{
    public class QuestionsAndAnswersSectionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuestionsAndAnswersSectionsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
  // GET: QuestionsAndAnswersSections
    public async Task<IActionResult> Index()
    {
        return View(await _unitOfWork.QuestionsAndAnswersSections.FindAllAsync(s=>s.IsDeleted==false));
    }

   

    // GET: QuestionsAndAnswersSections/Create
    public IActionResult Create()
    {
        return View();
    }

 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create( QuestionsAndAnswersSection questionsAndAnswersSection)
    {
        if (!ModelState.IsValid) return View(questionsAndAnswersSection);
      
            
        await _unitOfWork.QuestionsAndAnswersSections.AddAsync(questionsAndAnswersSection);
        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: QuestionsAndAnswersSections/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _unitOfWork.QuestionsAndAnswersSections == null)
        {
            return NotFound();
        }

        var questionsAndAnswersSection = await _unitOfWork.QuestionsAndAnswersSections
            .FindAsync(m => m.Id == id && m.IsDeleted == false);
        if (questionsAndAnswersSection == null)
        {
            return NotFound();
        }
        return View(questionsAndAnswersSection);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, QuestionsAndAnswersSection questionsAndAnswersSection)
    {
        if (id != questionsAndAnswersSection.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid) return View(questionsAndAnswersSection);
        try
        {
           
            _unitOfWork.QuestionsAndAnswersSections.Update(questionsAndAnswersSection);
            await _unitOfWork.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!QuestionsAndAnswersSectionExists(questionsAndAnswersSection.Id))
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

    public async Task<IActionResult> Delete(int id)
    {
        if (_unitOfWork.QuestionsAndAnswersSections == null)
        {
            return Problem("Entity set 'ApplicationContext.QuestionsAndAnswersSections'  is null.");
        }
        var questionsAndAnswersSection = await _unitOfWork.QuestionsAndAnswersSections
            .FindByQuery(
                criteria: s => s.Id == id && s.IsDeleted == false).FirstOrDefaultAsync();
        if (questionsAndAnswersSection != null)
        {
            questionsAndAnswersSection.IsDeleted = true;
            questionsAndAnswersSection.DeletedAt = DateTime.Now;
            _unitOfWork.QuestionsAndAnswersSections.Update(questionsAndAnswersSection);
        }

        await _unitOfWork.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }


    private bool QuestionsAndAnswersSectionExists(int id)
    {
        return _unitOfWork.QuestionsAndAnswersSections.IsExist(e => e.Id == id);
    }
    }
    
}
