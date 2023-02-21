using Microsoft.AspNetCore.Http;

namespace RankingUp.Core.Extensions
{
    public static class ByteExtensions
    {

        public static byte[] ObterArquivoImagem(IFormFile form)
        {
            if (form is null || form?.Length <= 0)
                return Array.Empty<byte>();

            using (var logo = form.OpenReadStream())
            {
                using (BinaryReader br = new BinaryReader(logo))
                {
                    return br.ReadBytes((int)logo.Length);
                }
            }
        }
    }
}
