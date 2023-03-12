using RankingUp.Core.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace RankingUp.Club.Application.ViewModels
{
    public class ClubDetailViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "O Nome do Clube é obrigatório")]
        [StringLength(100, ErrorMessage = "O Nome do Clube pode ter no máximo {1} caracteres")]
        public string Name { get; set; }
        [StringLength( 200, ErrorMessage = "O Descrição do Clube pode ter no máximo {1} caracteres")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O Numero do Endereço é obrigatório")]
        [StringLength(10, ErrorMessage = "O  Numero do Endereço pode ter no máximo {1} caracteres")]
        public string AddressNumber { get; set; }
        [StringLength(100, ErrorMessage = "O Complemento do Endereço pode ter no máximo {1} caracteres")]
        public string AddressComplement { get; set; }
        [StringLength(100, ErrorMessage = "O Distrito do Endereço pode ter no máximo {1} caracteres")]
        public string AddressDistrict { get; set; }
        [Required(ErrorMessage = "O Endereço é obrigatório")]
        [StringLength(100, ErrorMessage = "O Endereço pode ter no máximo {1} caracteres")]
        public string Address { get; set; }
        [StringLength( 50, ErrorMessage = "O Cidade pode ter no máximo {1} caracteres")]
        public string City { get; set; }
        [Required(ErrorMessage = "O Estado é obrigatório")]
        [StringLength( 50, ErrorMessage = "O Estado pode ter no máximo {1} caracteres")]
        public string State { get; set; }
        [Required(ErrorMessage = "O Pais é obrigatório")]
        [StringLength( 50, ErrorMessage = "O Pais pode ter no máximo {1} caracteres")]
        public string Country { get; set; }
        [Required(ErrorMessage = "O Codigo Postal é obrigatório")]
        [StringLength( 30, ErrorMessage = "O Codigo Postal pode ter no máximo {1} caracteres")]
        public string PostalCode { get; set; }
       
        [StringLength( 50, ErrorMessage = "O Telefone pode ter no máximo {1} caracteres")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "O Horario de Inicio de Funcionamento é obrigatório")]
        public string BusinessHourStart { get; set; }
        [Required(ErrorMessage = "O Horario de encerramento de Funcionamento é obrigatório")]
        public string BusinessHourEnd { get; set; }
        [StringLength( 100, ErrorMessage = "O Facebook pode ter no máximo {1} caracteres")]
        public string FacebookUrl { get; set; }
        [StringLength( 100, ErrorMessage = "O Instagram pode ter no máximo {1} caracteres")]
        public string InstagramUrl { get; set; }
        [StringLength( 100, ErrorMessage = "O Twitter pode ter no máximo {1} caracteres")]
        public string TwitterUrl { get; set; }

        [StringLength(200, ErrorMessage = "O Email pode ter no máximo {1} caracteres")]
        public string Email { get; set; }


        //public IEnumerable<> MyProperty { get; set; }

    }
}
