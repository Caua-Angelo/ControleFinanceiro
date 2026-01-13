namespace ControleFerias.testes.Common
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Mensagem { get; set; } = null!;
        public T? Data { get; set; }
        public List<CampoErro>? Erros { get; set; }

        // 🔹 Métodos auxiliares opcionais (facilitam uso no Controller)
        public static ApiResponse<T> Sucesso(string mensagem, T data, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Mensagem = mensagem,
                Data = data,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<T> Falha(string mensagem, List<CampoErro>? erros = null, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Mensagem = mensagem,
                Erros = erros,
                StatusCode = statusCode
            };
        }
    }

    public class CampoErro
    {
        public string Campo { get; set; } = null!;
        public List<string> Mensagens { get; set; } = new List<string>();
    }
}
