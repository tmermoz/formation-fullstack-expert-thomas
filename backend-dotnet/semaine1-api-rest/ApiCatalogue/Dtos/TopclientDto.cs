namespace ApiCatalogue.Dtos
{
    public class TopClientDto
    {
        public string NomClient { get; set; } = string.Empty;
        public decimal TotalDepense { get; set; }
        public int NombreAchats { get; set; }
    }
}