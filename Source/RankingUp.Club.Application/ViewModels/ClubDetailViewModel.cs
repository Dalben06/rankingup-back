using System.ComponentModel.DataAnnotations;

namespace RankingUp.Club.Application.ViewModels
{
    public class ClubDetailViewModel
    {
        [Key]
        public Guid UUId { get; set; }
        [Required(ErrorMessage = "O Nome do Clube é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome do Clube pode ter no maxino {1} caracteres")]
        public string Name { get; set; }
        [StringLength( 200, ErrorMessage = "O Descrição do Clube pode ter no maxino {1} caracteres")]
        public string Description { get; set; }
        [Required(ErrorMessage = "O Endereço é obrigatório")]
        [StringLength( 100, ErrorMessage = "O Endereço pode ter no maxino {1} caracteres")]
        public string Address { get; set; }
        [Required(ErrorMessage = "O Cidade é obrigatório")]
        [StringLength( 50, ErrorMessage = "O Cidade pode ter no maxino {1} caracteres")]
        public string City { get; set; }
        [Required(ErrorMessage = "O Estado é obrigatório")]
        [StringLength( 50, ErrorMessage = "O Estado pode ter no maxino {1} caracteres")]
        public string State { get; set; }
        [Required(ErrorMessage = "O Pais é obrigatório")]
        [StringLength( 50, ErrorMessage = "O Pais pode ter no maxino {1} caracteres")]
        public string Country { get; set; }
        [Required(ErrorMessage = "O Codigo Postal é obrigatório")]
        [StringLength( 30, ErrorMessage = "O Codigo Postal pode ter no maxino {1} caracteres")]
        public string PostalCode { get; set; }
       
        [StringLength( 50, ErrorMessage = "O Telefone pode ter no maxino {1} caracteres")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "O Horario de Inicio de Funcionamento é obrigatório")]
        public string BusinessHourStart { get; set; }
        [Required(ErrorMessage = "O Horario de encerramento de Funcionamento é obrigatório")]
        public string BusinessHourEnd { get; set; }
        [StringLength( 100, ErrorMessage = "O Facebook pode ter no maxino {1} caracteres")]
        public string FacebookUrl { get; set; }
        [StringLength( 100, ErrorMessage = "O Instagram pode ter no maxino {1} caracteres")]
        public string InstagramUrl { get; set; }
        [StringLength( 100, ErrorMessage = "O Twitter pode ter no maxino {1} caracteres")]
        public string TwitterUrl { get; set; }

        public int UserId { get; set; }

        //public IEnumerable<> MyProperty { get; set; }

    }
}
