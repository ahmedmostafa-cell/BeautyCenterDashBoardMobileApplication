using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JamalKhanah.Core.Entity.QuestionsAndAnswersData
{
    public class QuestionsAndAnswersSection : BaseEntity
    {
        [Required(ErrorMessage = "يجب أدخال القسم "), StringLength(50), MinLength(5, ErrorMessage = "يجب أن يكون القسم أكبر من 5 حروف")]
        [Display(Name = "القسم")]
        public string Section { get; set; }
        public List<QuestionsAndAnswers> QuestionsAndAnswers { get; set; }
    }
}
