using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamalKhanah.Core.Entity.QuestionsAndAnswersData
{
    public class QuestionsAndAnswers : BaseEntity
    {
        [Required(ErrorMessage = "يجب أدخال السؤال "), StringLength(500), MinLength(5, ErrorMessage = "يجب أن يكون السؤال أكبر من 5 حروف")]
        [Display(Name = "السؤال")]
        public string Question { get; set; }
        
        [Required(ErrorMessage = "يجب أدخال الجواب "), StringLength(500), MinLength(5, ErrorMessage = "يجب أن يكون الجواب أكبر من 5 حروف")]
        [Display(Name = "الجواب")]
        public string Answer { get; set; }
        

        [ForeignKey("QuestionsAndAnswersSection")]
        [Display(Name = "القسم")]
        [Required(ErrorMessage = "يجب أختيار القسم")]
        public int QuestionsAndAnswersSectionId { get; set; }
        [Display(Name = "القسم")]
        public QuestionsAndAnswersSection QuestionsAndAnswersSection { get; set; }
    }
}
