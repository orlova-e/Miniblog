using Domain.Entities.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Domain.Entities
{
    public class CheckList : IEnumerable<string>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string VerifiableWords { get; set; }
        public CheckAction CheckAction { get; set; }
        protected IEnumerable<string> words = new List<string>();
        public IEnumerator<string> GetEnumerator()
        {
            if (!words.Any() && !string.IsNullOrWhiteSpace(VerifiableWords))
                words = VerifiableWords.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

            return words.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (!words.Any() && !string.IsNullOrWhiteSpace(VerifiableWords))
                words = VerifiableWords.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();

            return words.GetEnumerator();
        }
    }
}
