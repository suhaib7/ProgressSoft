using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.BusinessCard
{
    public class BusinessCardModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(10)]
        public string Phone { get; set; }

        [StringLength(int.MaxValue)]
        public string Photo { get; set; }

        [Required]
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
