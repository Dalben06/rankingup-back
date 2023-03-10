using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RankingUp.Sport.Application.ViewModels
{
    public class SportViewModel
    {
        [Key]
        public string UUId { get; set; }
        [Required(ErrorMessage = "O Nome do Esporte é obrigatório")]
        [StringLength(50, ErrorMessage = "O Nome do Esporte pode ter no maxino {1} caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O Descrição do Esporte é obrigatório")]
        [StringLength(50, ErrorMessage = "O Descrição do Esporte pode ter no maximo {1} caracteres")]
        public string Description { get; set; }
    }
}
