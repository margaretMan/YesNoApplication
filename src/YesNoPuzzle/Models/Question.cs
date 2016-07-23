
using System;
using System.ComponentModel.DataAnnotations;

namespace YesNoPuzzle.Models
{
    public class Question
    {
        public int Id { get; set; }        

        public ApplicationUser User { get; set; }        

        public Game Game { get; set; }
        
        [Required]
        [Display(Name = "Question")]
        public string Text { get; set; }

        public int State { get; set; }//0 - no answer; 1 - yes; 2 - no; 3 - no matter;

        public DateTime QuestionDate { get; set; }
    }
}
