namespace LubricentroApi.DTOs
{
    // Parámetros que recibiremos desde la URL.
    public class ParametrosPaginacionDto
    {
        private int _pagina = 1;
        private int _tamanoPagina = 10;

        // La página nunca puede ser menor a 1.
        public int Pagina
        {
            get => _pagina;
            set => _pagina = value < 1 ? 1 : value;
        }

        // Por defecto mostramos 10 registros.
        // Como máximo permitimos 50 por página.
        public int TamanoPagina
        {
            get => _tamanoPagina;
            set
            {
                if (value < 1)
                    _tamanoPagina = 10;
                else if (value > 50)
                    _tamanoPagina = 50;
                else
                    _tamanoPagina = value;
            }
        }
    }

    // Respuesta genérica para cualquier listado paginado.
    public class RespuestaPaginadaDto<T>
    {
        public int PaginaActual { get; set; }

        public int TamanoPagina { get; set; }

        public int TotalRegistros { get; set; }

        public int TotalPaginas { get; set; }

        public bool TienePaginaAnterior { get; set; }

        public bool TienePaginaSiguiente { get; set; }

        public List<T> Datos { get; set; } = new List<T>();
    }
}