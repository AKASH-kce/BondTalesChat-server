namespace BondTalesChat_Server.Models.Dto
{
    public class IceCandidateDto
    {
        public string candidate { get; set; } = string.Empty;
        public string? sdpMid { get; set; }
        public int? sdpMLineIndex { get; set; }
    }
}


